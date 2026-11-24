/*
* Author: Zhi hng
* Date: 24 November 2026
* Description: Script for the NPC units to move, interact, and run the game mechanics.
*/

using System.Collections;
using System.Linq;
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
        BuildVisualModel();
    }

    GameObject targetEnemy;
    [SerializeField] GameObject deathEffect;
    [SerializeField] GameObject mergeEffect;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Image healthBar;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] Transform visualRoot;
    [HideInInspector] public Vector3 moveDirection;
    bool isLoaded = true;
    float aggroRange = 0.01f;
    float mergeRange = 0.03f;
    float mergeSpeed = 0.1f;
    [HideInInspector] public bool isMerging = false;
    [HideInInspector] public float maxHealth;

    void BuildVisualModel()
    {
        foreach (Transform child in visualRoot) //Clear Model when building/rebuilding
        {
            Destroy(child.gameObject);
        }

        int layer = 0;
        if (card.mergedWith.Count != 0)
        {
            if (card.mergedWith.Contains("B1"))
            {
                Instantiate(CardManager.instance.GetPrefab("B1", true), visualRoot);
                layer++; //Rice always bottom
            }
            if (card.mergedWith.Contains("B2"))
            {
                Instantiate(CardManager.instance.GetPrefab("B2", true), visualRoot); //Might have a separate model for a tied up seaweed
                //Seaweed dont really have volume
            }
        }
        GameObject spawnedModel = Instantiate(CardManager.instance.GetPrefab(card.id, true), visualRoot);
        spawnedModel.transform.localPosition = Vector3.up * layer * 0.01f;
        layer++;
        if (card.mergedWith.Any(id => id.StartsWith("S")))
        {
            foreach(string id in card.mergedWith)
            {
                if(id.StartsWith("S"))
                {
                    spawnedModel = Instantiate(CardManager.instance.GetPrefab(id, true), visualRoot);
                    spawnedModel.transform.localPosition = Vector3.up * layer * 0.01f;
                }
            }
        }
        healthBar.transform.parent.transform.localPosition = Vector3.up * (0.01f + layer * 0.01f);
        healthBar.fillAmount = card.health / maxHealth;
    }
    
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
        if (isMerging)
        {
            return;
        }
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
            if (!card.id.StartsWith("B")) // Base ingredients wont check to merge
            {
                FindMergeableAlly();
            }
            FindClosestEnemy();
            transform.position += moveDirection * card.moveSpeed / 100 * Time.deltaTime;
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
    void FindMergeableAlly()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, mergeRange);

        foreach (Collider obj in nearbyObjects)
        {
            NPCUnits ally = obj.GetComponent<NPCUnits>();

            if (ally == null)
                continue;

            if (ally == this)
                continue;

            if (ally.card.id == card.id)
            {
                continue;
            }

            if (ally.isMerging)
                continue;

            if (ally.isAlly != isAlly)
                continue;

            if (CardManager.instance.CheckIfCardsCanMerge(card, ally.card))
            {
                if (card.id.StartsWith("S")) //allows specials to merge with basic ingredients
                {
                    ally.maxHealth += maxHealth;
                    ally.card = CardManager.instance.GetMergedCardDetails(ally.card, card);
                    isMerging = true;
                    ally.isMerging = true;
                    print(card.name + " is merging into " + ally.card.name);
                    StartCoroutine(MergeMovement(this, ally.transform.position, ally));
                    return;
                }

                //Ingredients control the merging
                maxHealth += ally.maxHealth;
                card = CardManager.instance.GetMergedCardDetails(card, ally.card);
                isMerging = true;
                ally.isMerging = true;
                print(ally.card.name + " is merging into " + card.name);
                ally.StartCoroutine(MergeMovement(ally, transform.position, this));
                return;
            }
        }
    }

    void MoveTowardEnemy()
    {
        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        gameObject.transform.LookAt(targetEnemy.transform.position);

        transform.position += direction * card.moveSpeed / 100 * Time.deltaTime;
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

    IEnumerator MergeMovement(NPCUnits unitToDestroy, Vector3 targetLocation, NPCUnits survivingUnit)
    {
        unitToDestroy.transform.LookAt(targetLocation);

        while(Vector3.Distance(unitToDestroy.transform.position, targetLocation) > 0.001f)
        {
            unitToDestroy.transform.position = Vector3.MoveTowards(unitToDestroy.transform.position, targetLocation, mergeSpeed * Time.deltaTime);
            yield return null;
        }

        survivingUnit.isMerging = false;
        survivingUnit.BuildVisualModel();
        Instantiate(mergeEffect, targetLocation, Quaternion.identity);
        Destroy(unitToDestroy.gameObject);
    }
}
