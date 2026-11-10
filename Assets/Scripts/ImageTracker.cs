/*
* Author: Zhi hng
* Date: 8 November 2026
* Description: Script for image tracking
*/

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject arenaPrefab;

    public static GameObject ArenaInstance;

    private bool arenaSpawned = false;

    private void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
    }

    private void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
    }

    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach(var trackedImage in args.added)
        {
            TrySpawnArena(trackedImage);
        }
    }

    private void TrySpawnArena(ARTrackedImage trackedImage)
    {
        if(arenaSpawned)
            return;

        if(trackedImage.trackingState != TrackingState.Tracking)
            return;

        ArenaInstance = Instantiate(
            arenaPrefab,
            trackedImage.transform.position,
            trackedImage.transform.rotation
        );

        arenaSpawned = true;

        Debug.Log("Arena spawned");
    }
}
