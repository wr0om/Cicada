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
using SQLite;

namespace Lambda2
{
    /// <summary>
    /// question class for the sqlite database
    /// </summary>
    [Table("Question")]
    class Question
    {
        [PrimaryKey, AutoIncrement]
        public int Index { get; set; }
        public string Que{get; set;}
        public int Ans{ get; set; }
        /// <summary>
        /// empty constructor - needed for sql
        /// </summary>
        public Question() { }
        /// <summary>
        /// constructor that puts the question and answer in the object
        /// </summary>
        /// <param name="question">question</param>
        /// <param name="answer">answer</param>
        public Question(string question, int answer) 
        {
            Que = question;
            Ans = answer;
        }

    }
}