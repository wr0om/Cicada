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
using Android.Provider;
using System.IO;

namespace Lambda2

{
    /// <summary>
    /// Me Activity - show the user's information and allows to change profile picture
    /// </summary>
    [Activity(Label = "", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MeActivity : Activity, View.IOnClickListener, IOnCompleteListener
    {
        Button btnBack, btnCaptureImage, btnUploadImage, btnLogOut;
        TextView tvGames, tvWins, tvLosses,tvTies, tvUsername;
        ImageView ivProfilePic;
        SPData sp;
        User user;
        FirebaseData fd;
        Task taskCaptureImage,taskUploadImage, taskEqualCollection, taskInitStats;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_me);
            InitObjects();
            InitViews();
            InitPic();
        }
        /// <summary>
        /// Initializes player profile picture to imageview
        /// </summary>
        private void InitPic()
        {
            taskEqualCollection = fd.GetDocument(Constants.FS_USERS_COL, user.UserName).AddOnCompleteListener(this);
        }
        /// <summary>
        /// Initializes objects for this activity
        /// </summary>
        private void InitObjects()
        {
            fd = new FirebaseData();
            sp = new SPData(this);
            user = new User();
            user.Email = sp.GetStringData(Constants.EMAIL);
            user.UserName = sp.GetStringData(Constants.USERNAME);
            user.Password = sp.GetStringData(Constants.PASSWORD);
        }
        /// <summary>
        /// Initializes views of this activity
        /// </summary>
        private void InitViews()
        {
            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnCaptureImage = FindViewById<Button>(Resource.Id.btnCaptureImage);
            btnUploadImage = FindViewById<Button>(Resource.Id.btnUploadImage);
            btnLogOut = FindViewById<Button>(Resource.Id.btnLogOut);
            btnBack.SetOnClickListener(this);
            btnCaptureImage.SetOnClickListener(this);
            btnUploadImage.SetOnClickListener(this);
            btnLogOut.SetOnClickListener(this);
            tvGames = FindViewById<TextView>(Resource.Id.tvGames);
            tvWins = FindViewById<TextView>(Resource.Id.tvWins);
            tvLosses = FindViewById<TextView>(Resource.Id.tvLosses);
            tvTies = FindViewById<TextView>(Resource.Id.tvTies);
            tvUsername = FindViewById<TextView>(Resource.Id.tvUsername);
            ivProfilePic = FindViewById<ImageView>(Resource.Id.ivProfilePic);
            tvUsername.Text = user.UserName;
            taskInitStats = fd.GetDocument(Constants.FS_USERS_COL, user.UserName).AddOnCompleteListener(this);
        }
        /// <summary>
        /// handles click event
        /// </summary>
        /// <param name="v">view that is clicked</param>
        public void OnClick(View v)
        {
            if (v == btnBack)
            {
                Intent intent = new Intent(this, typeof(HomeActivity));
                StartActivity(intent);
                Finish();
            }
            if (v == btnCaptureImage)
            {
                OpenCamera();
            }
            if(v == btnUploadImage)//implement at this end
            {
                OpenGallery();
            }
            if(v == btnLogOut)
            {
                sp.DeleteSPData();
                Finish();
                Intent intent = new Intent(this, typeof(SignUpActivity));
                StartActivity(intent);
                Finish();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestCode">to show where we came from</param>
        /// <param name="resultCode">to show if successful</param>
        /// <param name="data">the data from the screen we got it from</param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constants.REQUEST_OPEN_CAMERA)
            {
                if (resultCode == Result.Ok && data != null)
                {
                    Bitmap bitmap = (Android.Graphics.Bitmap)data.Extras.Get(Constants.KEY_CAMERA_IMAGE);
                    taskCaptureImage = fd.SaveImage(bitmap, user.UserName + ".png").AddOnCompleteListener(this);
                }
            }
            if (requestCode == Constants.REQUEST_OPEN_GALLERY)
            {
                if (resultCode == Result.Ok && data != null)
                {
                    Stream stream = ContentResolver.OpenInputStream(data.Data);
                    Bitmap bitmap = BitmapFactory.DecodeStream(stream);
                    taskUploadImage = fd.SaveImage(bitmap, user.UserName + ".png").AddOnCompleteListener(this);
                }
            }
        }
        /// <summary>
        /// Opens gallery screen, to select photos
        /// </summary>
        private void OpenGallery()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(intent, Constants.REQUEST_OPEN_GALLERY);
        }
        /// <summary>
        /// opens camera screen, to take photo
        /// </summary>
        private void OpenCamera()
        {
            Intent intent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
            StartActivityForResult(intent, Constants.REQUEST_OPEN_CAMERA);
        }
        /// <summary>
        /// handles on complete events, capture image, upload image, init stats and get profile picture
        /// </summary>
        /// <param name="task">The task firestore returns</param>
        public void OnComplete(Task task)
        {
            if(task == taskCaptureImage || task == taskUploadImage)
            {
                if (task.IsSuccessful)
                {
                    UploadTask.TaskSnapshot taskSnapshot = (UploadTask.TaskSnapshot)task.Result;
                    user.ProfilePicture_url = taskSnapshot.DownloadUrl.ToString();
                    sp.SetData(Constants.PROFILE_PIC_URL, user.ProfilePicture_url);
                    fd.UpdateDocument(Constants.FS_USERS_COL, user.UserName, Constants.PROFILE_PIC_URL, user.ProfilePicture_url);
                    ImageService.Instance.LoadUrl(user.ProfilePicture_url).Retry(3, 200).FadeAnimation(true).DownSample(Constants.DOWNSAMPLE_SIZE * 2, Constants.DOWNSAMPLE_SIZE * 2).Into(ivProfilePic);
                }
            }
            if(task == taskEqualCollection)
            {
                DocumentSnapshot ds = (DocumentSnapshot)task.Result;
                if (task.IsSuccessful && ds.Get(Constants.PROFILE_PIC_URL) != null)
                {
                    //if the user set a profile picture manually already, we add its url to the user's information in sp and input the picture to the imageview
                    user.ProfilePicture_url = (string)ds.Get(Constants.PROFILE_PIC_URL);
                    sp.SetData(Constants.PROFILE_PIC_URL, user.ProfilePicture_url);
                    ImageService.Instance.LoadUrl(user.ProfilePicture_url).Retry(3, 200).FadeAnimation(true).DownSample(Constants.DOWNSAMPLE_SIZE * 2, Constants.DOWNSAMPLE_SIZE * 2).Into(ivProfilePic);
                }
            }if(task.IsSuccessful && task == taskInitStats)
            {
                DocumentSnapshot ds = (DocumentSnapshot)task.Result;
                user.TieNum = ds.Get(Constants.TIE_NUM) != null ? (int)ds.Get(Constants.TIE_NUM) : 0;
                user.WinNum = ds.Get(Constants.WIN_NUM) != null ? (int)ds.Get(Constants.WIN_NUM) : 0;
                user.LossNum = ds.Get(Constants.LOSS_NUM) != null ? (int)ds.Get(Constants.LOSS_NUM) : 0;
                tvGames.Text = user.TotalGamesNum().ToString();
                tvTies.Text = user.TieNum.ToString();
                tvWins.Text = user.WinNum.ToString();
                tvLosses.Text = user.LossNum.ToString();
            }
        }
    }
}
