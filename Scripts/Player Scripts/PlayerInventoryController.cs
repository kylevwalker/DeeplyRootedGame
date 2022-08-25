using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour
{
    [Header("Connected Game Objects")]
    public GameObject gameManager;
    public Text inventoryObjectGrabText;
    public Canvas inventoryHUDCanvas;
    public Camera inventoryCamera;
    public Camera playerCamera;
    public GameObject player;

    [Header("Inventory Drop Settings")]
    public float objectPickupVerticalRandomOffsetRangeX;
    public float objectPickupVerticalRandomOffsetRangeY;
    public float objectSpawnVerticalDropOffset;

    [Header("Inventory Grab Position Settings")]
    public float inventoryObjectHoldHeight;
    public float grabbedObjectPositioningResetTolerance;
    public AnimationCurve grabbedObjectPositionMoveSpeedCurve;
    [Range(1, 50)]
    public float grabbedObjectPositionMoveSpeedMultiplier;
    public float grabbedObjectPositionPhysicsPreservationVectorMultiplier;



    [Header("Inventory Grab Rotation Settings")]
    public float grabbedObjectRotationResetTolerance;
    public AnimationCurve grabbedObjectRotationSpeedCurve;
    [Range(1, 10000)]
    public float grabbedObjectRotationSpeedMultiplier;



    [Header("Debugging")]
    public bool enableDebugMode;

    [HideInInspector]
    public GameObject[] objectsInInventory;
    private GameObject interactableItem;
    [HideInInspector]
    public GameObject grabbedGameObject;
    private LayerMask interactionlayer;
    private char inventoryGrabKeyCharacter;
    [HideInInspector]
    public bool playerPaused;
    [HideInInspector]
    public bool playerInInventory;
    private bool objectBeingGrabbed;
    private bool textTransparent;
    private bool overPocket;
    private float inventoryTextOutlineFadeIncrement;
    private float interactionRayLength;
    private int potentialPocketIndex;
    private string objectGrabCharacterString;

    //METHOD INITIALIZATION AND UPDATE
    private void Start()
    {
        playerPaused = false;
        ExitInventory();
        textTransparent = true;
        UpdateInputCharacters();
        playerInInventory = false;
        objectBeingGrabbed = false;
        grabbedGameObject = null;
        interactionlayer = gameManager.GetComponent<GameControlsManager>().interactableItemLayer;
        interactionRayLength = gameManager.GetComponent<GameControlsManager>().interactionRayLength;
        inventoryTextOutlineFadeIncrement = gameManager.GetComponent<GameControlsManager>().inventoryTextOutlineTransparency / (inventoryObjectGrabText.color.a / gameManager.GetComponent<GameControlsManager>().inventoryTextIncrementValue);
        objectGrabCharacterString = "";
    }

    private void Update()
    {
        //print("Inventory Object Count : " + objectsInInventory.Length);
        PlayerInventoryAccess();
        playerPaused = gameManager.GetComponent<InGamePauseMenuManager>().playerCurrentlyPaused;
        InventoryInteractionController();
        InventoryInteractionTextController();
        ConfigureObject();
        ObjectGrabController();
        InventoryObjectFallCheck();
        if (enableDebugMode)
            print("Over Pocket : " + overPocket + " | Potential Pocket Index : " + potentialPocketIndex + " | Interactable Object : " + interactableItem + " | Object Being Grabbed : " + objectBeingGrabbed + " | Grabbed Object : " + grabbedGameObject);
    }

    //DONE
    public void UpdateInputCharacters()
    {
        inventoryGrabKeyCharacter = (char)gameManager.GetComponent<CustomPlayerControls>().inventoryGrabKey;
    }

    //DONE
    public void AddToInventory(GameObject objectToAdd)
    {
        //print("Added");
        Vector3 dropoffset = new Vector3(UnityEngine.Random.Range(-objectPickupVerticalRandomOffsetRangeX, objectPickupVerticalRandomOffsetRangeX), objectSpawnVerticalDropOffset, UnityEngine.Random.Range(-objectPickupVerticalRandomOffsetRangeY, objectPickupVerticalRandomOffsetRangeY));
        objectToAdd.GetComponent<Rigidbody>().velocity = Vector3.zero;
        objectToAdd.transform.position = gameObject.transform.position + dropoffset;
        objectToAdd.GetComponent<Rigidbody>().useGravity = true;
        objectToAdd.GetComponent<InteractableItemController>().itemInInventory = true;
        objectToAdd.GetComponent<InteractableItemController>().enabled = true;

        bool newToInventory = true;
        foreach (GameObject objectInInventory in objectsInInventory)
        {
            if (objectToAdd == objectInInventory)
            {
                newToInventory = false;
                break;
            }
        }

        //GHOST HOT SLOT PICKUP
        if (newToInventory)
        {
            int ghostSlotIndex = 0;
            foreach (GameObject objectInGhostSlot in player.GetComponent<PlayerHotSlotController>().ghostHotSlotObjects)
            {
                if (gameObject == objectInGhostSlot)
                {
                    player.GetComponent<PlayerHotSlotController>().hotSlotObjects[ghostSlotIndex] = gameObject;
                }
                ghostSlotIndex++;
            }

            var newInventoryArray = new GameObject[objectsInInventory.Length + 1];
            Array.Copy(objectsInInventory, newInventoryArray, objectsInInventory.Length);
            newInventoryArray[newInventoryArray.Length - 1] = objectToAdd;
            objectsInInventory = newInventoryArray;

            //print("Object Added | Inventory Object Count : " + objectsInInventory.Length);
        }
    }

    //DONE
    private void PlayerInventoryAccess()
    {
        if (playerInInventory)
        {
            if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerInventoryKey))
            {
                ExitInventory();
            }
        }
        else if (!playerPaused)
        {
            if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerInventoryKey))
            {
                EnterInventory();
            }
        }
    }

    //DONE
    private void UpdateValues(bool state)
    {
        inventoryHUDCanvas.enabled = state;
        playerInInventory = state;
        gameObject.GetComponentInChildren<Camera>().enabled = state;
        playerCamera.enabled = !state;
        player.GetComponent<PlayerMovement>().enabled = !state;
        player.GetComponentInChildren<PlayerLook>().enabled = !state;
        player.GetComponentInChildren<PlayerHeadBobbing>().enabled = !state;
    }

    //DONE
    public void ExitInventory()
    {
        UpdateValues(false);
        ReleaseObject();
        gameManager.GetComponent<CursorManager>().fullCursorDisable();
    }

    //DONE
    public void EnterInventory()
    {
        UpdateValues(true);
        ReleaseObject();
        gameManager.GetComponent<CursorManager>().enableConfinedCursor();
    }

    //DONE
    private void InventoryInteractionController()
    {
        if (playerInInventory)
        {
            Vector3 cursorPosition = Input.mousePosition;
            cursorPosition.z = inventoryCamera.nearClipPlane;
            Vector3 rayOrigin = inventoryCamera.ScreenToWorldPoint(cursorPosition);
            cursorPosition.z = inventoryCamera.farClipPlane;
            Vector3 reachPosition = inventoryCamera.ScreenToWorldPoint(cursorPosition);
            Vector3 rayDirection = reachPosition - rayOrigin;
            rayDirection.Normalize();
            if (enableDebugMode)
            {
                Debug.DrawRay(rayOrigin, rayDirection * interactionRayLength, Color.red);
            }

            RaycastHit[] interactionRaycastHits = Physics.RaycastAll(rayOrigin, rayDirection, interactionRayLength, ~interactionlayer);

            GameObject currentClosestObject = null;

            float closestinteractableItemDistance = interactionRayLength;

            bool FirstValueLarger(float valueOne, float ValueTwo)
            {
                return valueOne > ValueTwo;
            }

            if (interactionRaycastHits.Length > 0)
            {
                bool currentlyDetectedPocket = false;
                foreach (RaycastHit hitObject in interactionRaycastHits)
                {
                    float hitDistance = hitObject.distance;
                    GameObject hitGameObject = hitObject.collider.gameObject;
                    GameObject rootHitGameObject = hitGameObject.transform.gameObject;

                    if (rootHitGameObject.GetComponent<InteractableItemController>() != null)
                    {
                        rootHitGameObject = hitGameObject;
                        hitGameObject = rootHitGameObject;
                    }

                    if (hitGameObject.CompareTag("Pocket"))
                    {
                        currentlyDetectedPocket = true;
                        potentialPocketIndex = hitGameObject.GetComponentInParent<InventoryPocketContainer>().pocketIndex;
                    }
                    else if (!hitGameObject.CompareTag("Inventory Bag"))
                    {
                        if (FirstValueLarger(closestinteractableItemDistance, hitDistance))
                        {
                            closestinteractableItemDistance = hitDistance;
                            currentClosestObject = rootHitGameObject;
                        }
                    }
                }
                overPocket = currentlyDetectedPocket;
                if (currentClosestObject != null && currentClosestObject)
                {
                    print(interactableItem);
                    interactableItem = currentClosestObject.transform.root.gameObject;
                }
                else
                {
                    interactableItem = currentClosestObject;
                }
            }
            else
            {
                interactableItem = null;
                overPocket = false;
            }
        }
    }

    //DONE
    private bool holdGrabReset = true;
    private void ObjectGrabController()
    {
        if (playerInInventory)
        {
            if (gameManager.GetComponent<CustomPlayerControls>().holdToGrabInventoryObjects)
            {
                if (!holdGrabReset && Input.GetKeyUp(gameManager.GetComponent<CustomPlayerControls>().inventoryGrabKey))
                {
                    holdGrabReset = true;
                }
                if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().inventoryGrabKey) && overPocket && player.GetComponent<PlayerHotSlotController>().hotSlotObjects[potentialPocketIndex] != null)
                {
                    holdGrabReset = false;
                    player.GetComponent<PlayerHotSlotController>().RemoveObjectFromHotSlotPosition(potentialPocketIndex, true);
                }
                else if (Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().inventoryGrabKey) && holdGrabReset)
                {
                    if (!objectBeingGrabbed && interactableItem != null)
                    {
                        SetupGrabbedInventoryObject();
                    }
                }
                else
                {
                    if (objectBeingGrabbed && overPocket && grabbedGameObject.GetComponent<InteractableItemController>().equippable)
                    {
                        player.GetComponent<PlayerHotSlotController>().AssignHotSlot(grabbedGameObject, potentialPocketIndex);
                        interactableItem = null;
                    }
                    ReleaseObject();
                }
            }
            else
            {
                if (objectBeingGrabbed)
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().inventoryGrabKey))
                    {
                        if (overPocket && grabbedGameObject.GetComponent<InteractableItemController>().equippable)
                        {
                            player.GetComponent<PlayerHotSlotController>().AssignHotSlot(grabbedGameObject, potentialPocketIndex);
                        }
                        ReleaseObject();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().inventoryGrabKey))
                    {
                        if (overPocket && player.GetComponent<PlayerHotSlotController>().hotSlotObjects[potentialPocketIndex] != null)
                        {
                            player.GetComponent<PlayerHotSlotController>().RemoveObjectFromHotSlotPosition(potentialPocketIndex, true);
                        }
                        else if (interactableItem != null)
                        {
                            SetupGrabbedInventoryObject();
                        }
                    }
                }
            }
        }
    }

    //DONE
    private void SetupGrabbedInventoryObject()
    {
        grabbedGameObject = interactableItem;
        grabbedGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        objectBeingGrabbed = true;
    }

    //DONE
    private void InventoryInteractionTextController()
    {
        if (interactableItem != null || objectBeingGrabbed || (overPocket && player.GetComponent<PlayerHotSlotController>().hotSlotObjects[potentialPocketIndex] != null))
        {
            if (objectBeingGrabbed)
            {
                if (overPocket && grabbedGameObject.GetComponent<InteractableItemController>().equippable)
                {
                    objectGrabCharacterString = inventoryGrabKeyCharacter + " | Assign " + grabbedGameObject.GetComponent<InteractableItemController>().interactionName + " To Slot " + (potentialPocketIndex + 1);
                }
                else
                {
                    print(grabbedGameObject.GetComponent<InteractableItemController>().interactionName);
                    objectGrabCharacterString = inventoryGrabKeyCharacter + " | Drop " + grabbedGameObject.GetComponent<InteractableItemController>().interactionName;
                }
            }
            else if (interactableItem != null)
            {
                try
                {
                    objectGrabCharacterString = inventoryGrabKeyCharacter + " | Grab " + interactableItem.GetComponent<InteractableItemController>().interactionName;
                }
                catch { }
            }
            else if (overPocket && player.GetComponent<PlayerHotSlotController>().hotSlotObjects[potentialPocketIndex] != null)
            {
                objectGrabCharacterString = inventoryGrabKeyCharacter + " | Remove " + player.GetComponent<PlayerHotSlotController>().hotSlotObjects[potentialPocketIndex].GetComponent<InteractableItemController>().interactionName + " From Slot " + (potentialPocketIndex + 1);
            }
            inventoryObjectGrabText.text = objectGrabCharacterString;
            StartCoroutine(InteractionTextFadeIn());
        }
        else
        {
            StartCoroutine(InteractionTextFadeOut());
            if (textTransparent)
            {
                inventoryObjectGrabText.text = "";
            }
        }
    }

    //DONE
    private IEnumerator InteractionTextFadeIn()
    {
        Color newTextColor = inventoryObjectGrabText.color;
        Color newEffectColor = inventoryObjectGrabText.GetComponent<Outline>().effectColor;
        if (inventoryObjectGrabText.color.a < gameManager.GetComponent<GameControlsManager>().minInventoryTextTransparency)
        {
            textTransparent = true;
            newTextColor.a += gameManager.GetComponent<GameControlsManager>().inventoryTextIncrementValue;
            newEffectColor.a += inventoryTextOutlineFadeIncrement;
            inventoryObjectGrabText.color = newTextColor;
            inventoryObjectGrabText.GetComponent<Outline>().effectColor = newEffectColor;
            yield return new WaitForSecondsRealtime(1 / gameManager.GetComponent<GameControlsManager>().inventoryTextRefreshRate * Time.deltaTime);
        }
        textTransparent = false;
        yield return null;
    }

    //DONE
    private IEnumerator InteractionTextFadeOut()
    {
        Color newTextColor = inventoryObjectGrabText.color;
        Color newEffectColor = inventoryObjectGrabText.GetComponent<Outline>().effectColor;
        if (inventoryObjectGrabText.color.a > 0)
        {
            textTransparent = false;
            newTextColor.a -= gameManager.GetComponent<GameControlsManager>().inventoryTextIncrementValue;
            newEffectColor.a -= inventoryTextOutlineFadeIncrement;
            inventoryObjectGrabText.color = newTextColor;
            inventoryObjectGrabText.GetComponent<Outline>().effectColor = newEffectColor;
            yield return new WaitForSecondsRealtime(1 / gameManager.GetComponent<GameControlsManager>().inventoryTextRefreshRate * Time.deltaTime);
        }
        textTransparent = true;
        yield return null;
    }

    //DONE
    private Vector3 previousObjectMoveVector;
    private void ConfigureObject()
    {
        if (objectBeingGrabbed)
        {

            //MOVEMENT RESET

            grabbedGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            grabbedGameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            grabbedGameObject.GetComponent<Rigidbody>().useGravity = false;

            //POSITION DRIFTING

            Vector3 cursorPosition = Input.mousePosition;
            cursorPosition.z = inventoryCamera.nearClipPlane;
            Vector3 rayOrigin = inventoryCamera.ScreenToWorldPoint(cursorPosition);
            cursorPosition.z = inventoryCamera.farClipPlane;
            Vector3 reachPosition = inventoryCamera.ScreenToWorldPoint(cursorPosition);
            Vector3 transformDirection = reachPosition - rayOrigin;
            transformDirection.Normalize();

            float objectHeightDifference = inventoryCamera.transform.position.y - gameObject.transform.position.y - inventoryObjectHoldHeight;
            float objectTopDownDisplacement = new Vector3(grabbedGameObject.transform.position.x - inventoryCamera.transform.position.x, 0, grabbedGameObject.transform.position.z - inventoryCamera.transform.position.z).magnitude;
            float transformMagnitude = Mathf.Sqrt(Mathf.Pow(objectHeightDifference, 2) + Mathf.Pow(objectTopDownDisplacement, 2));

            Vector3 objectPositionDestination = inventoryCamera.transform.position + transformDirection * transformMagnitude;
            Vector3 grabbedObjectPositionOffset = objectPositionDestination - grabbedGameObject.transform.position;
            float grabbedObjectPositionOffsetMagnitude = Mathf.Abs(grabbedObjectPositionOffset.magnitude);

            if (grabbedObjectPositionOffsetMagnitude > grabbedObjectPositioningResetTolerance)
            {
                Vector3 objectMoveVector = Time.deltaTime * grabbedObjectPositionOffset * grabbedObjectPositionMoveSpeedCurve.Evaluate(grabbedObjectPositionOffsetMagnitude) * grabbedObjectPositionMoveSpeedMultiplier;
                grabbedGameObject.transform.position += objectMoveVector;
                previousObjectMoveVector = objectMoveVector * grabbedObjectPositionPhysicsPreservationVectorMultiplier;
                previousObjectMoveVector.y = 0;
            }
            else
            {
                grabbedGameObject.transform.position = objectPositionDestination;
                previousObjectMoveVector = Vector3.zero;
            }

            //ROTATION DRIFTING

            Quaternion currentObjectRotation = grabbedGameObject.transform.rotation;
            Vector3 objectRotationTargetEulers = grabbedGameObject.GetComponent<InteractableItemController>().inventoryHoldRotationOffset;
            Quaternion objectRotationTarget = Quaternion.Euler(objectRotationTargetEulers);
            float objectRotationOffsetMagnitude = (objectRotationTarget * Quaternion.Inverse(currentObjectRotation)).eulerAngles.magnitude;
            float objectRotationSpeed = grabbedObjectRotationSpeedCurve.Evaluate(Mathf.Abs(objectRotationOffsetMagnitude)) * grabbedObjectRotationSpeedMultiplier * Time.deltaTime / 1080;
            grabbedGameObject.transform.rotation = Quaternion.Slerp(currentObjectRotation, objectRotationTarget, objectRotationSpeed);
        }
    }

    //DONE
    public void ReleaseObject()
    {
        if (objectBeingGrabbed)
        {
            grabbedGameObject.GetComponent<Rigidbody>().useGravity = true;

            grabbedGameObject.GetComponent<Rigidbody>().velocity = previousObjectMoveVector * gameManager.GetComponent<GameControlsManager>().objectReleaseVectorScale * Time.deltaTime;
        }

        grabbedGameObject = null;
        interactableItem = null;
        objectBeingGrabbed = false;
    }

    //DONE
    private void InventoryObjectFallCheck()
    {
        foreach (GameObject objectToCheck in objectsInInventory)
        {
            if (!objectToCheck.GetComponent<InteractableItemController>().objectInHand)
            {
                if (objectToCheck.transform.position.y < gameManager.GetComponent<GameControlsManager>().playerInventoryBag.transform.position.y - gameManager.GetComponent<GameControlsManager>().objectDropYOffsetCheck)
                {
                    if (objectToCheck.GetComponent<InteractableItemController>().allowObjectDropping)
                    {
                        ObjectActions.RemoveObjectFromInventory(objectToCheck, true, true, gameManager, player);
                    }
                    else
                    {
                        //ADD WARNING TEXT, THIS MIGHT BE HELPFUL LATER
                        gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().AddToInventory(objectToCheck);
                    }
                }
            }
        }
    }
}