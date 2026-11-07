/*
* Author: Zhi hng
* Date: 7 November 2026
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

        //Spawn NPC
        Card card = CardManager.instance.GetCardDetails("I1");

        GameObject prefab = CardManager.instance.GetPrefab(card.id);

        GameObject unit = Instantiate(prefab);
        unit.GetComponent<NPCUnits>().card = card;
    }

    
}
