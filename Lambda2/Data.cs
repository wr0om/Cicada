using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lambda2
{
    /// <summary>
    /// responsible for sql and sharedpreference data management
    /// </summary>
    public abstract class Data
    {

        protected string DbPath { get; }
        public bool IsNewData { get; }

        /// <summary>
        /// gets the file name and puts the path in DbPath, also changes the value of IsNewData according to if the db exists
        /// </summary>
        /// <param name="fileName"> the file's name that is going to be saved</param>
        public Data(string fileName)
        {
            DbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            this.IsNewData = !File.Exists(DbPath);
        }

    }
}