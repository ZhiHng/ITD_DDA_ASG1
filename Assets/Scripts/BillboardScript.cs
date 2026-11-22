/*
* Author: Zhi hng
* Date: 22 November 2026
* Description: Script for objects that will always face the camera, used for health bars and other UI elements.
*/

using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Camera arCamera;

    private void Awake()
    {
        arCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(arCamera.transform);
        transform.Rotate(0f, 180f, 0f);
    }
}
