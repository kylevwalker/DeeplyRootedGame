using System;
using UnityEngine;

public class DumbwaiterController : MonoBehaviour
{
    [Header("Dumbwaiter Settings")]
    public float dumbwaiterUseInteractionLength;
    public float dumbdwaiterCallInteractionLength;

    [Header("Dumbwaiter Floors")]
    [HideInInspector]
    public int currentFloor;
    public DumbwaiterFloor[] dumbwaiterFloors;

    private void Start()
    {
        currentFloor = 1;
    }
}

[Serializable]
public struct DumbwaiterFloor
{
    public string floorName;
    public int floorIndex;
    public Vector3 playerSpawnLocation;
    public Vector3 playerSpawnRotation;
    public GameObject door;
}