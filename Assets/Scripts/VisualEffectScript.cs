/*
* Author: Zhi hng
* Date: 27 November 2026
* Description: Script for visual effects for projectiles, death and to destroy itself after a certain time.
*/

using System.Collections;
using UnityEngine;

public class VisualEffectScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent<Collider>(out Collider collider);
        if (collider != null)
        {
            StartCoroutine(DisableCollider(collider));
        }
        Destroy(gameObject, 1f);
    }
    IEnumerator DisableCollider(Collider collider)
    {
        yield return new WaitForSeconds(0.1f);
        collider.enabled = false;
    }
}
