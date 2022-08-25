using System.Collections;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Referenced GameObjects")]
    public GameObject gameManager;
    public GameObject playerCamera;

    [Header("Debug")]
    public bool enableInteractionDebugMode;

    //Value Storage
    [HideInInspector]
    public GameObject interactableItem;
    [HideInInspector]
    public GameObject grabbedGameObject;
    private LayerMask interactionLayer;
    [HideInInspector]
    public bool itemBeingGrabbed;
    private bool interactionReset;
    private Vector3 playerInteractionRaycastDirection;
    private Vector3 rayOrigin;
    private float interactionDistance;
    private bool playerLookMovementWithinTolerance;

    private float grabbedObjectHoldDistance;
    private float mostRecentGrabbedObjectHitDistance;

    //METHOD INITIALIZATION AND UPDATE || FIX GRABBED OBJECT SCALING WITH PLAYER
    private void Start()
    {
        interactionLayer = gameManager.GetComponent<GameControlsManager>().interactableItemLayer;
        interactionDistance = gameManager.GetComponent<GameControlsManager>().itemInteractionDistance;
        ObjectRelease(false);
        interactableItem = null;
        interactionReset = true;
    }
    void Update()
    {
        playerLookMovementWithinTolerance = playerLookMovementWithinToleranceCheck();
        MethodUpdateController();
    }

    //DONE
    private void MethodUpdateController()
    {
        if (!gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().playerInInventory && !gameManager.GetComponent<InGamePauseMenuManager>().playerCurrentlyPaused)
        {
            AvailableInteractionCheck();
            PlayerGrabControls();
            ObjectPickupCheck();
        }
        HeldObjectConfiguration();
        ObjectAutoRestriction();
    }

    //DONE
    private void AvailableInteractionCheck()
    {
        rayOrigin = playerCamera.transform.position;
        interactableItem = null;
        playerInteractionRaycastDirection = playerCamera.transform.forward;
        playerInteractionRaycastDirection.Normalize();

        RaycastHit[] interactionRaycastHits;
        interactionRaycastHits = Physics.RaycastAll(rayOrigin, playerInteractionRaycastDirection, interactionDistance);
        GameObject closestBlockingObject = null;
        float closestBlockingObjectDistance = interactionDistance;
        GameObject closestInteractableObject = null;
        float closestInteractableObjectDistance = interactionDistance;
        bool noInteractions = true;
        bool FirstValueLarger(float valueOne, float ValueTwo)
        {
            return valueOne > ValueTwo;
        }

        if (interactionRaycastHits.Length > 0)
        {
            foreach (RaycastHit hitObject in interactionRaycastHits)
            {
                float hitDistance = hitObject.distance;
                GameObject hitGameObject;
                if (hitObject.transform.gameObject.GetComponent<InteractableItemController>() != null)
                {
                    hitGameObject = hitObject.transform.gameObject.GetComponent<InteractableItemController>().gameObject;
                }
                else
                {
                    hitGameObject = hitObject.transform.root.gameObject;
                }
                LayerMask hitObjectLayer = hitGameObject.layer;

                if (hitGameObject == grabbedGameObject)
                {
                    mostRecentGrabbedObjectHitDistance = hitObject.distance;
                }

                if (hitObjectLayer == interactionLayer)
                {
                    noInteractions = false;
                    if (FirstValueLarger(closestInteractableObjectDistance, hitDistance))
                    {
                        closestInteractableObjectDistance = hitDistance;
                        closestInteractableObject = hitGameObject;
                    }
                }
                else if (hitObjectLayer != 10)
                {
                    if (FirstValueLarger(closestBlockingObjectDistance, hitDistance))
                    {
                        closestBlockingObjectDistance = hitDistance;
                        closestBlockingObject = hitGameObject;
                    }
                }
            }
        }

        bool blocked = FirstValueLarger(closestInteractableObjectDistance, closestBlockingObjectDistance);
        interactableItem = blocked ? null : closestInteractableObject;
        if (!blocked && !noInteractions)
        {
            InteractableObjectAction();
        }
        else
        {
            if (interactionStarted)
            {
                if (currentObjectChargeValue > 0)
                {
                    gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.gameObject.SetActive(false);
                    currentObjectChargeValue -= Time.deltaTime * gameManager.GetComponent<GameControlsManager>().objectUseUnchargeBarIncrement;
                    gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.value = currentObjectChargeValue;
                }
                else
                {
                    ObjectChargeReset();
                }
            }
        }

        if (enableInteractionDebugMode)
        {
            print("Hit Object Count : " + interactionRaycastHits.Length);
            print("Blocked : " + blocked + " | Blocking Object : " + closestBlockingObject);
            print("Closest Interaction : " + closestInteractableObject);
            Color raycastColor = blocked ? Color.red : Color.green;
            Debug.DrawRay(rayOrigin, playerInteractionRaycastDirection * interactionDistance, raycastColor);
        }
    }

    //DONE
    private void InteractableObjectAction()
    {
        interactableItem = interactableItem.GetComponentInParent<InteractableItemController>().gameObject;
        if (enableInteractionDebugMode)
        {
            print("Open Interaction With : " + interactableItem.name);
        }
        interactableItem.GetComponent<InteractableItemController>().enabled = true;
        if (itemBeingGrabbed && interactableItem != null)
        {
            if (interactableItem != grabbedGameObject)
            {
                PlayerInteractionControls();
            }
        }
        else
        {
            PlayerInteractionControls();
        }
    }

    //DONE
    private void PlayerInteractionControls()
    {
        if (!interactionReset)
        {
            if (Input.GetKeyUp(gameManager.GetComponent<CustomPlayerControls>().playerItemInteractionKey))
            {
                interactionReset = true;
            }
        }
        else if (Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().playerItemInteractionKey))
        {
            if (interactableItem.GetComponent<InteractableItemController>().animated)
            {
                if (interactableItem.GetComponent<Animator>() == null || interactableItem.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Static State"))
                {
                    ObjectUseCharge(!interactableItem.GetComponentInParent<InteractableItemController>().itemToggleState);
                }
            }
            else
            {
                interactableItem.GetComponentInParent<InteractableItemController>().itemToggleState = !interactableItem.GetComponentInParent<InteractableItemController>().itemToggleState;
                interactionReset = false;
            }
        }
        else if (interactionStarted)
        {
            if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerItemInteractionKey))
            {
                if (!interactableItem == chargingObject)
                {
                    ObjectChargeReset();
                }
            }
            else if (currentObjectChargeValue > 0)
            {
                currentObjectChargeValue -= Time.deltaTime * gameManager.GetComponent<GameControlsManager>().objectUseUnchargeBarIncrement;
                gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.value = currentObjectChargeValue;
            }
            else
            {
                ObjectChargeReset();
                interactionReset = true;
            }
        }
    }

    //DONE
    private void ObjectChargeReset()
    {
        interactionReset = false;
        interactionStarted = false;
        currentObjectChargeValue = 0;
        chargingObject = null;
        gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.value = 0;
        gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.gameObject.SetActive(false);
    }

    //DONE
    private GameObject chargingObject;
    private bool interactionStarted;
    private float currentObjectChargeValue;
    private void ObjectUseCharge(bool objectToggleStateDestination)
    {
        if (!interactionStarted)
        {
            interactionStarted = true;
            currentObjectChargeValue = 0;
            chargingObject = interactableItem;

            //UI Setup
            gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.maxValue = chargingObject.GetComponent<InteractableItemController>().interactionUseCharge;
            gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.value = 0;
        }
        else if (interactableItem == chargingObject)
        {
            gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.gameObject.SetActive(true);
            if (currentObjectChargeValue < chargingObject.GetComponent<InteractableItemController>().interactionUseCharge)
            {
                currentObjectChargeValue += Time.deltaTime * gameManager.GetComponent<GameControlsManager>().objectUseChargeBarIncrement;
                gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.value = currentObjectChargeValue;
            }
            else
            {
                chargingObject.GetComponent<InteractableItemController>().itemToggleState = objectToggleStateDestination;
                ObjectActions.ObjectActionController(chargingObject, gameManager);
                interactionReset = false;
                ObjectChargeReset();
            }
        }
        else
        {
            gameManager.GetComponent<PlayerHUDController>().playerObjectUseChargeSlider.gameObject.SetActive(false);
        }
    }

    //DONE
    private void PlayerGrabControls()
    {
        if (gameManager.GetComponent<CustomPlayerControls>().holdToGrab)
        {
            if (interactableItem != null && !itemBeingGrabbed && interactableItem.GetComponent<InteractableItemController>().moveableObject)
            {
                if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerItemGrabKey))
                {
                    ObjectGrab();
                }
            }
            else
            {
                if (Input.GetKeyUp(gameManager.GetComponent<CustomPlayerControls>().playerItemGrabKey))
                {
                    ObjectRelease(true);
                }
            }
        }
        else
        {
            if (interactableItem != null && !itemBeingGrabbed)
            {
                if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerItemGrabKey))
                {
                    ObjectGrab();
                }
            }
            else
            {
                if (itemBeingGrabbed)
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerItemGrabKey))
                    {
                        ObjectRelease(true);
                    }
                }
                else if (interactableItem != null)
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerItemGrabKey) && interactableItem.GetComponent<InteractableItemController>().moveableObject)
                    {
                        ObjectGrab();
                    }
                }
            }
        }
    }

    //DONE
    Vector3 grabbedObjectMovementPreservationVector;
    Vector3 previousGrabbedObjectPosition;
    private void ObjectGrab()
    {
        itemBeingGrabbed = true;
        grabbedGameObject = interactableItem;
        grabbedGameObject.GetComponent<Rigidbody>().useGravity = false;
        grabbedGameObject.transform.SetParent(playerCamera.transform);
        grabbedGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        grabbedGameObject.GetComponent<InteractableItemController>().objectBeingGrabbed = true;

        grabbedObjectHoldDistance = Mathf.Abs((grabbedGameObject.transform.position - playerCamera.transform.position).magnitude);

        previousGrabbedObjectPosition = grabbedGameObject.transform.position;
    }

    private void LateUpdate()
    {
        if (grabbedGameObject != null || itemBeingGrabbed)
        {
            grabbedObjectMovementPreservationVector = grabbedGameObject.transform.position - previousGrabbedObjectPosition;
        }
    }

    //WIP | Fix Held Object Glitching / Interpolation
    public void ObjectRelease(bool applyMovementPreservation)
    {
        if (grabbedGameObject != null || itemBeingGrabbed)
        {
            grabbedGameObject.transform.SetParent(null);
            grabbedGameObject.GetComponent<Rigidbody>().useGravity = true;
            grabbedGameObject.GetComponent<InteractableItemController>().objectBeingGrabbed = false;
            Vector3 movementVector = grabbedObjectMovementPreservationVector;
            //movementVector


            if (applyMovementPreservation)
            {
                grabbedGameObject.GetComponent<Rigidbody>().velocity = movementVector * gameManager.GetComponent<GameControlsManager>().grabbedObjectMovementPreservationMultiplier * Time.deltaTime;
            }
        }
        grabbedGameObject = null;
        itemBeingGrabbed = false;
    }

    //WIP | Fix Object Realease Glitching
    private void ObjectAutoRestriction()
    {
        if (itemBeingGrabbed)
        {
            //Block Check
            Vector3 localRayDirection = grabbedGameObject.transform.position - rayOrigin;
            RaycastHit[] grabPathObjects = Physics.RaycastAll(rayOrigin, localRayDirection, localRayDirection.magnitude);
            foreach (RaycastHit raycastHit in grabPathObjects)
            {
                GameObject hitObject = raycastHit.transform.root.gameObject;
                if (hitObject != grabbedGameObject && hitObject.layer != 10 && interactableItem != grabbedGameObject)
                {
                    ObjectRelease(false);
                    return;
                }
            }

            //High Speed Contact Check
            if (!playerLookMovementWithinTolerance && grabbedGameObject.GetComponent<InteractableItemController>().objectContact)
            {
                ObjectRelease(false);
                return;
            }

            //Object Distance Check
            Vector3 grabbedObjectDisplacementReferencePoint = playerCamera.transform.position + playerCamera.transform.forward * mostRecentGrabbedObjectHitDistance;
            float currentGrabbedObjectHoldDisplacement = (grabbedObjectDisplacementReferencePoint - grabbedGameObject.GetComponent<Collider>().ClosestPoint(grabbedObjectDisplacementReferencePoint)).magnitude;
            if (currentGrabbedObjectHoldDisplacement > gameManager.GetComponent<GameControlsManager>().grabbedObjectDisplacementAllowance && interactableItem != grabbedGameObject)
            {
                ObjectRelease(false);
                return;
            }
        }
    }

    //DONE
    private void HeldObjectConfiguration()
    {
        if (itemBeingGrabbed)
        {
            grabbedGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            PositionGrabbedObject();
        }
    }

    //DONE
    private bool playerLookMovementWithinToleranceCheck()
    {
        float mouseX = Input.GetAxis(gameManager.GetComponent<GameControlsManager>().mouseXInputName) * gameManager.GetComponent<CustomPlayerControls>().playerMouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(gameManager.GetComponent<GameControlsManager>().mouseYinputName) * gameManager.GetComponent<CustomPlayerControls>().playerMouseSensitivity * Time.deltaTime;
        return Mathf.Abs(mouseX) + Mathf.Abs(mouseY) <= gameManager.GetComponent<GameControlsManager>().playerLookInputSpeedTolerance;
    }

    //DONE
    private void PositionGrabbedObject()
    {
        Vector3 grabbedObjectPositionOffset = playerCamera.transform.position + (playerCamera.transform.forward * grabbedObjectHoldDistance) - grabbedGameObject.transform.position;
        float grabbedObjectPositionOffsetMagnitude = Mathf.Abs(grabbedObjectPositionOffset.magnitude);

        if (grabbedObjectPositionOffsetMagnitude > gameManager.GetComponent<GameControlsManager>().grabbedObjectPositioningResetTolerance)
        {
            grabbedGameObject.transform.position += Time.deltaTime * grabbedObjectPositionOffset * gameManager.GetComponent<GameControlsManager>().grabbedObjectPositionMoveSpeedCurve.Evaluate(grabbedObjectPositionOffsetMagnitude);
        }
        else
        {
            grabbedGameObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward * grabbedObjectHoldDistance;
        }
    }

    //DONE
    private void ObjectPickupCheck()
    {
        if (interactableItem != null)
        {
            if (interactableItem.GetComponent<InteractableItemController>().pickupable && !interactableItem.GetComponent<InteractableItemController>().objectInHand)
            {
                if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerItemInteractionKey))
                {
                    AudioClip pickupAudioClip = interactableItem.GetComponent<InteractableItemController>().objectPickupAudioClip;
                    float pickupAudioClipVolumeScale = interactableItem.GetComponent<InteractableItemController>().objectPickupAudioClipVolumeScale;
                    if (pickupAudioClip != null)
                    {
                        gameObject.GetComponent<AudioSource>().PlayOneShot(pickupAudioClip, pickupAudioClipVolumeScale);
                    }
                    gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().AddToInventory(interactableItem);
                }
            }
        }
    }

    //DONE
    public IEnumerator KeyUseAction(GameObject lockedObject, AudioClip unlockAudioClip)
    {
        if (unlockAudioClip != null)
        {
            lockedObject.GetComponentInChildren<AudioSource>().PlayOneShot(unlockAudioClip, 1);
            yield return new WaitForSeconds(unlockAudioClip.length);
        }

        lockedObject.GetComponent<InteractableItemController>().objectLocked = false;
        lockedObject.GetComponent<InteractableItemController>().interactionUseCharge = lockedObject.GetComponent<InteractableItemController>().unlockedObjectUseCharge;
    }
}
