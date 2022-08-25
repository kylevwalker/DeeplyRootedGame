using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class CustomPlayerControls : MonoBehaviour
{
    //Player Look Controls
    [Header("Player Look Controls")]
    public float playerMouseSensitivity;
    public bool invertMouseX;
    public bool invertMouseY;

    //Player Walk Controls
    [Header("Player Walk Controls")]
    public KeyCode playerForwardMovement;  
    public KeyCode playerBackMovement;     
    public KeyCode playerLeftMovement;     
    public KeyCode playerRightMovement;    

    //Player Sprint Controls
    [Header("Player Sprint Controls")]
    public KeyCode playerSprintKey;
    public bool holdToSprint;

    //Player Jump Controls
    [Header("Player Jump Controls")]
    public KeyCode playerJumpKey;

    //Player Crouch Controls
    [Header("Player Crouch Controls")]
    public KeyCode playerCrouchKey;
    public bool holdToCrouch;

    //Player Function Controls
    [Header("Player Interaction Controls")]
    public bool holdToGrab;
    public KeyCode playerItemInteractionKey;
    public KeyCode playerUseHeldItemKey;
    public KeyCode playerItemGrabKey;
    public KeyCode playerPauseKey;
    public KeyCode playerReloadObjectKey;

    //Player Crosshair Controls
    [Header("Player Crosshair Controls")]
    public bool crosshairAppearWhenStill;
    public bool crosshairAppearWhenMoving;

    //Player Inventory Controls
    [Header("Player Inventory Controls")]
    public KeyCode playerInventoryKey;
    public KeyCode inventoryGrabKey;
    public bool holdToGrabInventoryObjects;

    [Header("Player Hotslot Controls")]
    public KeyCode hotslotOneKey;
    public KeyCode hotslotTwoKey;
    public KeyCode hotslotThreeKey;

    [Header("Custom Editor Controls Settings")]
    public SettingsControlTextContainer[] customControlSettingsTextArray;
    [Tooltip("DISABLE ON EXPORT")]
    public bool clearCustomSettingsOnStart;
    public bool enableDebugMode;

    private void Start()
    {
        ClearControlSettings(clearCustomSettingsOnStart);
        LoadPlayerControls();
        UpdateSettingsText();
    }

    //DONE
    public void SensitivityValueIncrement(bool negativeIncrement)
    {
        playerMouseSensitivity += negativeIncrement ? playerMouseSensitivity > 10 ? -10 : 0 : playerMouseSensitivity < 1000 ? 10 : 0;
        SavePlayerControls();
        UpdateSettingsText();
    }

    //DONE
    public void AssignKeyFromButton(string keyName)
    {
        StartCoroutine(AssignKeyValue(keyName));
    }

    //DONE
    private IEnumerator AssignKeyValue(string keyName)
    {
        KeyCode initialKeyValue = KeyCode.None;
        void SwitchKeyValue(string keyToSwitchName, KeyCode insertedKeyValue, bool assignInitialValue)
        {
            switch (keyToSwitchName)
            {
                case "playerForwardMovement": if (assignInitialValue) { initialKeyValue = playerForwardMovement; } playerForwardMovement = insertedKeyValue; break;
                case "playerBackMovement": if (assignInitialValue) { initialKeyValue = playerBackMovement; } playerBackMovement = insertedKeyValue; break;
                case "playerLeftMovement": if (assignInitialValue) { initialKeyValue = playerLeftMovement; } playerLeftMovement = insertedKeyValue; break;
                case "playerRightMovement": if (assignInitialValue) { initialKeyValue = playerRightMovement; } playerRightMovement = insertedKeyValue; break;
                case "playerSprintKey": if (assignInitialValue) { initialKeyValue = playerSprintKey; } playerSprintKey = insertedKeyValue; break;
                case "playerJumpKey": if (assignInitialValue) { initialKeyValue = playerJumpKey; } playerJumpKey = insertedKeyValue; break;
                case "playerCrouchKey": if (assignInitialValue) { initialKeyValue = playerCrouchKey; } playerCrouchKey = insertedKeyValue; break;
                case "playerItemInteractionKey": if (assignInitialValue) { initialKeyValue = playerItemInteractionKey; } playerItemInteractionKey = insertedKeyValue; break;
                case "playerUseHeldItemKey": if (assignInitialValue) { initialKeyValue = playerUseHeldItemKey; } playerUseHeldItemKey = insertedKeyValue; break;
                case "playerItemGrabKey": if (assignInitialValue) { initialKeyValue = playerItemGrabKey; } playerItemGrabKey = insertedKeyValue; break;
                case "playerPauseKey": if (assignInitialValue) { initialKeyValue = playerPauseKey; } playerPauseKey = insertedKeyValue; break;
                case "playerReloadObjectKey": if (assignInitialValue) { initialKeyValue = playerReloadObjectKey; } playerReloadObjectKey = insertedKeyValue; break;
                case "playerInventoryKey": if (assignInitialValue) { initialKeyValue = playerInventoryKey; } playerInventoryKey = insertedKeyValue; break;
                case "inventoryGrabKey": if (assignInitialValue) { initialKeyValue = inventoryGrabKey; } inventoryGrabKey = insertedKeyValue; break;
                case "hotslotOneKey": if (assignInitialValue) { initialKeyValue = hotslotOneKey; } hotslotOneKey = insertedKeyValue; break;
                case "hotslotTwoKey": if (assignInitialValue) { initialKeyValue = hotslotTwoKey; } hotslotTwoKey = insertedKeyValue; break;
                case "hotslotThreeKey": if (assignInitialValue) { initialKeyValue = hotslotThreeKey; } hotslotThreeKey = insertedKeyValue; break;
            }
        }
        SwitchKeyValue(keyName, KeyCode.None, true);
        UpdateSettingsText();

        lookingForInput = true;
        KeyCode keyInput;
        for (; ; )
        {
            if (Input.anyKeyDown && currentKeyHit != KeyCode.None)
            {
                if (enableDebugMode)
                {
                    print("Key Hit");
                }
                if (currentKeyHit == playerPauseKey)
                {
                    keyInput = initialKeyValue;
                }
                else
                {
                    keyInput = currentKeyHit;
                }
                lookingForInput = false;
                break;
            }
            yield return null;
        }
        if (enableDebugMode)
        {
            print("Key Input : " + keyInput);
        }
        SwitchKeyValue(keyName, keyInput, false);
        SavePlayerControls();
        UpdateSettingsText();
        yield return null;
    }

    //DONE
    public void ToggleBoolValue(string boolName)
    {
        switch (boolName)
        {
            case "invertMouseX": invertMouseX = !invertMouseX; break;
            case "invertMouseY": invertMouseY = !invertMouseY; break;
            case "holdToSprint": holdToSprint = !holdToSprint; break;
            case "holdToCrouch": holdToCrouch = !holdToCrouch; break;
            case "holdToGrab": holdToGrab = !holdToGrab; break;
            case "crosshairAppearWhenStill": crosshairAppearWhenStill = !crosshairAppearWhenStill; break;
            case "crosshairAppearWhenMoving": crosshairAppearWhenMoving = !crosshairAppearWhenMoving; break;
            case "holdToGrabInventoryObjects": holdToGrabInventoryObjects = !holdToGrabInventoryObjects; break;
        }
        SavePlayerControls();
        UpdateSettingsText();
    }

    //DONE
    private KeyCode currentKeyHit;
    [HideInInspector]
    public bool lookingForInput;
    private void OnGUI()
    {
        Event currentEvent = Event.current;
        if (lookingForInput)
        {
            if ((currentEvent.isMouse || currentEvent.isKey) && (currentEvent.type == EventType.KeyDown || currentEvent.type == EventType.MouseDown) && currentEvent.keyCode != KeyCode.None)
            {
                if (enableDebugMode)
                {
                    print("Detected key code: " + currentEvent.keyCode);
                }
                currentKeyHit = currentEvent.keyCode;
            }
        }
    }

    //DONE
    public void UpdateSettingsText()
    {
        foreach (SettingsControlTextContainer currentControlSettingsTextContainer in customControlSettingsTextArray)
        {
            if (currentControlSettingsTextContainer.inUse)
            {
                switch (currentControlSettingsTextContainer.textReferenceName)
                {
                    case "playerMouseSensitivity": currentControlSettingsTextContainer.textToUpdate.text = playerMouseSensitivity.ToString(); break;
                    case "invertMouseX": currentControlSettingsTextContainer.textToUpdate.text = invertMouseX.ToString(); break;
                    case "invertMouseY": currentControlSettingsTextContainer.textToUpdate.text = invertMouseY.ToString(); break;
                    case "playerForwardMovement": currentControlSettingsTextContainer.textToUpdate.text = playerForwardMovement.ToString(); break;
                    case "playerBackMovement": currentControlSettingsTextContainer.textToUpdate.text = playerBackMovement.ToString(); break;
                    case "playerLeftMovement": currentControlSettingsTextContainer.textToUpdate.text = playerLeftMovement.ToString(); break;
                    case "playerRightMovement": currentControlSettingsTextContainer.textToUpdate.text = playerRightMovement.ToString(); break;
                    case "playerSprintKey": currentControlSettingsTextContainer.textToUpdate.text = playerSprintKey.ToString(); break;
                    case "holdToSprint": currentControlSettingsTextContainer.textToUpdate.text = holdToSprint.ToString(); break;
                    case "playerJumpKey": currentControlSettingsTextContainer.textToUpdate.text = playerJumpKey.ToString(); break;
                    case "playerCrouchKey": currentControlSettingsTextContainer.textToUpdate.text = playerCrouchKey.ToString(); break;
                    case "holdToCrouch": currentControlSettingsTextContainer.textToUpdate.text = holdToCrouch.ToString(); break;
                    case "holdToGrab ": currentControlSettingsTextContainer.textToUpdate.text = holdToGrab.ToString(); break;
                    case "playerItemInteractionKey": currentControlSettingsTextContainer.textToUpdate.text = playerItemInteractionKey.ToString(); break;
                    case "playerUseHeldItemKey": currentControlSettingsTextContainer.textToUpdate.text = playerUseHeldItemKey.ToString(); break;
                    case "playerItemGrabKey": currentControlSettingsTextContainer.textToUpdate.text = playerItemGrabKey.ToString(); break;
                    case "playerPauseKey": currentControlSettingsTextContainer.textToUpdate.text = playerPauseKey.ToString(); break;
                    case "playerReloadObjectKey": currentControlSettingsTextContainer.textToUpdate.text = playerReloadObjectKey.ToString(); break;
                    case "crosshairAppearWhenStill": currentControlSettingsTextContainer.textToUpdate.text = crosshairAppearWhenStill.ToString(); break;
                    case "crosshairAppearWhenMoving": currentControlSettingsTextContainer.textToUpdate.text = crosshairAppearWhenMoving.ToString(); break;
                    case "playerInventoryKey": currentControlSettingsTextContainer.textToUpdate.text = playerInventoryKey.ToString(); break;
                    case "inventoryGrabKey": currentControlSettingsTextContainer.textToUpdate.text = inventoryGrabKey.ToString(); break;
                    case "holdToGrabInventoryObjects": currentControlSettingsTextContainer.textToUpdate.text = holdToGrabInventoryObjects.ToString(); break;
                    case "hotslotOneKey": currentControlSettingsTextContainer.textToUpdate.text = hotslotOneKey.ToString(); break;
                    case "hotslotTwoKey": currentControlSettingsTextContainer.textToUpdate.text = hotslotTwoKey.ToString(); break;
                    case "hotslotThreeKey": currentControlSettingsTextContainer.textToUpdate.text = hotslotThreeKey.ToString(); break;
                }
            }
        }
    }

    //DONE
    [Serializable]
    public struct SettingsControlTextContainer
    {
        public string textReferenceName;
        public Text textToUpdate;
        public bool inUse;
    }

    //DONE
    private void SavePlayerControls()
    {
        PlayerControlDataManagementSystem.SavePlayerControlData(this);
        if (enableDebugMode)
        {
            print("Controls Saved");
        }
    }

    //DONE
    private void LoadPlayerControls()
    {
        PlayerControlData loadedPlayerControlData = PlayerControlDataManagementSystem.LoadPlayerControlData();
        if (loadedPlayerControlData == null)
        {
            return;
        }
        playerMouseSensitivity = loadedPlayerControlData.playerMouseSensitivity;
        invertMouseX = loadedPlayerControlData.invertMouseX;
        invertMouseY = loadedPlayerControlData.invertMouseY;
        playerForwardMovement = ConvertStringToKeyCode(loadedPlayerControlData.playerForwardMovement);
        playerBackMovement = ConvertStringToKeyCode(loadedPlayerControlData.playerBackMovement);
        playerLeftMovement = ConvertStringToKeyCode(loadedPlayerControlData.objectRotateLeftKey);
        playerRightMovement = ConvertStringToKeyCode(loadedPlayerControlData.objectRotateRightKey);
        playerSprintKey = ConvertStringToKeyCode(loadedPlayerControlData.playerSprintKey);
        holdToSprint = loadedPlayerControlData.holdToSprint;
        playerJumpKey = ConvertStringToKeyCode(loadedPlayerControlData.playerJumpKey);
        playerCrouchKey = ConvertStringToKeyCode(loadedPlayerControlData.playerCrouchKey);
        holdToCrouch = loadedPlayerControlData.holdToCrouch;
        holdToGrab = loadedPlayerControlData.holdToGrab;
        playerItemInteractionKey = ConvertStringToKeyCode(loadedPlayerControlData.playerItemInteractionKey);
        playerUseHeldItemKey = ConvertStringToKeyCode(loadedPlayerControlData.playerUseHeldItemKey);
        playerItemGrabKey = ConvertStringToKeyCode(loadedPlayerControlData.playerItemGrabKey);
        playerPauseKey = ConvertStringToKeyCode(loadedPlayerControlData.playerPauseKey);
        playerReloadObjectKey = ConvertStringToKeyCode(loadedPlayerControlData.playerReloadObjectKey);
        crosshairAppearWhenStill = loadedPlayerControlData.crosshairAppearWhenStill;
        crosshairAppearWhenMoving = loadedPlayerControlData.crosshairAppearWhenMoving;
        playerInventoryKey = ConvertStringToKeyCode(loadedPlayerControlData.playerInventoryKey);
        inventoryGrabKey = ConvertStringToKeyCode(loadedPlayerControlData.inventoryGrabKey);
        holdToGrabInventoryObjects = loadedPlayerControlData.holdToGrabInventoryObjects;
        hotslotOneKey = ConvertStringToKeyCode(loadedPlayerControlData.hotslotOneKey);
        hotslotTwoKey = ConvertStringToKeyCode(loadedPlayerControlData.hotslotTwoKey);
        hotslotThreeKey = ConvertStringToKeyCode(loadedPlayerControlData.hotslotThreeKey);
    }

    //DONE
    [Serializable]
    public class PlayerControlData
    {
        public float playerMouseSensitivity;
        public bool invertMouseX;
        public bool invertMouseY;

        public string playerForwardMovement;    //Convert to Keycode
        public string playerBackMovement;       //Convert to Keycode
        public string playerLeftMovement;       //Convert to Keycode
        public string playerRightMovement;      //Convert to Keycode

        public string playerSprintKey;         //Convert to Keycode
        public bool holdToSprint;

        public string playerJumpKey;           //Convert to Keycode

        public string playerCrouchKey;         //Convert to Keycode
        public bool holdToCrouch;

        public bool holdToGrab;
        public string playerItemInteractionKey;//Convert to Keycode
        public string playerUseHeldItemKey;    //Convert to Keycode
        public string playerDropHeldItemKey;   //Convert to Keycode
        public string playerItemGrabKey;       //Convert to Keycode
        public string playerPauseKey;          //Convert to Keycode
        public string playerReloadObjectKey;   //Convert to Keycode
        public string objectRotateLeftKey;     //Convert to Keycode
        public string objectRotateRightKey;    //Convert to Keycode

        public bool crosshairAppearWhenStill;
        public bool crosshairAppearWhenMoving;

        public string playerInventoryKey;      //Convert to Keycode
        public string inventoryGrabKey;        //Convert to Keycode
        public bool holdToGrabInventoryObjects;

        public string hotslotOneKey;           //Convert to Keycode
        public string hotslotTwoKey;           //Convert to Keycode
        public string hotslotThreeKey;         //Convert to Keycode

        public PlayerControlData(CustomPlayerControls customPlayerControls)
        {
            playerMouseSensitivity = customPlayerControls.playerMouseSensitivity;
            invertMouseX = customPlayerControls.invertMouseX;
            invertMouseY = customPlayerControls.invertMouseY;

            playerForwardMovement = customPlayerControls.playerForwardMovement.ToString();      //Convert to Keycode
            playerBackMovement = customPlayerControls.playerBackMovement.ToString();            //Convert to Keycode
            playerLeftMovement = customPlayerControls.playerLeftMovement.ToString();            //Convert to Keycode
            playerRightMovement = customPlayerControls.playerRightMovement.ToString();          //Convert to Keycode

            playerSprintKey = customPlayerControls.playerSprintKey.ToString();                  //Convert to Keycode
            holdToSprint = customPlayerControls.holdToSprint;

            playerJumpKey = customPlayerControls.playerJumpKey.ToString();                      //Convert to Keycode

            playerCrouchKey = customPlayerControls.playerCrouchKey.ToString();                  //Convert to Keycode
            holdToCrouch = customPlayerControls.holdToCrouch;

            holdToGrab = customPlayerControls.holdToGrab;
            playerItemInteractionKey = customPlayerControls.playerItemInteractionKey.ToString();//Convert to Keycode
            playerUseHeldItemKey = customPlayerControls.playerUseHeldItemKey.ToString();        //Convert to Keycode
            playerItemGrabKey = customPlayerControls.playerItemGrabKey.ToString();              //Convert to Keycode
            playerPauseKey = customPlayerControls.playerPauseKey.ToString();                    //Convert to Keycode

            crosshairAppearWhenStill = customPlayerControls.crosshairAppearWhenStill;
            crosshairAppearWhenMoving = customPlayerControls.crosshairAppearWhenMoving;

            playerInventoryKey = customPlayerControls.inventoryGrabKey.ToString();              //Convert to Keycode
            inventoryGrabKey = customPlayerControls.inventoryGrabKey.ToString();                //Convert to Keycode
            holdToGrabInventoryObjects = customPlayerControls.holdToGrabInventoryObjects;

            hotslotOneKey = customPlayerControls.hotslotOneKey.ToString();                      //Convert to Keycode
            hotslotTwoKey = customPlayerControls.hotslotTwoKey.ToString();                      //Convert to Keycode
            hotslotThreeKey = customPlayerControls.hotslotThreeKey.ToString();                  //Convert to Keycode
        }
    }

    //DONE
    public static class PlayerControlDataManagementSystem
    {
        public static bool enableDebugForDataLoad = false;
        public static void SavePlayerControlData(CustomPlayerControls customPlayerControls)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string dataSavePath = Application.persistentDataPath + "/customPlayerControls.savedControlsData";
            FileStream dataFileStream = new FileStream(dataSavePath, FileMode.Create);
            PlayerControlData playerControlData = new PlayerControlData(customPlayerControls);
            binaryFormatter.Serialize(dataFileStream, playerControlData);
            dataFileStream.Close();
        }

        public static PlayerControlData LoadPlayerControlData()
        {
            string dataLoadPath = Application.persistentDataPath + "/customPlayerControls.savedControlsData";
            if (File.Exists(dataLoadPath))
            {
                if (enableDebugForDataLoad)
                {
                    print("Loading Custom Controls");
                }
                FileStream dataFileStream = new FileStream(dataLoadPath, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                PlayerControlData loadedPlayerControlData = binaryFormatter.Deserialize(dataFileStream) as PlayerControlData;
                dataFileStream.Close();
                return loadedPlayerControlData;
            }
            else
            {
                if (enableDebugForDataLoad)
                {
                    Debug.LogWarning("No Existing Player Control Save Data, Using Default Controls");
                }
                return null;
            }
        }
    }

    //DONE
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));
    private KeyCode ConvertStringToKeyCode(string convertableString)
    {
        foreach (KeyCode keyToCheck in keyCodes)
        {
            if (keyToCheck.ToString() == convertableString)
            {
                return keyToCheck;
            }
        }
        if (enableDebugMode)
        {
            Debug.LogError("No Key Hit");
        }
        return KeyCode.None;
    }

    //DONE
    private void ClearControlSettings(bool ClearCustomSettingsOnStart)
    {
        if (ClearCustomSettingsOnStart)
        {
            string dataLoadPath = Application.persistentDataPath + "/customPlayerControls.savedControlsData";
            if (File.Exists(dataLoadPath))
            {
                File.Delete(dataLoadPath);
                if (enableDebugMode)
                {
                    Debug.Log("Custom Control Data Cleared");
                }
            }
            else
            {
                if (enableDebugMode)
                {
                    Debug.Log("No Existing Files In Custom Data Path To Clear");
                }
            }
        }
    }
}
