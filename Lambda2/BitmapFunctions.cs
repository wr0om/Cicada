using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
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
    /// Deals with bitmap, helps compress it
    /// </summary>
    class BitmapFunctions
    {
        /// <summary>
        /// compresses bitmap to byte array and returns it
        /// </summary>
        /// <param name="bitmap">bitmap for picture</param>
        /// <returns>byte array</returns>
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
            return ms.ToArray();
        }
    }
}