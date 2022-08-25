using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DevTool : MonoBehaviour
{
    [Header("Referenced Gameobjects")]
    public Light playerSpotLight;

    [Header("Cheats")]
    public bool enablePlayerSpotLight;

    [Header("Add Prefabs To Inventory On Spawn")]
    public Prefab[] objectsToAdd;
    private GameObject gameManager;

    void Start()
    {
        playerSpotLight.enabled = enablePlayerSpotLight;
        gameManager = FindObjectOfType<GameControlsManager>().gameObject;
        foreach (Prefab prefabToCreate in objectsToAdd)
        {
            for (int count = 0; count < prefabToCreate.prefabCount; count++)
            {
                GameObject newObject = Instantiate(prefabToCreate.prefab);
                gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().AddToInventory(newObject); 
            }
        }
    }

    [System.Serializable]
    public struct Prefab
    {
        public string objectName;
        public GameObject prefab;
        public int prefabCount;
    }
}