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
    string[] cardIds = { "B1", "B2", "I1", "I2", "I3", "S1", "S2"};

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
        string cardId = cardIds[Random.Range(0, cardIds.Length)];
        print("Spawning ally unit, cardId: " + cardId);
        CardManager.instance.SpawnUnit(cardId, allySpawnPosition + Vector3.right * Random.Range(-0.175f, 0.175f) + Vector3.up * 0.01f, true);
    }

    void OnSpawnEnemy()
    {
        string cardId = cardIds[Random.Range(0, cardIds.Length)];
        print("Spawning enemy unit, cardId: " + cardId);
        CardManager.instance.SpawnUnit(cardId, enemySpawnPosition + Vector3.right * Random.Range(-0.175f, 0.175f) + Vector3.up * 0.01f, false);
    }
}
