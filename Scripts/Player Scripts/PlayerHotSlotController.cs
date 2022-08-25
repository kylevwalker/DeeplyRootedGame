using UnityEngine;

public class PlayerHotSlotController : MonoBehaviour
{
    [Header("Connected Game Objects")]
    public GameObject gameManager;

    [Header("Debugging")]
    public bool enableDebugMode;
    public bool allowObjectHidingOnDoubleSelect;

    [HideInInspector]
    public GameObject[] hotSlotObjects = new GameObject[3];
    [HideInInspector]
    public GameObject objectInHand;
    [HideInInspector]
    public int heldObjectIndex;
    private int mostRecentHotSlotIndex;
    [HideInInspector]
    public GameObject[] ghostHotSlotObjects = new GameObject[3];

    //METHOD INITIALIZATION AND UPDATE
    private void Start()
    {
        TakeOutObject(0);
    }
    private void Update()
    {
        TransformObjectInHand();
        if (!gameManager.GetComponent<InGamePauseMenuManager>().playerCurrentlyPaused && !gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().playerInInventory)
        {
            PlayerInputControls();
            HotBarScrollWheelController();
        }
        if (enableDebugMode)
        {
            print("Object In Hand : " + objectInHand + " | Held Object Index : " + heldObjectIndex + " | Objects In Hot Slots : " + hotSlotObjects);
        }
    }

    //DONE
    public void AssignHotSlot(GameObject objectToAssign, int hotSlotArrayIndex)
    {
        RemoveObjectFromHotSlotPosition(hotSlotArrayIndex, true);
        hotSlotObjects[hotSlotArrayIndex] = objectToAssign;
        if (objectToAssign != null)
        {
            ghostHotSlotObjects[hotSlotArrayIndex] = objectToAssign;
        }
        if (heldObjectIndex == hotSlotArrayIndex)
        {
            TakeOutObject(hotSlotArrayIndex);
        }
        else
        {
            objectToAssign.SetActive(false);
        }
    }

    //DONE
    public void RemoveObjectFromHotSlotPosition(int hotSlotArrayIndex, bool addToInventory)
    {
        GameObject objectToRemove = hotSlotObjects[hotSlotArrayIndex];
        if (objectToRemove != null)
        {
            if (objectInHand == hotSlotObjects[hotSlotArrayIndex])
            {
                PutObjectAway();
            }
            hotSlotObjects[hotSlotArrayIndex] = null;
            objectToRemove.GetComponent<Rigidbody>().useGravity = true;
            objectToRemove.SetActive(true);
            objectToRemove.GetComponent<InteractableItemController>().itemInInventory = addToInventory;
            objectToRemove.GetComponent<InteractableItemController>().enabled = true;
            if (addToInventory)
            {
                gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().AddToInventory(objectToRemove);
            }
        }
    }

    //DONE | ADD OBJECT TAKE OUT ANIMATION
    private void TakeOutObject(int objectToPutInHandIndex)
    {
        GameObject objectToPutInHand = hotSlotObjects[objectToPutInHandIndex];
        heldObjectIndex = objectToPutInHandIndex;
        if (objectInHand != objectToPutInHand)
        {
            PutObjectAway();
        }
        if (objectToPutInHand != null)
        {
            objectInHand = objectToPutInHand;
            objectInHand.GetComponent<Rigidbody>().useGravity = false;
            objectInHand.transform.SetParent(gameObject.GetComponentInChildren<Camera>().gameObject.transform);
            objectInHand.transform.localPosition = objectInHand.GetComponent<InteractableItemController>().holdPositionOffset;
            objectInHand.transform.localEulerAngles = objectInHand.GetComponent<InteractableItemController>().holdRotationOffset;
            objectInHand.SetActive(true);
            objectInHand.GetComponent<InteractableItemController>().objectInHand = true;
            objectInHand.GetComponent<InteractableItemController>().enabled = true;
        }
    }

    //DONE | ADD OBJECT USE ANIMATION
    private void TransformObjectInHand()
    {
        if (objectInHand != null)
        {
            objectInHand.transform.localPosition = objectInHand.GetComponent<InteractableItemController>().holdPositionOffset;
            objectInHand.transform.localEulerAngles = objectInHand.GetComponent<InteractableItemController>().holdRotationOffset;
        }
    }

    //DONE | ADD OBJECT PUT AWAY ANIMATION
    private void PutObjectAway()
    {
        if (objectInHand != null)
        {
            objectInHand.GetComponent<InteractableItemController>().objectInHand = false;
            objectInHand.SetActive(false);
            objectInHand.transform.SetParent(null);
            objectInHand = null;
        }
    }

    //DONE | ADD TEXT FADE
    private void PlayerInputControls()
    {
        if (objectInHand != null)
        {
            ObjectActions.ObjectUseController(objectInHand, gameManager, gameObject);
        }

        if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().hotslotOneKey))
        {
            SetCurrentHotSlot(0);
        }
        if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().hotslotTwoKey))
        {
            SetCurrentHotSlot(1);
        }
        if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().hotslotThreeKey))
        {
            SetCurrentHotSlot(2);
        }
    }

    //DONE
    private void SetCurrentHotSlot(int slotIndex)
    {
        if (heldObjectIndex != slotIndex)
        {
            mostRecentHotSlotIndex = slotIndex;
            TakeOutObject(slotIndex);
        }
        else if (allowObjectHidingOnDoubleSelect)
        {
            PutObjectAway();
            heldObjectIndex = -1;
        }
    }

    //DONE
    private void HotBarScrollWheelController()
    {
        if (!allowObjectHidingOnDoubleSelect)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                SetCurrentHotSlot(mostRecentHotSlotIndex == 2 ? 0 : mostRecentHotSlotIndex + 1);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                SetCurrentHotSlot(mostRecentHotSlotIndex == 0 ? 2 : mostRecentHotSlotIndex - 1);
            }
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (heldObjectIndex == -1)
                {
                    SetCurrentHotSlot(mostRecentHotSlotIndex == 2 ? 0 : mostRecentHotSlotIndex + 1);
                }
                else
                {
                    SetCurrentHotSlot(mostRecentHotSlotIndex);
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (heldObjectIndex == -1)
                {
                    SetCurrentHotSlot(mostRecentHotSlotIndex == 0 ? 2 : mostRecentHotSlotIndex - 1);
                }
                else
                {
                    SetCurrentHotSlot(mostRecentHotSlotIndex);
                }
            }
        }
    }
}
