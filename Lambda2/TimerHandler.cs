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
    /// Handles timer data
    /// </summary>
    class TimerHandler : Handler
    {
        Context context;
        TextView tvCountDown;
        GameActivity gameActivity;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="tvCountDown">text view that changes</param>
        /// <param name="gameActivity"></param>
        public TimerHandler(Context context, TextView tvCountDown, GameActivity gameActivity)
        {
            this.context = context;
            this.tvCountDown = tvCountDown;
            this.gameActivity = gameActivity;
        }
        /// <summary>
        /// Handles messages every second (each call)
        /// </summary>
        /// <param name="msg"></param>
        public override void HandleMessage(Message msg)
        {
            gameActivity.RunOnUiThread(() =>
            {
                tvCountDown.Text = "" + msg.Arg1;
            });
        }

    }
}