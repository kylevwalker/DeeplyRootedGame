using UnityEngine;

public class InteractableItemController : MonoBehaviour
{
    [HideInInspector]
    public enum ItemType { Actionless, SimpleOpenObject, PlaceHolder, Key, pipeGun, shotgun, pipeGunBullet, shotgunBullet, screwDriver, ventCover, character, dumbwaiterRope, dumbwaiterDoor }

    [Header("Item ID Characteristics")]
    public ItemType itemType;
    public string interactionName;
    public string interactionString;

    [Header("Item Use Characteristics")]
    public bool animated;
    public bool moveableObject;
    public bool pickupable;
    public bool equippable;
    public bool allowObjectDropping;
    public float interactionUseCharge;
    public AudioClip objectPickupAudioClip;
    public float objectPickupAudioClipVolumeScale;

    [Header("Item Transform Settings")]
    public Vector3 holdPositionOffset;
    public Vector3 holdRotationOffset;
    public Vector3 inventoryHoldRotationOffset;
    public AudioClip objectOpenAudioClip;
    public AudioClip objectCloseAudioClip;

    [Header("Referenced Gameobjects")]
    public Light referencedLight;

    [Header("Lock Settings")]
    public bool objectLocked;
    public string lockKeyName;
    public string lockedObjectTextPrompt;
    public float unlockedObjectUseCharge;
    public bool destroyKeyAfterUse;
    public AudioClip lockedInteractionAudioClip;

    [Header("Dumbwaiter Settings")]
    public GameObject referencedDumbwaiterDoor;
    public int floorDestinationIndex;

    [HideInInspector]
    public bool itemToggleState;
    [HideInInspector]
    public bool itemInInventory = false;
    [HideInInspector]
    public bool objectInHand = false;
    [HideInInspector]
    public bool objectContact;
    [HideInInspector]
    public bool objectBeingGrabbed;

    private void Start()
    {
        objectInHand = false;
        ObjectActions.ReferencedLightStateUpdate(gameObject);
        gameObject.GetComponent<InteractableItemController>().enabled = false;
    }
}