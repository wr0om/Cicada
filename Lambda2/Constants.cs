using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lambda2
{
    /// <summary>
    /// constants for the whole project
    /// </summary>
    static class Constants
    {
        public const int DEFAULT_COST_VALUE = 0;
        public const string USERNAME = "Username";
        public const string SUBJECT = "Subject";
        public const string GENERAL_KNOWLEDGE = "ידע כללי";
        public const string SP_DATA_FILENAME = "sp.data";
        public const string SQL_DATA_FILENAME = "queAndAns.db";
        public const string EMAIL = "Email";
        public const string PASSWORD = "Password";
        public const string PROFILE_PIC_URL = "ProfilePicURL";
        public const string FS_USERS_COL = "users";
        public const string GAMES_COL = "games";
        public const string QUESTIONS_COL = "questions";
        public const string QUESTIONS_ARR = "questions";
        public const string ANSWERS_COL = "answers";

        public const string ANSWERS_ARR = "answers";
        //for the player's data
        public const string TIE_NUM = "TieNum";
        public const string WIN_NUM = "WinNum";
        public const string LOSS_NUM = "LossNum";

        public const int NUMBER_OF_QUESTIONS = 100;//number of questions I put in the sql database.

        public const string HOST_ANSWER = "host_Answer";
        public const string PLAYER_ANSWER = "player_Answer";
        public const string HOST_POINTS= "host_Points";
        public const string PLAYER_POINTS = "player_Points";
        public const int POINTS_TO_WIN = 5;
        public const string IS_RESTING_TIME = "isRestingTime";
        public const string IS_NEW_QUESTION = "isNewQuestion";

        public const string ISLIVE_GAME = "isLive";
        public const string ISHOST_GAME = "isHost";
        public const string GAMENUM = "gameNum";
        public const string CURRENT_Q_INDEX = "currentQuestion";
        public const string HOST_GAME = "host";
        public const string PLAYER_GAME = "player";
        public const int COUNTDOWN_TIME = 10;
        public const int RESTING_TIME = 5;
        public const string IS_MUSIC_MUTED = "IsMuted";
        public const string OPPONENT_NAME = "opponent name";
        public const string FS_IMAGES = "Images/";
        public const string KEY_CAMERA_IMAGE = "data";
        public const int DOWNSAMPLE_SIZE = 70;
        public const int REQUEST_OPEN_CAMERA = 1;
        public const int REQUEST_OPEN_GALLERY = 2;

    }
}