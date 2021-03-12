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
    /// The activity where you are supposed to sign up to the app
    /// </summary>
    [Activity(Label = "Cicada", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme", MainLauncher = true)]
    public class SignUpActivity : AppCompatActivity, Android.Views.View.IOnClickListener, IOnCompleteListener
    {
        Button btnSignUp, btnSignIn_Page;
        EditText etEmail, etPassword;
        FirebaseData fd;
        Task taskSignUp, taskSignIn;
        User user;
        SPData sp;
        bool HasSignedIn;
        /// <summary>
        /// the function that is called at the start of the app, calls 
        /// </summary>
        /// <param name="savedInstanceState">built in</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.sign_up);
            InitObjects();
            InitViews();
        }
        /// <summary>
        /// Initalizes all the objects needed for the activity
        /// </summary>
        private void InitObjects()
        {
            fd = new FirebaseData();
            sp = new SPData(this);
            user = new User
            {
                Email = sp.GetStringData(Constants.EMAIL),
                Password = sp.GetStringData(Constants.PASSWORD),
            };
            HasSignedIn = (user.Email != string.Empty) && (user.Password != string.Empty);
            if (HasSignedIn)
            {
                user.UserName = user.Email.Split('@')[0];
                taskSignIn = fd.SignIn(user.Email, user.Password);
                taskSignIn.AddOnCompleteListener(this);
            }
        }
        /// <summary>
        /// Initializes all the views needed for the activity
        /// </summary>
        private void InitViews()
        {
            btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            btnSignIn_Page = FindViewById<Button>(Resource.Id.btnSignIn_Page);
            etEmail = FindViewById<EditText>(Resource.Id.etEmail);
            etPassword = FindViewById<EditText>(Resource.Id.etPassword);
            btnSignUp.SetOnClickListener(this);
            btnSignIn_Page.SetOnClickListener(this);
        }
        /// <summary>
        /// built in function
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        /// <summary>
        /// Gets called when we press a button, responsible for communicating with pages
        /// </summary>
        /// <param name="v">the button\view that was pressed</param>
        public void OnClick(View v)
        {
            if (v == btnSignUp && etEmail.Text != "" && etPassword.Text != "")
            {
                taskSignUp = fd.CreateUser(etEmail.Text, etPassword.Text).AddOnCompleteListener(this);
            }
            else if (v == btnSignIn_Page)//Go to LogIn page
            {
                Intent intent = new Intent(this, typeof(SignInActivity));
                StartActivity(intent);
            }
        }
        /// <summary>
        /// gets called when we add a listener and the firebase db returns a task - signs up or signs in automatically if we signed in before.
        /// </summary>
        /// <param name="task">The task that we get from firebase</param>
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == taskSignUp)
                {
                    user.Email = etEmail.Text;
                    user.Password = etPassword.Text;
                    user.UserName = etEmail.Text.Split('@')[0];
                    Toast.MakeText(this, "Created new user", ToastLength.Long).Show();
                }
                else if (task == taskSignIn)
                {
                    Toast.MakeText(this, "User " + user.UserName + " signed in automatically", ToastLength.Long).Show();
                }
                sp.SetData(Constants.EMAIL, user.Email);
                sp.SetData(Constants.PASSWORD, user.Password);
                sp.SetData(Constants.USERNAME, user.UserName);
                Intent intent = new Intent(this, typeof(HomeActivity));
                StartActivity(intent);
                Finish();
            }
            else
                Toast.MakeText(this, task.Exception.Message, ToastLength.Long).Show();
        }
    }
}