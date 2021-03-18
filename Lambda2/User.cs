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
using Android.Graphics;
using Java.Util;
using System.Threading.Tasks;
using System.IO;
using Android.Util;
using Firebase.Firestore;

namespace Lambda2
{
    /// <summary>
    /// User managment in the app
    /// </summary>
    public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfilePicture_url { get; set; }
        public int Points { get; set; }
        public int TieNum { get; set; }
        public int WinNum { get; set; }
        public int LossNum { get; set; }
        /// <summary>
        /// empty constructor
        /// </summary>
        public User()
        {
            
        }
        /// <summary>
        /// returns the total number of games
        /// </summary>
        /// <returns></returns>
        public int TotalGamesNum() { return TieNum + WinNum + LossNum; }//returns the total number of games played
        /// <summary>
        /// returns the win precentage
        /// </summary>
        /// <returns></returns>
        public string WinPrecentage() 
        { 
            double winP = TotalGamesNum() != 0 ? ((double)WinNum / TotalGamesNum() * 100) : 0;
            string winPercent = String.Format("{0:0.00}", winP);
            return winPercent;
        }//returns the win precentage relative to the total games played
        /// <summary>
        /// Sets user values in hashmap
        /// </summary>
        /// <returns></returns>
        public HashMap SetUserData()
        {
            HashMap hm = new HashMap();
            hm.Put(Constants.EMAIL, Email);
            hm.Put(Constants.USERNAME, UserName);
            hm.Put(Constants.PASSWORD, Password);
            hm.Put(Constants.PROFILE_PIC_URL, ProfilePicture_url);
            hm.Put(Constants.TIE_NUM, TieNum);
            hm.Put(Constants.WIN_NUM, WinNum);
            hm.Put(Constants.LOSS_NUM, LossNum);
            return hm;
        }
        /// <summary>
        /// Sets user data according to document values
        /// </summary>
        /// <param name="ds"></param>
        public void SetUserData(DocumentSnapshot ds)
        {
            UserName = (string)ds.Get(Constants.USERNAME);
            Email = (string)ds.Get(Constants.EMAIL);
            Password = (string)ds.Get(Constants.PASSWORD);
            if (ds.Get(Constants.PROFILE_PIC_URL) != null)
            ProfilePicture_url = (string)ds.Get(Constants.PROFILE_PIC_URL);
            if (ds.Get(Constants.TIE_NUM) != null)
            TieNum = (int)ds.Get(Constants.TIE_NUM);
            if (ds.Get(Constants.WIN_NUM) != null)
            WinNum = (int)ds.Get(Constants.WIN_NUM);
            if (ds.Get(Constants.LOSS_NUM) != null)
            LossNum = (int)ds.Get(Constants.LOSS_NUM);
        }

    }
}