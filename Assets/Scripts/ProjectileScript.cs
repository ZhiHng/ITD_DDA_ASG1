/*
* Author: Zhi hng
* Date: 22 November 2026
* Description: Script for the projectiles for both ally and enemy units.
*/

using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    float speed = 0.1f;
    float lifetime = 1f;
    [SerializeField] GameObject[] impactEffect = new GameObject[2]; //0 for normal, 1 for area damage
    Renderer projectileRenderer;
    bool isAreaDamage;
    public void Initialize(Color color, bool isAreaDamage, bool isAllyProjectile, int damage)
    {
        projectileRenderer = GetComponentInChildren<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        projectileRenderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", color);
        projectileRenderer.SetPropertyBlock(block);

        this.isAreaDamage = isAreaDamage;
        if (isAllyProjectile)
        {
            gameObject.tag = "AllyProjectile";
        }
        else
        {
            gameObject.tag = "EnemyProjectile";
        }
        GetComponent<DamageSource>().damage = damage;
    }

    void Start()
    {
        projectileRenderer = GetComponentInChildren<Renderer>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.CompareTag("Ally") || other.gameObject.CompareTag("Enemy")) || (other.gameObject.CompareTag("Ally") && gameObject.CompareTag("AllyProjectile")) || (other.gameObject.CompareTag("Enemy") && gameObject.CompareTag("EnemyProjectile")))
        {
            return;
        }
        if (!isAreaDamage)
        {
            Instantiate(impactEffect[0], transform.position, Quaternion.identity);
        }
        else
        {
            GameObject explosion = Instantiate(impactEffect[1], transform.position, Quaternion.identity);
            explosion.GetComponent<DamageSource>().damage = GetComponent<DamageSource>().damage / 2; //aoe does half the damage
            if (gameObject.tag == "AllyProjectile")
            {
                explosion.tag = "AllyProjectile";
            }
            else
            {
                explosion.tag = "EnemyProjectile";
            }
        }
        Destroy(gameObject);
    }
}
