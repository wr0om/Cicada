using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Tasks;
using System;
using Java.Util;
using Firebase.Firestore;
using System.Collections.Generic;
using Android.Graphics;
using Android.Views;
using Android.Content;
using FFImageLoading;
using Android.Media;
using Random = System.Random;
using System.Threading;


namespace Lambda2
{
    /// <summary>
    /// The activity of the actual game, here is where we play the trivia game - Cicada
    /// </summary>
    
    [Activity(Label = "", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameActivity : Activity, Android.Views.View.IOnClickListener, Firebase.Firestore.IEventListener, IOnCompleteListener
    {

        Button btn0, btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9;
        Button btnSend, btnDelete;
        TextView tvTitle, tvMe, tvOpponent, tvMyPoints, tvOpponentPoints, tvMyAnswer, tvOpponentAnswer, tvQuestion, tvAnswer, tvCountDown;
        ImageView ivMePic, ivOpponentPic;
        SPData sp;
        Game game;
        User user, opponent;
        SqlDb sqlDb;
        FirebaseData fd;
        Task taskPullMyPic, taskPullOpponentPic, taskPresentQuestion, taskAnsweredQue, taskSavePlayerResult;
        string gameNum;
        MediaPlayer backgroud_music;
        List<Question> questions;
        int amountOfQuestions, lastIndex, rand;//rand is a global variable appointed to the current question (used by player (not host) primarilly)
        bool isHost, isNewQuestion, isInRest, isGameOver;
        TimerHandler timerHandler;
        Timer timer;
        HashMap gameHashMap;
        bool[] contains;
        Dialog d;
        Button btnDialogGoHome;
        TextView tvEndResult, tvDialogMyPoints, tvDialogOpponentPoints;
        int gameStatus;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);
            InitObjects();
            InitViews();
            InitPics();
            InitMusic();
        }
        /// <summary>
        /// Initializes questions from sql db, game hashmap that is needed to continue the game going forward
        /// </summary>
        private void InitGame()
        {
            questions = sqlDb.GetAllQuestions();
            amountOfQuestions = Constants.NUMBER_OF_QUESTIONS;
            gameHashMap = new HashMap();
            gameHashMap.Put(Constants.GAMENUM, game.GameNum);
            gameHashMap.Put(Constants.ISLIVE_GAME, true);
            gameHashMap.Put(Constants.HOST_GAME, user.UserName);
            gameHashMap.Put(Constants.PLAYER_GAME, opponent.UserName);
            StartGame();
            //keep working on the game
        }
        /// <summary>
        /// Handles the callbacks from firestore - responsible for pulling profile pictures of the player and opponent, presenting the questions
        /// , posting player answers to current game document and saving the players' results after the game is over.
        /// </summary>
        /// <param name="task"></param>
        public void OnComplete(Task task)
        {
            DocumentSnapshot ds = (DocumentSnapshot)task.Result;
            if (task == taskPullMyPic)
            {
                if (task.IsSuccessful && ds.Get(Constants.PROFILE_PIC_URL) != null)//if we did pull an image, we put it in the imageview (we are supposed to at this point)
                {
                    //if the user set a profile picture manually already, we add it to the user's information
                    user.ProfilePicture_url = (string)ds.Get(Constants.PROFILE_PIC_URL);
                    ImageService.Instance.LoadUrl(user.ProfilePicture_url).Retry(3, 200).FadeAnimation(true).DownSample(130, 100).Into(ivMePic);
                }
            }
            else if(task == taskPullOpponentPic)
            {
                if (task.IsSuccessful && ds.Get(Constants.PROFILE_PIC_URL) != null)//if we did pull an image, we put it in the imageview (we are supposed to at this point)
                {
                    //if the opponent set a profile picture manually already, we add it to the user's information
                    opponent.ProfilePicture_url = (string)ds.Get(Constants.PROFILE_PIC_URL);
                    ImageService.Instance.LoadUrl(opponent.ProfilePicture_url).Retry(3, 200).FadeAnimation(true).DownSample(130, 100).Into(ivOpponentPic);
                    InitGame();//we start the game only when we finish those tasks
                }
            }
            else if (task.IsSuccessful && task == taskPresentQuestion)
                PresentQuestion(rand);
            else if(task.IsSuccessful && task == taskAnsweredQue)
            {
                gameHashMap = SetHashMap(ds, gameHashMap);
                if (isHost)
                    gameHashMap.Put(Constants.HOST_ANSWER, int.Parse(tvMyAnswer.Text));
                else
                    gameHashMap.Put(Constants.PLAYER_ANSWER, int.Parse(tvMyAnswer.Text));
                timer.GameHM = gameHashMap;
                fd.AddDocumentToCollection(Constants.GAMES_COL, game.GameNum, gameHashMap);
            }
            else if(task.IsSuccessful && task == taskSavePlayerResult)
            {
                int currTieNum = ds.Get(Constants.TIE_NUM) != null ? (int)ds.Get(Constants.TIE_NUM) : 0;
                int currWinNum = ds.Get(Constants.WIN_NUM) != null ? (int)ds.Get(Constants.WIN_NUM) : 0;
                int currLossNum = ds.Get(Constants.LOSS_NUM) != null ? (int)ds.Get(Constants.LOSS_NUM) : 0;
                switch (gameStatus)//if tie = 0, if host wins = 1, if player wins = 2
                {
                    case 0:
                        fd.UpdateDocument(Constants.FS_USERS_COL, user.UserName, Constants.TIE_NUM, ++currTieNum);
                        break;
                    case 1:
                        if (isHost)
                            fd.UpdateDocument(Constants.FS_USERS_COL, user.UserName, Constants.WIN_NUM, ++currWinNum);
                        else
                            fd.UpdateDocument(Constants.FS_USERS_COL, user.UserName, Constants.LOSS_NUM, ++currLossNum);
                        break;
                    case 2:
                        if(isHost)
                            fd.UpdateDocument(Constants.FS_USERS_COL, user.UserName, Constants.LOSS_NUM, ++currLossNum);
                        else
                            fd.UpdateDocument(Constants.FS_USERS_COL, user.UserName, Constants.WIN_NUM, ++currWinNum);
                        break;
                }
                sp.SetData(Constants.TIE_NUM, currTieNum);
                sp.SetData(Constants.WIN_NUM, currWinNum);
                sp.SetData(Constants.LOSS_NUM, currLossNum);
            }
        }
        /// <summary>
        /// Handles callbacks from firestore, when certain values are changed, responsible for most of the game logic (when to show question/answer and when to change ect)
        /// </summary>
        /// <param name="value">value from firestore</param>
        /// <param name="error">exception</param>
        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            DocumentSnapshot ds = (DocumentSnapshot)value;
            if (!isHost && ds.Get(Constants.CURRENT_Q_INDEX) != null && lastIndex != (int)ds.Get(Constants.CURRENT_Q_INDEX) && !isGameOver)
            {
                isInRest = true;
                gameHashMap = SetHashMap(ds, gameHashMap);
                //reset the views before next question
                tvMyAnswer.Text = 0.ToString();
                tvOpponentAnswer.Text = 0.ToString();
                tvAnswer.Text = "תשובה";
                gameHashMap = SetHashMap(ds, gameHashMap);
                int index = (int)ds.Get(Constants.CURRENT_Q_INDEX);
                lastIndex = index;
                PresentQuestion(index);
            }
            if (ds.Get(Constants.IS_RESTING_TIME) != null && (bool)ds.Get(Constants.IS_RESTING_TIME) && isInRest && !isGameOver)// if the question is done, we go to rest
            {
                gameHashMap = SetHashMap(ds, gameHashMap);
                isInRest = false;
                //check answers
                CheckAnsAndUpdate(ds);
                gameStatus = HasWon();//if host wins = 1, if player wins = 2, if tie = 0, if not won at all = -1
                tvAnswer.Text = questions[(int)ds.Get(Constants.CURRENT_Q_INDEX)].Ans.ToString();
                if (gameStatus != Constants.NOT_WON)//if the game is over, we show the ending dialog
                {
                    isGameOver = true;
                    ShowGameEndDialog(gameStatus);
                }
                else if(!isGameOver)//else, we keep the game going
                {
                    if (isHost)
                    {
                        isNewQuestion = true;
                        gameHashMap.Put(Constants.IS_NEW_QUESTION, false);
                        gameHashMap.Put(Constants.HOST_ANSWER, 0);
                        gameHashMap.Put(Constants.PLAYER_ANSWER, 0);
                        fd.AddDocumentToCollection(Constants.GAMES_COL, game.GameNum, gameHashMap);
                    }
                    StartTimer(Constants.RESTING_TIME);//resting time after the answer is presented
                    //put the rest that is down there here after game is finished!
                }
                //we reset the answers of both players to 0, so we don't have them set for next round
            }
            if (isHost && ds.Get(Constants.IS_NEW_QUESTION) != null && (bool)ds.Get(Constants.IS_NEW_QUESTION) && isNewQuestion && !isGameOver)//host picks new question when resting is over
            {
                gameHashMap = SetHashMap(ds, gameHashMap);
                isInRest = true;
                isNewQuestion = false;//to not go in this condition many times at once...(bug)
                //reset the views before next question
                tvMyAnswer.Text = 0.ToString();
                tvOpponentAnswer.Text = 0.ToString();
                tvAnswer.Text = "תשובה";
                gameHashMap.Put(Constants.CURRENT_Q_INDEX, RandomNumber());
                gameHashMap.Put(Constants.IS_NEW_QUESTION, false);
                fd.AddDocumentToCollection(Constants.GAMES_COL, game.GameNum, gameHashMap);
                fd.AddSnapShotListenerToDocument(Constants.GAMES_COL, game.GameNum, this);
                PresentQuestion(rand);
            }
        }
        /// <summary>
        /// Starts the game for the game host and for the player, it depends on who started the search before - the game logic depends on it
        /// </summary>
        private void StartGame()
        {
            if (isHost)
            {
                contains = new bool[amountOfQuestions];//to check whether the question was picked already by its index
                gameHashMap.Put(Constants.CURRENT_Q_INDEX, RandomNumber());
                taskPresentQuestion = fd.AddDocumentToCollection(Constants.GAMES_COL, game.GameNum, gameHashMap).AddOnCompleteListener(this);
            }
            else
                fd.AddSnapShotListenerToDocument(Constants.GAMES_COL, game.GameNum, this);
        }
        /// <summary>
        /// Checks the answers of both player (if didn't answer - 0) and updates the textviews and points in firestore accordingly
        /// </summary>
        /// <param name="ds"></param>
        private void CheckAnsAndUpdate(DocumentSnapshot ds)
        {
            int hostAns = ds.Get(Constants.HOST_ANSWER) != null ? (int)ds.Get(Constants.HOST_ANSWER) : 0;
            int playerAns = ds.Get(Constants.PLAYER_ANSWER) != null ? (int)ds.Get(Constants.PLAYER_ANSWER) : 0;
            int result = CheckAnswers(hostAns, playerAns, (int)ds.Get(Constants.CURRENT_Q_INDEX));//if tie = 0, if host wins = 1, if player wins = 2
            //init points
            int hostPoints = ds.Get(Constants.HOST_POINTS) != null ? (int)ds.Get(Constants.HOST_POINTS) : 0;//if there are no points we init to 0
            int playerPoints = ds.Get(Constants.PLAYER_POINTS) != null ? (int)ds.Get(Constants.PLAYER_POINTS) : 0;//if there are no points we init to 0

            switch (result)//if tie = 0, if host wins = 1, if player wins = 2
            {
                case Constants.TIE:
                    hostPoints++;
                    playerPoints++;
                    break;
                case Constants.HOST_WINS:
                    hostPoints++;
                    break;
                case Constants.PLAYER_WINS:
                    playerPoints++;
                    break;
            }

            gameHashMap.Put(Constants.HOST_POINTS, hostPoints);
            gameHashMap.Put(Constants.PLAYER_POINTS, playerPoints);

            fd.UpdateDocument(Constants.GAMES_COL, game.GameNum, Constants.HOST_POINTS, hostPoints);
            fd.UpdateDocument(Constants.GAMES_COL, game.GameNum, Constants.PLAYER_POINTS, playerPoints);

            //put host and player answers + points on both screens
            if (isHost)
            {
                user.Points = hostPoints; opponent.Points = playerPoints;
                //init answers
                tvMyAnswer.Text = ds.Get(Constants.HOST_ANSWER) != null ? ((int)ds.Get(Constants.HOST_ANSWER)).ToString() : "0";
                tvOpponentAnswer.Text = ds.Get(Constants.PLAYER_ANSWER) != null ? ((int)ds.Get(Constants.PLAYER_ANSWER)).ToString() : "0";
                //init points
                tvMyPoints.Text = hostPoints.ToString();
                tvOpponentPoints.Text = playerPoints.ToString();
            }
            else
            {
                user.Points = playerPoints; opponent.Points = hostPoints;
                //init answers
                tvMyAnswer.Text = ds.Get(Constants.PLAYER_ANSWER) != null ? ((int)ds.Get(Constants.PLAYER_ANSWER)).ToString() : "0";
                tvOpponentAnswer.Text = ds.Get(Constants.HOST_ANSWER) != null ? ((int)ds.Get(Constants.HOST_ANSWER)).ToString() : "0";
                //init points
                tvMyPoints.Text = playerPoints.ToString();
                tvOpponentPoints.Text = hostPoints.ToString();
            }
            
        }

        
        /// <summary>
        /// Presents the current question accord to the index, and starts the COUNTDOWN_TIME timer
        /// </summary>
        /// <param name="index">index of current question</param>
        public void PresentQuestion(int index)
        {
            tvQuestion.Text = questions[index].Que;
            StartTimer(Constants.COUNTDOWN_TIME);
        }
        /// <summary>
        /// Starts the timer according to seconds specified, creates the timerhandler and timer and starts it
        /// </summary>
        /// <param name="seconds">seconds to count down from</param>
        public void StartTimer(int seconds)
        {
            timerHandler = new TimerHandler(this, tvCountDown, this);
            timer = new Timer(timerHandler, seconds);
            timer.Start(gameHashMap, isHost, this);
        }
        /// <summary>
        /// puts the random number in "rand" variable and returns it while not repeating the same number twice in one game (using bool array)
        /// </summary>
        /// <returns></returns>
        public int RandomNumber()
        {
            Random rnd = new Random();
            rand = rnd.Next(0, amountOfQuestions);
            while (contains[rand])
            {
                rand = rnd.Next(0, amountOfQuestions);
            }
            contains[rand] = true;
            return rand;
        }
        /// <summary>
        /// Checks current answers from host and player relative to the current index question's answer 
        /// </summary>
        /// <param name="hostNum">host answer</param>
        /// <param name="playerNum">player answer</param>
        /// <param name="currentIndex">current index of the question</param>
        /// <returns>if tie = 0, if host wins = 1, if player wins = 2</returns>
        public int CheckAnswers(int hostNum, int playerNum, int currentIndex)
        {
            if (Math.Abs(hostNum - questions[currentIndex].Ans) > Math.Abs(playerNum - questions[currentIndex].Ans))
                return Constants.PLAYER_WINS;
            else if (Math.Abs(hostNum - questions[currentIndex].Ans) < Math.Abs(playerNum - questions[currentIndex].Ans))
                return Constants.HOST_WINS;
            return Constants.TIE;
        }
        /// <summary>
        /// Checks to see if someone won the game - reached Constants.POINTS_TO_WON
        /// </summary>
        /// <returns>if tie = 0, if host wins = 1, if player wins = 2, if not won at all = -1</returns>
        public int HasWon()
        {
            if (user.Points == Constants.POINTS_TO_WIN && opponent.Points == Constants.POINTS_TO_WIN)
                return Constants.TIE;
            else if (user.Points == Constants.POINTS_TO_WIN)
                if (isHost)
                    return Constants.HOST_WINS;
                else
                    return Constants.PLAYER_WINS;
            else if (opponent.Points == Constants.POINTS_TO_WIN)
                if (isHost)
                    return Constants.PLAYER_WINS;
                else
                    return Constants.HOST_WINS;
            return Constants.NOT_WON;
        }
        /// <summary>
        /// Shows the ending game dialog - we get the gameStatus to see if the player tied, won or lost
        /// gameStatus - if tie = 0, if host wins = 1, if player wins = 2
        /// Shows message in dialog accordingly
        /// </summary>
        /// <param name="gameStatus"></param>
        private void ShowGameEndDialog(int gameStatus)
        {
            SavePlayerResult();
            d = new Dialog(this);
            d.SetContentView(Resource.Layout.gameEnd_Dialog);
            d.SetCancelable(false);
            d.SetTitle("סיכום משחק");
            btnDialogGoHome = d.FindViewById<Button>(Resource.Id.btnDialogGoHome);
            btnDialogGoHome.SetOnClickListener(this);
            tvEndResult = d.FindViewById<TextView>(Resource.Id.tvEndResult);
            tvDialogMyPoints = d.FindViewById<TextView>(Resource.Id.tvDialogMyPoints);
            tvDialogOpponentPoints = d.FindViewById<TextView>(Resource.Id.tvDialogOpponentPoints);
            //init points in dialog - to show the game outcome
            tvDialogMyPoints.Text =  tvDialogMyPoints.Text + user.Points.ToString();
            tvDialogOpponentPoints.Text = tvDialogOpponentPoints.Text + opponent.Points.ToString();
            //show the result of the game, if the player tied / won / lost
            string tie = "תוצאת המשחק היא תיקו!\nבהצלחה בפעם הבאה...";
            string won = "ניצחת את המשחק!\nכל הכבוד!";
            string lost = "הפסדת את המשחק!\nבהצלחה בפעם הבאה...";
            switch (gameStatus)
            {
                case Constants.TIE:
                    tvEndResult.Text = tie;
                    break;
                case Constants.HOST_WINS:
                    if (isHost)
                        tvEndResult.Text = won;
                    else
                        tvEndResult.Text = lost;
                    break;
                case Constants.PLAYER_WINS:
                    if (!isHost)
                        tvEndResult.Text = won;
                    else
                        tvEndResult.Text = lost;
                    break;
            }
            d.Show();
        }
        /// <summary>
        /// Saves player's resulf of the game to this user in firestore
        /// </summary>
        private void SavePlayerResult()
        {
            taskSavePlayerResult = fd.GetDocument(Constants.FS_USERS_COL, user.UserName).AddOnCompleteListener(this);
        }
        /// <summary>
        /// Sets and returns game hashmap according to info in document snapshot recieved 
        /// </summary>
        /// <param name="ds">document of the current game</param>
        /// <param name="hm">the game hashmap</param>
        /// <returns></returns>
        public HashMap SetHashMap(DocumentSnapshot ds, HashMap GameHMap)
        {
            GameHMap.Put(Constants.GAMENUM, (string)ds.Get(Constants.GAMENUM));
            GameHMap.Put(Constants.HOST_GAME, (string)ds.Get(Constants.HOST_GAME));
            GameHMap.Put(Constants.PLAYER_GAME, (string)ds.Get(Constants.PLAYER_GAME));
            GameHMap.Put(Constants.CURRENT_Q_INDEX, (int)ds.Get(Constants.CURRENT_Q_INDEX));
            GameHMap.Put(Constants.ISLIVE_GAME, (bool)ds.Get(Constants.ISLIVE_GAME));
            GameHMap.Put(Constants.IS_RESTING_TIME, (bool)ds.Get(Constants.IS_RESTING_TIME));
            GameHMap.Put(Constants.IS_NEW_QUESTION, (bool)ds.Get(Constants.IS_NEW_QUESTION));
            GameHMap.Put(Constants.HOST_ANSWER, (int)ds.Get(Constants.HOST_ANSWER));
            GameHMap.Put(Constants.PLAYER_ANSWER, (int)ds.Get(Constants.PLAYER_ANSWER));
            GameHMap.Put(Constants.HOST_POINTS, (int)ds.Get(Constants.HOST_POINTS));
            GameHMap.Put(Constants.PLAYER_POINTS, (int)ds.Get(Constants.PLAYER_POINTS));
            return GameHMap;
        }
        /// <summary>
        /// Initializes all objects needed for GameActivity
        /// </summary>
        private void InitObjects()
        {
            fd = new FirebaseData();
            game = new Game();
            sp = new SPData(this);
            user = new User();
            opponent = new User();
            sqlDb = new SqlDb();
            isNewQuestion = true;
            isGameOver = false;
            isInRest = true;
            user.UserName = sp.GetStringData(Constants.USERNAME);
            user.Email = sp.GetStringData(Constants.EMAIL);
            user.Password = sp.GetStringData(Constants.PASSWORD);
            user.ProfilePicture_url = sp.GetStringData(Constants.PROFILE_PIC_URL);
            gameNum = Intent.GetStringExtra(Constants.GAMENUM);
            game.GameNum = gameNum;
            game.Subject = Intent.GetStringExtra(Constants.SUBJECT);
            opponent.UserName = Intent.GetStringExtra(Constants.OPPONENT_NAME);//we get the opponent's name from HomeActivity's intent call
            isHost = Intent.GetBooleanExtra(Constants.ISHOST_GAME, false);
            lastIndex = -1; //so we don't mistakenly check and think this was an index
        }
        /// <summary>
        /// Initializes music for the game (according to if it has been muted)
        /// </summary>
        private void InitMusic()
        {
            bool isMuted = sp.GetBoolData(Constants.IS_MUSIC_MUTED);
            backgroud_music = MediaPlayer.Create(this, Resource.Raw.BackgroundMusic);
            backgroud_music.Looping = true;
            if (!isMuted)
                backgroud_music.Start();
        }
        /// <summary>
        /// Initializes the pictures of the player and opponent and shows them in the imageviews accordingly
        /// </summary>
        private void InitPics()
        {
            taskPullMyPic = fd.GetDocument(Constants.FS_USERS_COL, user.UserName).AddOnCompleteListener(this);
            taskPullOpponentPic = fd.GetDocument(Constants.FS_USERS_COL, opponent.UserName).AddOnCompleteListener(this);
        }
        /// <summary>
        /// Initializes all the views needed for the GameActivity
        /// </summary>
        private void InitViews()
        {
            InitButtons();
            InitTextViews();
            ivMePic = FindViewById<ImageView>(Resource.Id.ivMePic);
            ivOpponentPic = FindViewById<ImageView>(Resource.Id.ivOpponentPic);
            tvTitle.Text = "נושא: " + game.Subject ?? " ";
            tvMe.Text = user.UserName;
            tvOpponent.Text = opponent.UserName;
        }
        /// <summary>
        /// Handles click events, typing answer, deleting answer, sending answer and going back to the home screen (when the end dialog is seen)
        /// </summary>
        /// <param name="v">view clicked</param>
        public void OnClick(View v)
        {
            if (int.Parse(tvMyAnswer.Text) < 10000)//Don't input values over 99999
                NumbersToTextView(v);
            if (v == btnDelete)//Change answer to 0
                tvMyAnswer.Text = 0.ToString();
            else if (v == btnSend)
                taskAnsweredQue = fd.GetDocument(Constants.GAMES_COL, game.GameNum).AddOnCompleteListener(this);
            else if(v == btnDialogGoHome)
            {
                HashMap reset = new HashMap();
                reset.Put(Constants.GAMENUM, game.GameNum);
                reset.Put(Constants.ISLIVE_GAME, false);
                _ = fd.AddDocumentToCollection(Constants.GAMES_COL, game.GameNum, reset);
                backgroud_music.Stop();
                d.Dismiss();
                Intent intent = new Intent(this, typeof(HomeActivity));
                StartActivity(intent);
                Finish();
            }
        }
        /// <summary>
        /// Handles the answer typing process - according to the buttons (0-9)
        /// </summary>
        /// <param name="v"></param>
        private void NumbersToTextView(View v)//Input numbers into the answer like a keyboard
        {
            int answer = int.Parse(tvMyAnswer.Text);
            if (v == btn0)
                answer = int.Parse(tvMyAnswer.Text) * 10;
            else if (v == btn1)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 1;
            else if (v == btn2)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 2;
            else if (v == btn3)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 3;
            else if (v == btn4)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 4;
            else if (v == btn5)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 5;
            else if (v == btn6)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 6;
            else if (v == btn7)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 7;
            else if (v == btn8)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 8;
            else if (v == btn9)
                answer = int.Parse(tvMyAnswer.Text) * 10 + 9;
            tvMyAnswer.Text = answer.ToString();
        }
        /// <summary>
        /// Initializes all the buttons needed for the GameActivity
        /// </summary>
        private void InitButtons()
        {
            btn0 = FindViewById<Button>(Resource.Id.btn0);
            btn1 = FindViewById<Button>(Resource.Id.btn1);
            btn2 = FindViewById<Button>(Resource.Id.btn2);
            btn3 = FindViewById<Button>(Resource.Id.btn3);
            btn4 = FindViewById<Button>(Resource.Id.btn4);
            btn5 = FindViewById<Button>(Resource.Id.btn5);
            btn6 = FindViewById<Button>(Resource.Id.btn6);
            btn7 = FindViewById<Button>(Resource.Id.btn7);
            btn8 = FindViewById<Button>(Resource.Id.btn8);
            btn9 = FindViewById<Button>(Resource.Id.btn9);
            btnSend = FindViewById<Button>(Resource.Id.btnSend);
            btnDelete = FindViewById<Button>(Resource.Id.btnDelete);
            btn0.SetOnClickListener(this);
            btn1.SetOnClickListener(this);
            btn2.SetOnClickListener(this);
            btn3.SetOnClickListener(this);
            btn4.SetOnClickListener(this);
            btn5.SetOnClickListener(this);
            btn6.SetOnClickListener(this);
            btn7.SetOnClickListener(this);
            btn8.SetOnClickListener(this);
            btn9.SetOnClickListener(this);
            btnSend.SetOnClickListener(this);
            btnDelete.SetOnClickListener(this);
        }
        /// <summary>
        /// Initializes all textviews needed for the GameActivity
        /// </summary>
        private void InitTextViews()
        {
            tvQuestion = FindViewById<TextView>(Resource.Id.tvQuestion);
            tvMe = FindViewById<TextView>(Resource.Id.tvMe);
            tvOpponent = FindViewById<TextView>(Resource.Id.tvOpponent);
            tvMyPoints = FindViewById<TextView>(Resource.Id.tvMyPoints);
            tvOpponentPoints = FindViewById<TextView>(Resource.Id.tvOpponentPoints);
            tvMyAnswer = FindViewById<TextView>(Resource.Id.tvMyAnswer);
            tvOpponentAnswer = FindViewById<TextView>(Resource.Id.tvOpponentAnswer);
            tvTitle = FindViewById<TextView>(Resource.Id.tvTitle);
            tvAnswer = FindViewById<TextView>(Resource.Id.tvAnswer);
            tvMyAnswer.Text = "0";
            tvCountDown = FindViewById<TextView>(Resource.Id.tvCountDown);
        }
        }
    }
