/*
* Author: Zhi hng
* Date: 22 November 2026
* Description: Script for the NPC units to move, interact, and run the game mechanics.
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Image healthBar;
    [SerializeField] Transform projectileSpawnPoint;
    [HideInInspector] public Vector3 moveDirection;
    bool isLoaded = true;
    float aggroRange = 0.1f;
    float moveSpeed = 0.02f;
    float maxHealth;
    
    void Start()
    {
        maxHealth = card.health;
        healthBar.fillAmount = 1f;
        if (isAlly)
        {
            gameObject.tag = "Ally";
            healthBar.color = Color.green;
            moveDirection = transform.forward; //arena.transform.forward;
        }
        else
        {
            gameObject.tag = "Enemy";
            healthBar.color = Color.red;
            moveDirection = -transform.forward; //-arena.transform.forward;
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
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (targetEnemy == null)
        {
            FindClosestEnemy();
        }
        if ((other.gameObject.CompareTag("AllyProjectile") && !isAlly) || other.gameObject.CompareTag("EnemyProjectile") && isAlly)
        {
            TakeDamage(other.gameObject.GetComponent<DamageSource>().damage);
        }
    }

    void FindClosestEnemy()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, aggroRange + card.range / 100f);

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

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        projectileScript.Initialize(card.projectileColor, card.areaDamage, isAlly, card.damage);

        isLoaded = false;
        StartCoroutine(AttackCooldown());
    }

    public void TakeDamage(int damage)
    {
        card.health -= damage;
        healthBar.fillAmount = card.health / maxHealth;
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
