/*
* Author: Zhi hng
* Date: 21 November 2026
* Description: Script for the NPC units to move, interact, and run the game mechanics.
*/

using System.Collections;
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
    [SerializeField] GameObject projectilePrefab;
    bool isLoaded = true;
    float aggroRange = 0.05f;
    float moveSpeed = 0.02f;
    
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
            if (Vector3.Distance(transform.position, targetEnemy.transform.position) <= card.range / 100f) // convert range from cm to m
            {
                if (isLoaded)
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
            if (isAlly)
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            else
            {
                transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            }
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
            TakeDamage(other.gameObject.GetComponent<ProjectileScript>().projectileDamage);
        }
        else if (other.gameObject.CompareTag("EnemyProjectile") && isAlly)
        {
            TakeDamage(other.gameObject.GetComponent<ProjectileScript>().projectileDamage);
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
        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        GameObject projectile = Instantiate(projectilePrefab, transform.position + direction, Quaternion.identity);
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        projectileScript.Initialize(card.projectileColor, card.areaDamage, isAlly, card.damage);

        isLoaded = false;
        StartCoroutine(AttackCooldown());
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

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(card.attackCooldown);
        isLoaded = true;
    }
}
