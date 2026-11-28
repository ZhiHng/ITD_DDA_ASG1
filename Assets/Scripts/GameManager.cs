/*
* Author: Zhi hng
* Date: 28 November 2026
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
    public int gameLevel;
    public int playingLevel;


    //Game state variables
    public int money;
    public int spawnedUnits;
    public int merges;
    float baseHealth;
    public int maxSpawnedUnits = 3;
    Transform enemySpawn;
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

    void Awake()
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

    public void GameEnd(bool isWin)
    {
        StopCoroutine(spawnEnemyCoroutine);
        FindFirstObjectByType<ImageTracker>().enabled = false;
        NPCUnits[] npcScripts = FindObjectsByType<NPCUnits>(0);
        foreach (NPCUnits script in npcScripts)
        {
            if (script.isBase)
                baseHealth = script.card.health;
            script.enabled = false;
        }
        int gainedExperience = 0;
        if (isWin)
        {
            gainedExperience += 50 * (playingLevel + 1) + Mathf.Min(merges * (playingLevel + 1), 25 * (playingLevel + 1));
            if (playingLevel == gameLevel)
            {
                gameLevel++;
            }
        }
        gainedExperience += Mathf.Min(spawnedUnits * (playingLevel + 1), 10 * (playingLevel + 1));
        experience += gainedExperience;
        int levelUp = 0;
        int levelsGained = 0;
        while (experience >= GetRequiredXP(level))
        {
            experience -= GetRequiredXP(level);
            level++;
            levelUp = level;
            levelsGained++;
        }

        string levelUpString = "";
        if (levelUp != 0)
        {
            levelUpString = "Leveled Up! + " + levelsGained + "\nNew Level: " + levelUp;
        }

        string stats = "Remaining Base Health: " + baseHealth + "\nSpawned Units: " + spawnedUnits + "\nMerges: " + merges + "\nExperience Gained: " + gainedExperience + "\n" + levelUpString;
        UIManager.instance.ShowGameEndScreen(true, isWin, playingLevel, stats);
        UIManager.instance.UpdatePlayableLevels();
        GetComponent<FirebaseGameDatabase>().UpdateDatabase(userId, level, experience, gameLevel);
    }
    public void GameStart(int level)
    {
        playingLevel = level;
        enemySpawn = ImageTracker.ArenaInstance.GetNamedChild("EnemySpawn").transform;
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
        spawnEnemyCoroutine = StartCoroutine(SpawnEnemies(playingLevel));
    }

    public int GetRequiredXP(int level)
    {
        return 100 + (level * 50);
    }
}
