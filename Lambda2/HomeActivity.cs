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
using Firebase.Storage;
using FFImageLoading;
using System.IO;

namespace Lambda2
{
    /// <summary>
    /// Home screen, allows to go to MeActivity 
    /// </summary>
    [Activity(Label = "מסך הבית", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HomeActivity : AppCompatActivity, Android.Views.View.IOnClickListener, IOnCompleteListener, Firebase.Firestore.IEventListener
    {
        Button btnMe, btnGenKnow;
        TextView tvWins, tvWinPercentage;
        Dialog d;
        CheckBox cbMuteMusic;
        Button btnSearch;
        ImageView ivMePic;
        FirebaseData fd;
        SPData sp;
        User user;
        Game game;
        Task taskEqualCollection, taskFindGame;
        [Obsolete]
        ProgressDialog pd;
        bool isHost, isInGame;
        SqlDb db;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_home);
            InitObjects();
            InitViews();
            InitPic();
            if (db.IsNewData)//if we created the sql database already, we don't add to it again
                db.InitQuestionsDB();
        }
        /// <summary>
        /// Initialize stats of player - number of wins and win percentage
        /// </summary>
        private void InitStats()
        {
            tvWins.Text = tvWins.Text + user.WinNum.ToString();
            tvWinPercentage.Text = tvWinPercentage.Text + user.WinPrecentage() + "%";
        }
        /// <summary>
        /// Creates the menu and inflates it
        /// </summary>
        /// <param name="menu">the menu created</param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            for (int i = 0; i < menu.Size(); i++)
            {
                Android.Views.IMenuItem item = menu.GetItem(i);
                item.SetShowAsAction(Android.Views.ShowAsAction.Always);
            }
            return base.OnCreateOptionsMenu(menu);
        }
        /// <summary>
        /// Allows to choose the items in the menu
        /// </summary>
        /// <param name="item">the menu</param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_settings)
            {
                CreateSettingsDialog();
            }
            return base.OnOptionsItemSelected(item);
        }
        /// <summary>
        /// Initialize profile picture
        /// </summary>
        public void InitPic()
        {
            taskEqualCollection = fd.GetDocument(Constants.FS_USERS_COL, user.UserName).AddOnCompleteListener(this);
        }
        /// <summary>
        /// Initialize objects
        /// </summary>
        private void InitObjects()
        {
            isInGame = false;
            db = new SqlDb();
            fd = new FirebaseData();
            sp = new SPData(this);
            user = new User();
            game = new Game();
            if(sp.GetStringData(Constants.EMAIL)!=null)
                user.Email = sp.GetStringData(Constants.EMAIL);
            if (sp.GetStringData(Constants.USERNAME) != null)
                user.UserName = sp.GetStringData(Constants.USERNAME);
            if (sp.GetStringData(Constants.PASSWORD) != null)
                user.Password = sp.GetStringData(Constants.PASSWORD);
            if (sp.GetStringData(Constants.PROFILE_PIC_URL) != null)
                user.ProfilePicture_url = sp.GetStringData(Constants.PROFILE_PIC_URL);
        }
        /// <summary>
        /// Initialize views of home activity
        /// </summary>
        private void InitViews()
        {
            ivMePic = FindViewById<ImageView>(Resource.Id.ivMePic);
            tvWins = FindViewById<TextView>(Resource.Id.tvWins);
            tvWinPercentage = FindViewById<TextView>(Resource.Id.tvWinPercentage);
            btnMe = FindViewById<Button>(Resource.Id.btnMe);
            btnGenKnow = FindViewById<Button>(Resource.Id.btnGenKnow);
            btnMe.SetOnClickListener(this);
            btnGenKnow.SetOnClickListener(this);
            btnMe.Text = user.UserName;
        }
        /// <summary>
        /// Event that is called from firestore, responsible for setting a new user's data in firestore, loading profile picture and setting user data in sharedpreferences.
        /// Also, responsible for the "matchmaking" process when attempting to join a game.
        /// </summary>
        /// <param name="task"></param>
        [Obsolete]
        public void OnComplete(Task task)
        {
            if (task == taskEqualCollection)
            {
                if (task.IsSuccessful)//if we did pull an image url, we put it in the imageview
                {
                    DocumentSnapshot ds = (DocumentSnapshot)task.Result;
                    //if the user set a profile picture manually already, we add its url to the user's information and input the picture to the imageview
                    if (ds.Exists() && ds.Get(Constants.PROFILE_PIC_URL) != null)
                    {
                        user.SetUserData(ds);
                        user.TieNum = ds.Get(Constants.TIE_NUM) != null ? (int)ds.Get(Constants.TIE_NUM) : 0;
                        user.WinNum = ds.Get(Constants.WIN_NUM) != null ? (int)ds.Get(Constants.WIN_NUM) : 0;
                        user.LossNum = ds.Get(Constants.LOSS_NUM) != null ? (int)ds.Get(Constants.LOSS_NUM) : 0;
                        InitStats();
                        user.ProfilePicture_url = (string)ds.Get(Constants.PROFILE_PIC_URL);
                        sp.SetData(Constants.PROFILE_PIC_URL, user.ProfilePicture_url);
                        sp.SetData(Constants.USERNAME, (string)ds.Get(Constants.USERNAME));
                        sp.SetData(Constants.EMAIL, (string)ds.Get(Constants.EMAIL));
                        sp.SetData(Constants.PASSWORD, (string)ds.Get(Constants.PASSWORD));

                        if(ds.Get(Constants.TIE_NUM) != null)
                            sp.SetData(Constants.TIE_NUM, (int)ds.Get(Constants.TIE_NUM));
                        if (ds.Get(Constants.WIN_NUM) != null)
                            sp.SetData(Constants.WIN_NUM, (int)ds.Get(Constants.WIN_NUM));
                        if (ds.Get(Constants.LOSS_NUM) != null)
                            sp.SetData(Constants.LOSS_NUM, (int)ds.Get(Constants.LOSS_NUM));
                        ImageService.Instance.LoadUrl(user.ProfilePicture_url).Retry(3, 200).FadeAnimation(true).DownSample(Constants.DOWNSAMPLE_SIZE, Constants.DOWNSAMPLE_SIZE).Into(ivMePic);
                    }
                    else//incase its a new user
                    {
                        HashMap hm = user.SetUserData();
                        fd.AddDocumentToCollection(Constants.FS_USERS_COL, user.UserName, hm); //Init user's information to firebase, document name is user's username
                    }
                }
            }
            if (task == taskFindGame && task.IsSuccessful)
            {
                QuerySnapshot qs = (QuerySnapshot)task.Result;
                string gameNum = "";
                bool doStart = false;
                string opponentName = string.Empty;
                foreach (DocumentSnapshot ds in qs.Documents)//we check to see if someone is looking to start a game (if not we start a game)
                {
                    bool isLive = (bool)ds.Get(Constants.ISLIVE_GAME);
                    if (!isLive)//we found a game that's not started yet (1 or less players connected)
                    {
                        gameNum = (string)ds.Get(Constants.GAMENUM);
                        HashMap hm = new HashMap();
                        if (ds.Get(Constants.HOST_GAME) != null)//check if there is already a player in the game, if yes we join him
                        {
                            opponentName = (string)ds.Get(Constants.HOST_GAME);
                            hm.Put(Constants.GAMENUM, gameNum);
                            hm.Put(Constants.HOST_GAME, opponentName);
                            hm.Put(Constants.PLAYER_GAME, user.UserName);
                            hm.Put(Constants.ISLIVE_GAME, true);//we put true to say a game is running
                            fd.AddDocumentToCollection(Constants.GAMES_COL, gameNum, hm);//put the information according to the game number
                            doStart = true;
                            isHost = false;
                            break;
                        }
                    }
                }
                if (doStart)
                    StartGame(gameNum, opponentName, game.Subject);
                else
                {
                    foreach (DocumentSnapshot ds in qs.Documents)//we know a game hasn't started yet because we looped all the available games
                    {
                        bool isLive = (bool)ds.Get(Constants.ISLIVE_GAME);
                        if (!isLive)
                        {
                            gameNum = (string)ds.Get(Constants.GAMENUM);
                            HashMap hm = new HashMap();
                            hm.Put(Constants.GAMENUM, gameNum);
                            hm.Put(Constants.ISLIVE_GAME, false);
                            hm.Put(Constants.HOST_GAME, user.UserName);
                            fd.AddDocumentToCollection(Constants.GAMES_COL, gameNum, hm);//put the information according to the game number
                            fd.AddSnapShotListenerToDocument(Constants.GAMES_COL, gameNum, this);//add event listener on current game
                            //put loading screen until 2nd joins game
                            ShowProgressDlg();
                            isHost = true;
                            break;//stop checking for other games
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates and shows the progress dialog
        /// </summary>
        [Obsolete]
        private void ShowProgressDlg()
        {
            pd = new ProgressDialog(this);
            pd.SetTitle("אנא המתן");
            pd.SetMessage("מחפש אחר יריב...");
            pd.SetCancelable(false);
            pd.SetProgressStyle(ProgressDialogStyle.Spinner);
            pd.Show();
        }
        /// <summary>
        /// Gets called when a value is changed in the current game we are looking for, "looking for an opponent"
        /// </summary>
        /// <param name="value">The info from firestore</param>
        /// <param name="error">exception</param>
        [Obsolete]
        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            DocumentSnapshot ds = (DocumentSnapshot)value;
            string opponentName = string.Empty;
            if (isInGame)
                Finish();
            else if (ds != null)
            {
                if ((bool)ds.Get(Constants.ISLIVE_GAME))
                {
                    //stop loading and begin game
                    pd.Dismiss();
                    opponentName = (string)ds.Get(Constants.PLAYER_GAME);
                    StartGame((string)ds.Get(Constants.GAMENUM), opponentName, game.Subject);
                }
            }
        }
        /// <summary>
        /// Puts all relavent values in intent and starts game - goes to GameActivity
        /// </summary>
        /// <param name="gameNum">The current game "server" in firestore that is being played</param>
        /// <param name="opponentName">The name of the opponent</param>
        /// <param name="gameSubject">The subject of the game</param>
        private void StartGame(string gameNum, string opponentName, string gameSubject)
        {
            Intent intent = new Intent(this, typeof(GameActivity));
            intent.PutExtra(Constants.OPPONENT_NAME, opponentName);//so we know the opponent's name in the GameActivity
            intent.PutExtra(Constants.GAMENUM, gameNum); //so we know the gameNum while playing the game
            intent.PutExtra(Constants.SUBJECT, gameSubject);
            intent.PutExtra(Constants.ISHOST_GAME, isHost);
            isInGame = true;
            StartActivity(intent);
        }
        /// <summary>
        /// Handles click events - going to MeActivity, Muting music in dialog, and searching for a game
        /// </summary>
        /// <param name="v"></param>
        public void OnClick(View v)
        {
            if (v == btnMe)
            {
                Intent intent = new Intent(this, typeof(MeActivity));
                StartActivity(intent);
                Finish();
            }
            if (v == btnGenKnow)
            {
                CreateStartGameDialog();
                game.Subject = Constants.GENERAL_KNOWLEDGE;
            }
            if (v == btnSearch)
            {
                d.Dismiss();
                taskFindGame = fd.GetCollection(Constants.GAMES_COL).AddOnCompleteListener(this);//search for game    
            }
            if(v == cbMuteMusic)
            {
                sp.SetData(Constants.IS_MUSIC_MUTED, cbMuteMusic.Checked);
            }
        }
        /// <summary>
        /// Creates and show settings dialog - where we can mute the background music of the game
        /// </summary>
        private void CreateSettingsDialog()
        {
            d = new Dialog(this);
            d.SetContentView(Resource.Layout.Settings_Dialog);
            d.SetCancelable(true);
            d.SetTitle("הגדרות");
            cbMuteMusic = d.FindViewById<CheckBox>(Resource.Id.cbMuteMusic);
            cbMuteMusic.SetOnClickListener(this);
            SetSavedSettings();
            d.Show();
        }
        /// <summary>
        /// "remembers" if we muted the music already and gets that from sharedpreferences
        /// </summary>
        private void SetSavedSettings()
        {
            //take from sp
            bool isMusicMuted = sp.GetBoolData(Constants.IS_MUSIC_MUTED);
            cbMuteMusic.Checked = isMusicMuted;
        }
        /// <summary>
        /// Creates and starts the start game dialog, if we press the start game button we start looking for an opponent
        /// </summary>
        private void CreateStartGameDialog()
        {
            d = new Dialog(this);
            d.SetContentView(Resource.Layout.StartGame_Dialog);
            d.SetCancelable(true);
            d.SetTitle("חיפוש");
            btnSearch = d.FindViewById<Button>(Resource.Id.btnSearch);
            btnSearch.SetOnClickListener(this);
            d.Show();
        }
    }
}