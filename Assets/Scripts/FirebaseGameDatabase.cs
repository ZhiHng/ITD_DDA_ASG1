/*
* Author: Zhi hng
* Date: 5 November 2026
* Description: CRUD operations for user data in Firebase Realtime Database
*/

using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.UI;
using TMPro;

public class FirebaseGameDatabase : MonoBehaviour
{
    DatabaseReference databaseReference;
    [SerializeField] TMP_InputField usernameInputField;
    public string currentUserId;
    void Start()
    {
        databaseReference = FirebaseDatabase.GetInstance("https://speedy-sushi-ar-default-rtdb.asia-southeast1.firebasedatabase.app").RootReference;
        print("User data written to Firebase Realtime Database.");
        FirebaseDatabase.DefaultInstance.GetReference("users").ValueChanged += HandleUsernameChanged;
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
                    string level = snapshot.Child("level").Value.ToString();
                    string experience = snapshot.Child("experience").Value.ToString();
                    Debug.Log($"User ID: {currentUserId}, Username: {username}, Level: {level}, Experience: {experience}");
                    GameManager.instance.userId = currentUserId;
                    GameManager.instance.username = username;
                    GameManager.instance.level = int.Parse(level);
                    GameManager.instance.experience = int.Parse(experience);
                }
                else
                {
                    Debug.Log($"User ID: {currentUserId} does not exist in the database.");
                }
            }
        });
    }
    void HandleUsernameChanged(object sender, ValueChangedEventArgs e)
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
    
    
}