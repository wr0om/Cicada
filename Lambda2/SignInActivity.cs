using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Views;
using Android.Gms.Tasks;
using Android.Content;

namespace Lambda2
{
    /// <summary>
    /// sign in screen, manages signing in to the app.
    /// </summary>
    [Activity(Label = "SignInActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SignInActivity : Activity, Android.Views.View.IOnClickListener, IOnCompleteListener
    {
        Button btnSignIn;
        EditText etEmail, etPassword;
        FirebaseData fd;
        Task taskSignIn;
        SPData sp;
        User user;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.sign_in);
            InitObjects();
            InitViews();
        }
        /// <summary>
        /// Initialized the objects of the activity
        /// </summary>
        private void InitObjects()
        {
            fd = new FirebaseData();
            sp = new SPData(this);
            user = new User();
        }
        /// <summary>
        /// Initializes the views of the activity
        /// </summary>
        private void InitViews()
        {
            btnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            etEmail = FindViewById<EditText>(Resource.Id.etEmail);
            etPassword = FindViewById<EditText>(Resource.Id.etPassword);
            btnSignIn.SetOnClickListener(this);
        }
        /// <summary>
        /// handles click event
        /// </summary>
        /// <param name="v">view with listener clicked</param>
        public void OnClick(View v)
        {
            if (v == btnSignIn && etEmail.Text != string.Empty && etPassword.Text != string.Empty)
            {
                taskSignIn = fd.SignIn(etEmail.Text, etPassword.Text);
                taskSignIn.AddOnCompleteListener(this);
            }
        }
        /// <summary>
        /// Signs in to the app if correct credentials, if not - toast exception message.
        /// </summary>
        /// <param name="task"></param>
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == taskSignIn)
                {
                    user.Email = etEmail.Text;
                    user.Password = etPassword.Text;
                    user.UserName = user.Email.Split('@')[0];
                    sp.SetData(Constants.EMAIL, user.Email);
                    sp.SetData(Constants.PASSWORD, user.Password);
                    sp.SetData(Constants.USERNAME, user.UserName);
                    Toast.MakeText(this, "User " + user.UserName + " signed in", ToastLength.Long).Show();
                    Intent intent = new Intent(this, typeof(HomeActivity));
                    StartActivity(intent);
                    Finish();
                }
            }
            else
                Toast.MakeText(this, task.Exception.Message, ToastLength.Long).Show();
        }
    }
}