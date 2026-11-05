/*
* Author: Zhi hng
* Date: 5 November 2026
* Description: Game Manager to store user data locally and manage game state
*/

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string userId;
    public string username;
    public int level;
    public int experience;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
