/*
* Author: Zhi hng
* Date: 28 November 2026
* Description: CRUD operations for user data in Firebase Realtime Database
*/

using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class FirebaseGameDatabase : MonoBehaviour
{
    DatabaseReference databaseReference;
    [SerializeField] TMP_InputField usernameInputField;
    void Start()
    {
        databaseReference = FirebaseDatabase.GetInstance("https://speedy-sushi-ar-default-rtdb.asia-southeast1.firebasedatabase.app").RootReference;
        print("User data written to Firebase Realtime Database.");
        //FirebaseDatabase.DefaultInstance.GetReference("users").ValueChanged += HandleDataChanged;
    }
    public void WriteNewUser(string userId, string username)
    {
        User user = new User(username);
        string json = JsonUtility.ToJson(user);

        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }
    public void ChangeUsername(string currentUserId, string newUsername)
    {
        databaseReference.Child("users").Child(currentUserId).Child("username").SetValueAsync(newUsername);
    }
    public void DeleteUser(string currentUserId)
    {
        databaseReference.Child("users").Child(currentUserId).RemoveValueAsync();
    }
    public void ReadUserData(string currentUserId)
    {
        databaseReference.Child("users").Child(currentUserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string username = snapshot.Child("username").Value.ToString();
                    int level = int.Parse(snapshot.Child("level").Value.ToString());
                    int experience = int.Parse(snapshot.Child("experience").Value.ToString());
                    int gameLevel = int.Parse(snapshot.Child("gameLevel").Value.ToString());
                    Debug.Log($"User ID: {currentUserId}, Username: {username}, Level: {level}, Experience: {experience}, Game Level: {gameLevel}");
                    GameManager.instance.userId = currentUserId;
                    GameManager.instance.username = username;
                    GameManager.instance.level = level;
                    GameManager.instance.experience = experience;
                    GameManager.instance.gameLevel = gameLevel;
                    UIManager.instance.SwitchMenu(1);
                }
                else
                {
                    Debug.Log($"User ID: {currentUserId} does not exist in the database.");
                }
            }
        });
    }

    public void ContinueAsGuest()
    {
        GameManager.instance.userId = "Guest";
        GameManager.instance.username = "Guest";
        GameManager.instance.level = 1;
        GameManager.instance.experience = 0;
        GameManager.instance.gameLevel = 0;
        UIManager.instance.SwitchMenu(1);
    }
    void HandleDataChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogError("Error retrieving username data from Firebase: " + e.DatabaseError.Message);
            return;
        }

        if (e.Snapshot.Exists)
        {
            string username = e.Snapshot.Value.ToString();
            Debug.Log("Username changed: " + username);
        }
    }

    public void UpdateDatabase(string currentUserId, int level, int experience, int gameLevel)
    {
        if (currentUserId == "")
            return;

        databaseReference.Child("users").Child(currentUserId).Child("level").SetValueAsync(level);
        databaseReference.Child("users").Child(currentUserId).Child("experience").SetValueAsync(experience);
        databaseReference.Child("users").Child(currentUserId).Child("gameLevel").SetValueAsync(gameLevel);
    }
}