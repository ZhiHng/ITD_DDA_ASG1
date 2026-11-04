/*
* Author: Zhi hng
* Date: 4 November 2026
* Description: Handles authentication then calls the database to read/write user data
*/

using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;

public class FirebaseAuthentication : MonoBehaviour
{
    Firebase.Auth.FirebaseAuth auth;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField usernameInputField;
    [SerializeField] FirebaseGameDatabase gameDatabase;
    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void SignUp()
    {
        var createTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(emailInputField.text, passwordInputField.text);
        createTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignUp was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignUp encountered an error: " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("SignUp completed successfully. Please Sign in.");
                gameDatabase.WriteNewUser(task.Result.User.UserId, usernameInputField.text);
            }
        });
    }

    public void SignIn()
    {
        var signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(emailInputField.text, passwordInputField.text);
        signInTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignIn was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignIn encountered an error: " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("SignIn completed successfully.");
                var uid = task.Result.User.UserId;
                
                Debug.Log("User ID: " + uid);
                gameDatabase.currentUserId = uid;
                gameDatabase.ReadUserData(uid);
            }
        });
    }
}
