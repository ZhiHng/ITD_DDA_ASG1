/*
* Author: Zhi hng
* Date: 27 November 2026
* Description: Game Manager to store user data locally and manage game state
*/

using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
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
    //string[] cardIds = { "B1", "B2", "I1", "I2", "I3", "S1", "S2"};
    [SerializeField] Animator countdownAnim;
    Dictionary<int, string[]> levelEnemies = new Dictionary<int, string[]>()
    {
        {0, new string[] {"B1", "B2"}},
        {1, new string[] {"B1", "B2", "I1"}},
        {2, new string[] {"B1", "B2", "I1", "I2", "I3"}},
        {3, new string[] {"B1", "B2", "I1", "I2", "I3", "S1", "S2"}},
        {4, new string[] {"B1", "B2", "I1", "I2", "I3", "S1", "S2"}},
    };
    Coroutine spawnEnemyCoroutine;

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
    }

    // void OnSpawnAlly()
    // {
    //     string cardId = cardIds[Random.Range(0, cardIds.Length)];
    //     CardManager.instance.SpawnUnit(cardId, allySpawnPosition + Vector3.right * Random.Range(-0.175f, 0.175f) + Vector3.up * 0.01f, true);
    // }

    // void OnSpawnEnemy()
    // {
    //     string cardId = cardIds[Random.Range(0, cardIds.Length)];
    //     CardManager.instance.SpawnUnit(cardId, enemySpawnPosition + Vector3.right * Random.Range(-0.175f, 0.175f) + Vector3.up * 0.01f, false);
    // }

    public void GameEnd(bool isWin)
    {
        StopCoroutine(spawnEnemyCoroutine);
    }
    public void GameStart(int level)
    {
        CardManager.instance.SpawnUnit("B1", ImageTracker.ArenaInstance.transform.Find("AllySpawn").transform.position, true, true, false);
        switch (level)
        {
            case 0:
                CardManager.instance.SpawnUnit("B1", ImageTracker.ArenaInstance.transform.Find("EnemySpawn").transform.position, false, false, true);
                break;
            case 1:
                CardManager.instance.SpawnUnit("B2", ImageTracker.ArenaInstance.transform.Find("EnemySpawn").transform.position, false, false, true);
                break;
            case 2:
                CardManager.instance.SpawnUnit("I1", ImageTracker.ArenaInstance.transform.Find("EnemySpawn").transform.position, false, false, true);
                break;
            case 3:
                CardManager.instance.SpawnUnit("I2", ImageTracker.ArenaInstance.transform.Find("EnemySpawn").transform.position, false, false, true);
                break;
            case 4:
                CardManager.instance.SpawnUnit("I3", ImageTracker.ArenaInstance.transform.Find("EnemySpawn").transform.position, false, false, true);
                break;
        }
        //Start Animation
        countdownAnim.SetTrigger("StartCountdown");
        StartCoroutine(LevelCountdown());
    }
    IEnumerator SpawnEnemies(int level)
    {
        Transform enemySpawn = ImageTracker.ArenaInstance.GetNamedChild("EnemySpawn").transform;
        string[] spawnEnemies = levelEnemies[level];
        while (true)
        {  
            foreach (string unit in spawnEnemies)
            {
                yield return new WaitForSeconds(Mathf.Max(0.1f, Random.Range(5-level, 7-level)));
                Vector3 spawnPos = enemySpawn.position + enemySpawn.right * Random.Range(-0.175f, 0.175f);
                CardManager.instance.SpawnUnit(unit, spawnPos, false);
            }
        }
    }

    IEnumerator LevelCountdown()
    {
        yield return new WaitForSeconds(3);
        spawnEnemyCoroutine = StartCoroutine(SpawnEnemies(level));
    }
}
