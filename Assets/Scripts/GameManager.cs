/*
* Author: Zhi hng
* Date: 21 November 2026
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

    //Game state variables
    public int money;
    public int spawnedUnits;
    public int maxSpawnedUnits = 3;
    Vector3 allySpawnPosition;
    Vector3 enemySpawnPosition;
    string[] cardIds = { "B1", "B", "I1", "I2", "I3", "S1", "S2"};

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
        maxSpawnedUnits += level;
        allySpawnPosition = GameObject.FindWithTag("AllySpawn").transform.position;
        enemySpawnPosition = GameObject.FindWithTag("EnemySpawn").transform.position;
    }

    void OnSpawnAlly()
    {
        CardManager.instance.SpawnUnit(cardIds[Random.Range(0, cardIds.Length)], allySpawnPosition + Vector3.right * Random.Range(-0.175f, 0.175f), true);
    }

    void OnSpawnEnemy()
    {
        CardManager.instance.SpawnUnit(cardIds[Random.Range(0, cardIds.Length)], enemySpawnPosition + Vector3.right * Random.Range(-0.175f, 0.175f), false);
    }
}
