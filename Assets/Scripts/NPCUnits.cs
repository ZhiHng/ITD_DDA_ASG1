/*
* Author: Zhi hng
* Date: 7 November 2026
* Description: Script for the NPC units to move, interact, and run the game mechanics.
*/

using UnityEngine;

public class NPCUnits : MonoBehaviour
{
    public bool isAlly;
    public Card card;

    public void Initialize(Card cardData, bool ally)
    {
        card = cardData;
        isAlly = ally;
    }
    
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
