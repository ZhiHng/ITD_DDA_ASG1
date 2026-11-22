/*
* Author: Zhi hng
* Date: 22 November 2026
* Description: Script for visual effects for projectiles, death and to destroy itself after a certain time.
*/

using UnityEngine;

public class VisualEffectScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 1f);
    }
}
