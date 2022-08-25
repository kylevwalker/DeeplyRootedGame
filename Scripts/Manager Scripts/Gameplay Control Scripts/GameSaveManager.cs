using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    [Header("Referenced GameObjects")]
    public GameObject player;

    [Header("Debug")]
    public bool enableDebugMode;

    private int gameSaveIndex;

    //Add Player Position, Enemy Position/State, Story State, Inventory/HotSlot/Held Object Info, SubScenesLoaded, Weapon Use Timers, Object Enable States, 

    private void Start()
    {
        LoadGameSaveIndex();
        //Load Saved Data By Index
    }

    private void LoadGameSaveIndex()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string dataSavePath = Application.persistentDataPath + "/CurrentGameSaveIndex.Index";
        if (File.Exists(dataSavePath))
        {
            FileStream fileStream = new FileStream(dataSavePath, FileMode.Open);
            SceneLoadManager.SceneLoadIndexContainer sceneLoadIndexContainer = binaryFormatter.Deserialize(fileStream) as SceneLoadManager.SceneLoadIndexContainer;
            gameSaveIndex = sceneLoadIndexContainer.sceneLoadIndex;
            fileStream.Close();
            if (enableDebugMode)
            {
                print("Loaded Game Save Index: " + gameSaveIndex);
            }
            File.Delete(dataSavePath);
        }
        else if (enableDebugMode)
        {
            Debug.LogWarning("No Game Save Index Retrieved");
        }
    }

    public void deleteSave()
    {

    }

    [Serializable]
    public class SceneDataContainer
    {
        public InteractableObjectDataContainer[] dataContainers;
    }

    [Serializable]
    public class InteractableObjectDataContainer
    {
        public string objectReferenceName;
        public bool toggleState;
        public bool enableState;
        public bool savePosition;
        public float[] savedPosition;
        public float[] savedRotation;
        public bool actionless;
    }

    [Serializable]
    public class EnemyStateDataContainer
    {

    }

    [Serializable]
    public class PlayerStateAndInventoryDataContainer
    {

    }

    [Serializable]
    public class StoryStateDataContainer
    {

    }

    private static float[] ConvertVector3ToFloatArray(Vector3 convertableVector3)
    {
        float[] convertedVector3 = new float[3];
        convertedVector3[0] = convertableVector3.x;
        convertedVector3[1] = convertableVector3.y;
        convertedVector3[2] = convertableVector3.z;
        return convertedVector3;
    }

    [Serializable]
    public class RandomObjectDataContainer
    {
        public string ObjectReferenceName;
    }

    public void SaveGameData()
    {
        SceneDataContainer currentSceneDataContainer = new SceneDataContainer();


        foreach (GameObject sceneObject in FindObjectsOfType<GameObject>())
        {
            //Add More Object Save Data Exceptions
            //Interactable Objects
            if (sceneObject.GetComponent<InteractableItemController>() != null)
            {
                InteractableObjectDataContainer currentObjectDataContainer = new InteractableObjectDataContainer();
                //Save Object Data Exluding Position And Rotation
                if (sceneObject.GetComponent<InteractableItemController>().moveableObject || sceneObject.GetComponent<InteractableItemController>().pickupable)
                {

                }
                currentSceneDataContainer.dataContainers[currentSceneDataContainer.dataContainers.Length] = currentObjectDataContainer;
            }
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string dataSaveFileName = "GameSaveData" + gameSaveIndex;
        string dataSavePath = Application.persistentDataPath + "/" + dataSaveFileName + ".savedGameData";
        FileStream dataFileStream = new FileStream(dataSavePath, FileMode.Create);
        binaryFormatter.Serialize(dataFileStream, currentSceneDataContainer);
        dataFileStream.Close();
    }

    /*
    public void LoadGameSaveData()
    {
        string dataLoadFileName = "GameLoadData" + gameSaveIndex;
        string dataLoadPath = Application.persistentDataPath + "/" + dataLoadFileName + ".savedControlsData";
        if (File.Exists(dataLoadPath))
        {
            print("Loading Custom Controls");
            FileStream dataFileStream = new FileStream(dataLoadPath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            PlayerControlData loadedPlayerControlData = binaryFormatter.Deserialize(dataFileStream) as PlayerControlData;
            dataFileStream.Close();
            return loadedPlayerControlData;
        }
    }
    */
}
