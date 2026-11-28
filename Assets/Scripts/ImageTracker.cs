/*
* Author: Zhi hng
* Date: 28 November 2026
* Description: Script for image tracking
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager trackedImageManager;

    [SerializeField] GameObject arenaPrefab;

    public static GameObject ArenaInstance;
    BoxCollider deployZone;
    HashSet<string> deployedCards = new();
    bool arenaSpawned = false;
    [Tooltip("0: Game Scene, 1: Info Scene")]
    [SerializeField] int sceneType;

    int level = 0;

    void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
    }

    void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (ARTrackedImage image in args.added)
        {
            HandleTrackedImage(image);
        }

        foreach (ARTrackedImage image in args.updated)
        {
            HandleTrackedImage(image);
        }
    }

    void HandleTrackedImage(ARTrackedImage image)
    {
        string imageName = image.referenceImage.name;

        // Arena Marker

        if (sceneType == 0 && imageName == "Arena" && !arenaSpawned)
        {
            if (image.trackingState == TrackingState.Tracking)
            {
                ArenaInstance = Instantiate(arenaPrefab, image.transform.position, image.transform.rotation);

                arenaSpawned = true;
                GameManager.instance.GameStart(level);
                Debug.Log("Arena Spawned");
            }
            deployZone = ArenaInstance.transform.Find("AllySpawn").GetComponent<BoxCollider>();

            return;
        }

        if (image.trackingState != TrackingState.Tracking)
        {
            deployedCards.Remove(imageName);
            return;
        }

        if (sceneType == 0 && deployZone == null)
        {
            return;
        }

        if (!deployedCards.Contains(imageName))
        {
            if (sceneType == 0)
            {
                Vector3 localPos = deployZone.transform.InverseTransformPoint(image.transform.position);

                bool inDeployZone = Mathf.Abs(localPos.x) <= deployZone.size.x * 0.7f && Mathf.Abs(localPos.z) <= deployZone.size.z * 0.7f;
                if (inDeployZone)
                {
                    SpawnCardUnit(imageName, image.transform.position);

                    deployedCards.Add(imageName);
                }
            }
            else if (sceneType == 1)
            {
                
            }
        }
    }

    void SpawnCardUnit(string imageName, Vector3 cardPosition)
    {
        Vector3 spawnPosition = new Vector3(cardPosition.x, deployZone.bounds.center.y, cardPosition.z);
        switch (imageName)
        {
            case "B1":
                CardManager.instance.SpawnUnit(
                    "B1",
                    spawnPosition,
                    true
                );
                break;
            case "B2":
                CardManager.instance.SpawnUnit(
                    "B2",
                    spawnPosition,
                    true
                );
                break;
            case "I1":
                CardManager.instance.SpawnUnit(
                    "I1",
                    spawnPosition,
                    true
                );
                break;

            case "I2":
                CardManager.instance.SpawnUnit(
                    "I2",
                    spawnPosition,
                    true
                );
                break;

            case "I3":
                CardManager.instance.SpawnUnit(
                    "I3",
                    spawnPosition,
                    true
                );
                break;

            case "S1":
                CardManager.instance.SpawnUnit(
                    "S1",
                    spawnPosition,
                    true
                );
                break;

            case "S2":
                CardManager.instance.SpawnUnit(
                    "S2",
                    spawnPosition,
                    true
                );
                break;
        }
    }
}