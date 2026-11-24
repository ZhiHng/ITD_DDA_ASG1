/*
* Author: Zhi hng
* Date: 24 November 2026
* Description: Data for the stats of the cards that will be scanned with AR
*/

using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
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
    }

    public List<CardPrefab> cardPrefabs;
    public Card GetCardDetails(string id)
    {
        if (CardDatabase.cards.TryGetValue(id, out Card card))
        {
            Card clonedCard = card.Clone();
            clonedCard.id = id;

            return clonedCard;
        }

        Debug.LogError("Card with ID " + id + " not found.");
        return null;
    }

    public GameObject GetPrefab(string cardId, bool isGetVisual)
    {
        if (isGetVisual)
        {
            foreach (var cardVisual in cardPrefabs)
            {
                if (cardVisual.cardId == cardId)
                {
                    return cardVisual.visualPrefab;
                }
            }

            Debug.LogError("Visual Prefab for card ID " + cardId + " not found.");
            return null;
        }
        else
        {
            foreach (var cardPrefab in cardPrefabs)
            {
                if (cardPrefab.cardId == cardId)
                {
                    return cardPrefab.prefab;
                }
            }

            Debug.LogError("Prefab for card ID " + cardId + " not found.");
            return null;
        }
    }

    public bool CheckIfCardsCanMerge(Card currentCard, Card otherCard)
    {
        // Special ingredients can only be used once
        if (otherCard.mergedWith.Contains(currentCard.id))
        {
            //Debug.Log("This card has already been merged.");
            return false;
        }

        if (currentCard.mergeable.Contains("U"))
        {
            return true;
        }

        if (!currentCard.mergeable.Contains(otherCard.id))
        {
            //Debug.LogError("Card with ID " + otherCard.id + " cannot be merged with card with ID " + currentCard.id);
            return false;
        }

        return true;
    }

    public Card GetMergedCardDetails(Card currentCard, Card mergedWithCard)
    {
        Card card;

        if (currentCard.id.StartsWith("S"))
        {
            // Special cards enhance the target card
            card = mergedWithCard.Clone();

            card.damage += currentCard.damage;
            card.health += currentCard.health;
            card.range += currentCard.range;
            card.areaDamage |= currentCard.areaDamage;
            card.attackCooldown = Mathf.Min(currentCard.attackCooldown, mergedWithCard.attackCooldown);
            card.moveSpeed += currentCard.moveSpeed / 3;

            card.mergedWith.Add(currentCard.id);
            card.mergeable.Remove(currentCard.id);
        }
        else
        {
            // Ingredients absorb the target card
            card = currentCard.Clone();

            card.damage += mergedWithCard.damage;
            card.health += mergedWithCard.health;
            card.range += mergedWithCard.range;
            card.areaDamage |= mergedWithCard.areaDamage;
            card.attackCooldown = Mathf.Min(currentCard.attackCooldown, mergedWithCard.attackCooldown);
            card.moveSpeed += mergedWithCard.moveSpeed / 3;

            card.mergedWith.Add(mergedWithCard.id);
            card.mergeable.Remove(mergedWithCard.id);
        }

        return card;
    }

    public void ActivateEffect(Card card)
    {
        switch(card.effect)
        {
            case EffectType.GiveMoney:
                GameManager.instance.money += (int)card.effectValue;
                break;

            case EffectType.Heal:
                card.health += (int)card.effectValue;
                break;
            case EffectType.AttackSpeedBoost:
                card.attackCooldown = Mathf.Max(0.1f, card.attackCooldown - card.effectValue);
                break;
        }
    }

    public void SpawnUnit(string cardId, Vector3 spawnPosition, bool isAlly)
    {
        Card card = GetCardDetails(cardId);

        GameObject prefab = GetPrefab(cardId, false);

        GameObject unitObject = Instantiate(prefab, spawnPosition, Quaternion.identity);//ImageTracker.ArenaInstance.transform

        NPCUnits npc = unitObject.GetComponent<NPCUnits>();

        npc.Initialize(card, isAlly);
    }
}

[System.Serializable]
public class Card
{
    public string id;
    public string name;
    public string description;

    public int damage;
    public int health;
    public int range;
    public float attackCooldown;
    public float moveSpeed;

    public bool areaDamage;
    public int cost;

