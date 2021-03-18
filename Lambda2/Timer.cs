using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using Java.Util;
using System;
using System.Collections;
using System.Threading;

namespace Lambda2
{
    /// <summary>
    /// timer class, responsible for the countdown for the game.
    /// </summary>
    public class Timer
    {
        public int c { get; set; }
        Handler handler;
        FirebaseData fd;
        public HashMap GameHM { get; set; }
        bool isHost, isRestingTime;
        GameActivity gameActivity;
        ThreadStart threadStart;
        Thread t;
        /// <summary>
        /// Constructor, initializes the hanlder and c
        /// </summary>
        /// <param name="handler">timerhandler</param>
        /// <param name="c">seconds</param>
        public Timer(Handler handler, int c)
        {
            this.handler = handler;
            this.c = c;
            fd = new FirebaseData();
        }
        /// <summary>
        /// Starts the timer
        /// </summary>
        /// <param name="gameHM">game hashmap</param>
        /// <param name="isHost">bool value to check if the player is the host or not</param>
        /// <param name="gameActivity">GameActivity</param>
        public void Start(HashMap gameHM, bool isHost, GameActivity gameActivity)
        {
            this.isHost = isHost;
            this.GameHM = gameHM;
            this.gameActivity = gameActivity;

            if (c == Constants.RESTING_TIME)
                isRestingTime = true;
            else
                isRestingTime = false;
            threadStart = new ThreadStart(Run);
            t = new Thread(threadStart);
            t.Start();
        }
        /// <summary>
        /// function that is called when the thread starts - responsible to communicate with firestore to let it know when the timer stops and reaches 0
        /// </summary>
        void Run()
        {
            while (c > 0)
            {
                Message message = new Message();
                message.Arg1 = c;
                handler.HandleMessage(message);
                Thread.Sleep(1000);
                c--;
            }
            if (isHost)
            {
                if (!isRestingTime)
                {
                    GameHM.Put(Constants.IS_RESTING_TIME, true);
                    GameHM.Put(Constants.IS_NEW_QUESTION, false);
                    fd.UpdateDocument(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(), Constants.IS_RESTING_TIME, true);
                    fd.UpdateDocument(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(), Constants.IS_NEW_QUESTION, false);

                   // fd.AddDocumentToCollection(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(),);
                    fd.AddSnapShotListenerToDocument(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(), gameActivity);
                }
                else
                {
                    GameHM.Put(Constants.IS_RESTING_TIME, false);
                    GameHM.Put(Constants.IS_NEW_QUESTION, true);//so we have an indication of when we need to set a new question
                    fd.UpdateDocument(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(), Constants.IS_RESTING_TIME, false);
                    fd.UpdateDocument(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(), Constants.IS_NEW_QUESTION, true);

                    //fd.AddDocumentToCollection(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(), GameHM);
                    fd.AddSnapShotListenerToDocument(Constants.GAMES_COL, GameHM.Get(Constants.GAMENUM).ToString(), gameActivity);
                }
            }
        }
    }
}
