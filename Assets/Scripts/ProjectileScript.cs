/*
* Author: Zhi hng
* Date: 10 November 2026
* Description: Script for the projectiles for both ally and enemy units.
*/

using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    [SerializeField] GameObject[] impactEffect = new GameObject[2]; //0 for normal, 1 for area damage
    Renderer projectileRenderer;
    bool isAreaDamage;
    int projectileDamage;
    Rigidbody rb;
    public void Initialize(Color color, bool isAreaDamage, bool isAllyProjectile, int damage)
    {
        projectileRenderer = GetComponent<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        projectileRenderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", color);
        projectileRenderer.SetPropertyBlock(block);

        this.isAreaDamage = isAreaDamage;
        if (isAllyProjectile)
        {
            rb.excludeLayers = LayerMask.GetMask("Ally");
        }
        else
        {
            rb.excludeLayers = LayerMask.GetMask("Enemy");
        }
        projectileDamage = damage;
    }

    void Start()
    {
        projectileRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isAreaDamage)
        {
            Instantiate(impactEffect[0], transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<NPCUnits>().TakeDamage(projectileDamage);
        }
        else
        {
            Instantiate(impactEffect[1], transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