    public List<string> mergeable;
    public List<string> mergedWith;

    public EffectType effect;
    public float effectValue;
    public Color projectileColor;

    public Card Clone()
    {
        return new Card
        {
            id = id,
            name = name,
            description = description,

            damage = damage,
            health = health,
            range = range,
            attackCooldown = attackCooldown,
            moveSpeed = moveSpeed,

            areaDamage = areaDamage,
            cost = cost,

            effect = effect,
            effectValue = effectValue,
            projectileColor = projectileColor,

            mergeable = new List<string>(mergeable),
            mergedWith = new List<string>(mergedWith)
        };
    }
}

public enum EffectType
{
    None,
    GiveMoney,
    Heal,
    AttackSpeedBoost
}

public class CardDatabase
{
    public static Dictionary<string, Card> cards = new Dictionary<string, Card>()
    {
        {
            "B1",
            new Card
            {
                name = "Rice",
                description = "Most basic ingredient to make sushi.",
                damage = 1,
                health = 10,
                range = 1,
                attackCooldown = 1f,
                moveSpeed = 2,
                areaDamage = false,
                cost = 20,
                mergeable = new List<string>() {},
                mergedWith = new List<string>() {},
                effect = EffectType.None,
                effectValue = 0,
                projectileColor = Color.white
            }
        },
        {
            "B2",
            new Card
            {
                name = "Seaweed",
                description = "Also known as nori, used to wrap sushi.",
                damage = 1,
                health = 5,
                range = 8,
                attackCooldown = 1f,
                moveSpeed = 1,
                areaDamage = false,
                cost = 20,
                mergeable = new List<string>() {},
                mergedWith = new List<string>() {},
                effect = EffectType.None,
                effectValue = 0,
                projectileColor = Color.darkGreen
            }
        },
        {
            "I1",
            new Card
            {
                name = "Salmon",
                description = "A popular fish used in sushi.",
                damage = 3,
                health = 10,
                range = 1,
                attackCooldown = 1f,
                moveSpeed = 2,
                areaDamage = true,
                cost = 30,
                mergeable = new List<string>() {"B1"},
                mergedWith = new List<string>() {},
                effect = EffectType.Heal,
                effectValue = 5,
                projectileColor = Color.orange
            }
        },
        {
            "I2",
            new Card
            {
                name = "Tuna",
                description = "Another popular fish used in sushi.",
                damage = 4,
                health = 4,
                range = 5,
                attackCooldown = 1f,
                moveSpeed = 2,
                areaDamage = false,
                cost = 30,
                mergeable = new List<string>() {"B1",},
                mergedWith = new List<string>() {},
                effect = EffectType.AttackSpeedBoost,
                effectValue = 0.5f,
                projectileColor = Color.red
            }
        },
        {
            "I3",
            new Card
            {
                name = "Egg",
                description = "Also known as Tamago.",
                damage = 2,
                health = 15,
                range = 1,
                attackCooldown = 1f,
                moveSpeed = 1,
                areaDamage = true,
                cost = 25,
                mergeable = new List<string>() { "B1", "B2" },
                mergedWith = new List<string>() {},
                effect = EffectType.GiveMoney,
                effectValue = 10,
                projectileColor = Color.yellow
            }
        },
        {
            "S1",
            new Card
            {
                name = "Wasabi",
                description = "Eaten with sushi to add spice.",
                damage = 5,
                health = 1,
                range = 0,
                attackCooldown = 1f,
                moveSpeed = 3,
                areaDamage = false,
                cost = 10,
                mergeable = new List<string>() {"U"},
                mergedWith = new List<string>() {},
                effect = EffectType.None,
                effectValue = 0,
                projectileColor = Color.green
            }
        },
        {
            "S2",
            new Card
            {
                name = "Soy Sauce",
                description = "Eaten with sushi to add a salty flavor.",
                damage = 0,
                health = 5,
                range = 1,
                attackCooldown = 1f,
                moveSpeed = 3,
                areaDamage = true,
                cost = 15,
                mergeable = new List<string>() {"U"},
                mergedWith = new List<string>() {},
                effect = EffectType.None,
                effectValue = 0,
                projectileColor = Color.brown
            }
        }
    };
}

[System.Serializable]
public class CardPrefab
{
    public string cardId;
    public GameObject prefab;
    public GameObject visualPrefab;
}