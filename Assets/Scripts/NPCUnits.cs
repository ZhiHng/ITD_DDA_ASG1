/*
* Author: Zhi hng
* Date: 10 November 2026
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

    GameObject targetEnemy;
    [SerializeField] GameObject deathEffect;
    float aggroRange = 5f;
    float moveSpeed = 2f;
    
    void Start()
    {
        if (isAlly)
        {
            gameObject.tag = "Ally";
            gameObject.layer = LayerMask.NameToLayer("Ally");
        }
        else
        {
            gameObject.tag = "Enemy";
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
    }
    void Update()
    {
        if (targetEnemy != null)
        {
            if (Vector3.Distance(transform.position, targetEnemy.transform.position) <= card.range)
            {
                AttackEnemy();
            }
            else
            {
                MoveTowardEnemy();
            }
        }
        else
        {
            FindClosestEnemy();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (targetEnemy == null)
        {
            FindClosestEnemy();
        }
        if (other.gameObject.CompareTag("AllyProjectile") && !isAlly)
        {
            
        }
        else if (other.gameObject.CompareTag("EnemyProjectile") && isAlly)
        {
            
        }
    }

    void FindClosestEnemy()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, aggroRange);

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider obj in nearbyObjects)
        {
            if (isAlly)
            {
                if (!obj.CompareTag("Enemy"))
                    continue;
            }
            else
            {
                if (!obj.CompareTag("Ally"))
                    continue;
            }

            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = obj.gameObject;
            }
        }

        targetEnemy = closestEnemy;
    }

    void MoveTowardEnemy()
    {
        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void AttackEnemy()
    {
        
    }

    public void TakeDamage(int damage)
    {
        card.health -= damage;

        if (card.health <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
