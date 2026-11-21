/*
* Author: Zhi hng
* Date: 4 November 2026
* Description: User class to represent user data in Firebase Realtime Database
*/

[System.Serializable]
public class User
{
    public string username;
    public int level;
    public int experience;

    public User() {}

    public User(string username, int level = 0, int experience = 0)
    {
        this.username = username;
        this.level = level;
        this.experience = experience;
    }
}
