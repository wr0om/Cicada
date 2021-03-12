using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lambda2
{
    /// <summary>
    /// Handles all the sharedpreferences in the project
    /// </summary>
    public class SPData : Data
    {
        private ISharedPreferences sp;
        private Context context;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ctx">context of activity where we use the sharedpreferences</param>
        public SPData(Context ctx): base(Constants.SP_DATA_FILENAME)//check what about the path is creating the exception
        {
            this.context = ctx;
            sp = ctx.GetSharedPreferences(Constants.SP_DATA_FILENAME, FileCreationMode.Private);
        }
        /// <summary>
        /// returns string with key
        /// </summary>
        /// <param name="keyName">key</param>
        /// <returns></returns>
        public string GetStringData(string keyName)
        {
            return sp.GetString(keyName, string.Empty);
        }
        /// <summary>
        /// returns bool with key
        /// </summary>
        /// <param name="keyName">key</param>
        /// <returns></returns>
        public bool GetBoolData(string keyName)
        {
            return sp.GetBoolean(keyName, false);
        }
        /// <summary>
        /// sets string data in sharedpreferences with key
        /// </summary>
        /// <param name="keyName">key</param>
        /// <param name="value">value</param>
        public void SetData(string keyName, string value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutString(keyName, value);
            editor.Commit();
        }
        /// <summary>
        /// sets int data in sharedpreferences with key
        /// </summary>
        /// <param name="keyName">key</param>
        /// <param name="value">value</param>
        public void SetData(string keyName, int value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt(keyName, value);
            editor.Commit();
        }
        /// <summary>
        /// sets bool data in sharedpreferences with key
        /// </summary>
        /// <param name="keyName">key</param>
        /// <param name="value">value</param>
        public void SetData(string keyName, bool value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutBoolean(keyName, value);
            editor.Commit();
        }
        /// <summary>
        /// Deletes all sharedpreferences data from app
        /// </summary>
        public void DeleteSPData()
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.Clear();
            editor.Commit();
        }
    }
}