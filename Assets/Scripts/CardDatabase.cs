/*
* Author: Zhi hng
* Date: 5 November 2026
* Description: Data for the stats of the cards that will be scanned with AR
*/
using System.Collections.Generic;

[System.Serializable]
public class Card
{
    public string name;
    public string description;
    public int damage;
    public int health;
    public int range;
    public bool areaDamage;
    public int cost;
    public string[] mergeable;
    public bool isMerged;
}

public class CardDatabase
{
    public static Dictionary<string, Card> cards =
        new Dictionary<string, Card>()
    {
        {
            "C1",
            new Card
            {
                name = "Rice",
                description = "Most basic ingredient to make sushi.",
                damage = 1,
                health = 10,
                range = 1,
                areaDamage = false,
                cost = 20
            }
        },
        {
            "C2",
            new Card
            {
                name = "Seaweed",
                description = "Also known as nori, used to wrap sushi.",
                damage = 1,
                health = 5,
                range = 3,
                areaDamage = false,
                cost = 20
            }
        },
        {
            "C3",
            new Card
            {
                name = "Salmon",
                description = "A popular fish used in sushi.",
                damage = 3,
                health = 10,
                range = 1,
                areaDamage = true,
                cost = 30,
                mergeable = new string[] { "C1", "C2" },
                isMerged = false
            }
        },
        {
            "C4",
            new Card
            {
                name = "Tuna",
                description = "Another popular fish used in sushi.",
                damage = 4,
                health = 4,
                range = 3,
                areaDamage = false,
                cost = 30,
                mergeable = new string[] { "C1", "C2" },
                isMerged = false
            }
        },
        {
            "C5",
            new Card
            {
                name = "Eel",
                description = "A type of fish used in sushi, often grilled.",
                damage = 5,
                health = 3,
                range = 2,
                areaDamage = false,
                cost = 50
            }
        },
        {
            "C6",
            new Card
            {
                name = "Shrimp",
                description = "A type of seafood used in sushi.",
                damage = 3,
                health = 4,
                range = 2,
                areaDamage = false,
                cost = 30
            }
        },
        {
            "C7",
            new Card
            {
                name = "Octopus",
                description = "A type of seafood used in sushi.",
                damage = 4,
                health = 3,
                range = 2,
                areaDamage = false,
                cost = 40
            }
        },
        {
            "C8",
            new Card
            {
                name = "Squid",
                description = "A type of seafood used in sushi.",
                damage = 5,
                health = 2,
                range = 2,
                areaDamage = false,
                cost = 50
            }
        },
        
    };
}