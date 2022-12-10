using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class AuthManager : MonoBehaviour
{
    private static AuthManager _singleton;
    public static AuthManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(AuthManager)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    //variables
    [Header("Firebase Vars")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            } else
            {
                Debug.LogError("Could not resolve all Firebase dependencies " + dependencyStatus);
            }
        });
        Singleton = this;
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void LoginAttempt(string email, string password, ushort id)
    {
        StartCoroutine(Login(email, password, id));
    }

    public void RegisterAttempt(string email, string password, string password2, string username, ushort id)
    {
        StartCoroutine(Register(email, password, password2, username, id));
    }

    private IEnumerator Login(string _email, string _password, ushort id)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if(LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            SendWarning(message, id);
            NetworkManager.Singleton.Server.DisconnectClient(id);
        } else
        {
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            //warningLoginText.text = "";

            GameLogic.Singleton.SendScene("Menu", id);
            AcceptLogin(id, User.DisplayName);
        }
    }

    private IEnumerator Register(string _email, string _password, string _password2, string _username, ushort id)
    {
        if(_username == "")
        {
            SendWarning("Missing Username", id);
            NetworkManager.Singleton.Server.DisconnectClient(id);
        } else if(_password != _password2) {
            SendWarning("Passwords Do Not Match!", id);
            NetworkManager.Singleton.Server.DisconnectClient(id);
        } else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
            if(RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                SendWarning(message, id);
                NetworkManager.Singleton.Server.DisconnectClient(id);
            } else
            {
                User = RegisterTask.Result;

                if(User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if(ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        NetworkManager.Singleton.Server.DisconnectClient(id);
                        SendWarning("Username Set Failed", id);
                    } else
                    {
                        SendWarning("Congrats!", id);
                        NetworkManager.Singleton.Server.DisconnectClient(id);
                    }
                }
            }
        }
    }
    


    private IEnumerator UpdateUsernameAuth(string _username)
    {
        UserProfile profile = new UserProfile { DisplayName = _username };

        var ProfileTask = User.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if(ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        } else
        {

        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        } else
        {

        }
    }

    #region messages

    public void SendWarning(string warn, ushort toClientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.warningMessage);
        message.AddString(warn);
        NetworkManager.Singleton.Server.Send((message), toClientId);
    }

    public void AcceptLogin(ushort toClientId, string username)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.acceptLogin);
        message.AddBool(true);
        message.AddString(username);
        NetworkManager.Singleton.Server.Send((message), toClientId);

    }

    #endregion
}
