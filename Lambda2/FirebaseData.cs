using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Gms.Tasks;
using Android.Graphics;
using Firebase.Storage;

namespace Lambda2
{
    /// <summary>
    /// controlls the firebase gameActivity of the project, with many functions to help.
    /// </summary>
    public class FirebaseData
    {
        private FirebaseFirestore firestore;
        private FirebaseAuth auth;
        private FirebaseApp app;

        /// <summary>
        /// constructor
        /// </summary>
        public FirebaseData()
        {
            app = FirebaseApp.InitializeApp(Application.Context);
            if (app is null)
            {
                FirebaseOptions options = GetMyOptions();
                app = FirebaseApp.InitializeApp(Application.Context, options);
            }
            firestore = FirebaseFirestore.GetInstance(app);
            auth = FirebaseAuth.Instance;
        }
        /// <summary>
        /// return the firebase options
        /// </summary>
        /// <returns>firebase identification</returns>
        private FirebaseOptions GetMyOptions()
        {
            return new FirebaseOptions.Builder()
                    .SetProjectId("lambda-febb5")
                    .SetApplicationId("lambda-febb5")
                    .SetApiKey("AIzaSyBW_4MfdNcyTHDyyoFspLuCyGQbYoCprzg")
                    .SetDatabaseUrl("https://lambda-febb5-default-rtdb.firebaseio.com/")
                    .SetStorageBucket("lambda-febb5.appspot.com")
                    .Build();
        }
        /// <summary>
        /// creates uer with email and password
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>task that is supposed to create user</returns>
        public Android.Gms.Tasks.Task CreateUser(string email, string password)
        {
            return auth.CreateUserWithEmailAndPassword(email, password);
        }
        /// <summary>
        /// signs in with email and password
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">email</param>
        /// <returns>task that is suppose to sign in</returns>
        public Android.Gms.Tasks.Task SignIn(string email, string password)
        {
            return auth.SignInWithEmailAndPassword(email, password);
        }
        /// <summary>
        /// adds document to collection, with given info
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="dName">document name</param>
        /// <param name="hmFields">hashmap with fields to enter the document</param>
        /// <returns>task that is supposed to add document</returns>
        public Task AddDocumentToCollection(string cName, string dName, HashMap hmFields)//Use Dictionary instead of hashmap
        {
            DocumentReference cr;
            if (dName is null)
                cr = firestore.Collection(cName).Document();
            else
                cr = firestore.Collection(cName).Document(dName);
            return cr.Set(hmFields);
        }
        /// <summary>
        /// adds snapshot listener to collection, so OnEvent would be called if there's a change
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="activity">game gameActivity</param>
        public void AddSnapshotListenerToCollection(string cName, GameActivity gameActivity)
        {
            firestore.Collection(cName).AddSnapshotListener(gameActivity);
        }
        /// <summary>
        /// returns the task with a certain document
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="dName">document name</param>
        /// <returns>task with document</returns>
        public Task GetDocument(string cName, string dName)
        {
            return firestore.Collection(cName).Document(dName).Get();
        }
        /// <summary>
        /// adds snapshot listener to document, so OnEvent would be called if there's a change
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="dName">document name</param>
        /// <param name="homeActivity"">home activity</param>
        public void AddSnapShotListenerToDocument(string cName, string dName, HomeActivity homeActivity)
        {
            firestore.Collection(cName).Document(dName).AddSnapshotListener(homeActivity);
        }
        /// <summary>
        /// adds snapshot listener to document, so OnEvent would be called if there's a change
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="dName">document name</param>
        /// <param name="gameActivity">game activity</param>
        public void AddSnapShotListenerToDocument(string cName, string dName, GameActivity gameActivity)
        {
            firestore.Collection(cName).Document(dName).AddSnapshotListener(gameActivity);
        }
        /// <summary>
        /// returns collection
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <returns>task that is supposed to return collection</returns>
        public Task GetCollection(string cName)
        {
            return firestore.Collection(cName).Get();
        }
        /// <summary>
        /// updates document with key and bool value
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="dName">document name</param>
        /// <param name="key">name of value (the key)</param>
        /// <param name="value">boolean value</param>
        /// <returns> task after the action</returns>
        public Task UpdateDocument(string cName, string dName, string key, bool value)
        {
            return firestore.Collection(cName).Document(dName).Update(key, value);
        }
        /// <summary>
        /// updates document with key and string value
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="dName">document name</param>
        /// <param name="key">name of value (the key)</param>
        /// <param name="value">string value</param>
        /// <returns> task after the action</returns>
        public Task UpdateDocument(string cName, string dName, string key, string value)
        {
            return firestore.Collection(cName).Document(dName).Update(key, value);
        }
        /// <summary>
        /// updates document with key and int value
        /// </summary>
        /// <param name="cName">collection name</param>
        /// <param name="dName">document name</param>
        /// <param name="key">name of value (the key)</param>
        /// <param name="value">int value</param>
        /// <returns> task after the action</returns>
        public Task UpdateDocument(string cName, string dName, string key, int value)
        {
            return firestore.Collection(cName).Document(dName).Update(key, value);
        }
        /// <summary>
        /// saves bitmap in storage according to image name given
        /// </summary>
        /// <param name="bitmap">bitmap - the picture</param>
        /// <param name="imgName">image name to put in storage</param>
        /// <returns>task th</returns>
        public Task SaveImage(Bitmap bitmap, string imgName)
        {
            StorageReference storageReference = FirebaseStorage.Instance.GetReference(Constants.FS_IMAGES + imgName);
            Byte[] imgBytes = BitmapFunctions.BitmapToByteArray(bitmap);
            return storageReference.PutBytes(imgBytes);
        }
    }
}