using UnityEngine;

public class GameControlsManager : MonoBehaviour
{
    //Ctrl M + Ctrl X to expand all methods
    //Ctrl M + Ctrl E to expand method
    //Ctrl M + Ctrl O to collapse all methods
    //Ctrl M + Ctrl S to collapse method

    [Header("Connected Objects")]
    public GameObject player;
    public GameObject playerInventoryBag;


    //* Player Look Controls
    [Header("Private Player Look Controls")]
    public string mouseXInputName;
    public string mouseYinputName;


    [Header("Private Player Movement Multiplier Controls")]
    public float playerMovementMultiplierAccelerationSpeed;
    public float playerMovementMultiplierDeaccelerationSpeed;
    public float playerMovementMaximumMultiplierValue;


    //* Player Sprint Controls
    [Header("Private Player Sprint Controls")]
    [Range(0.01f, 20)]
    public float playerMovementSprintMultiplier;


    //* Player Walk Controls
    [Header("Private Player Walk Controls")]
    [Range(0, 10)]
    public float playerWalkSpeed;
    [Range(0, 0.1f)]
    public float playerMoveCheckTolerance;
    [Range(1, 2)]
    public float playerForwardDiagonalWalkSpeedDivider;
    [Range(1, 2)]
    public float playerSideWalkSpeedDivider;
    [Range(1, 2)]
    public float playerBackDiagonalWalkSpeedDivider;
    [Range(1, 2)]
    public float playerBackWalkSpeedDivider;


    //* Player Jump Controls
    [Header("Private Player Jump Controls")]
    [Range(0, 10)]
    public float playerJumpStrength;


    //* Player In Air Controls
    [Header("Private Player In Air Controls")]
    public float playerGravityForce;
    public float playerTerminalVelocity;
    [Range(0, 1)]
    public float playerGroundCheckTolerance;
    [Range(0, 1)]
    public float potentialGroundYVelocityTolerance;


    //* Player Head Bobbing
    [Header("Private Player Head Bob Controls")]
    [Range(0, 10)]
    public float playerHeadBobbingBaseSpeed;
    [Range(0.01f, 10)]
    public float playerHeadBobSpeedMultiplier;
    public float playerHeadBobbingHeight;


    //* Player Crouch Controls
    [Header("Private Player Crouch Controls")]
    [Range(0.01f, 10)]
    public float playerCrouchedMovementSpeed;
    [Range(0, 20)]
    public float playerCrouchedMovementSpeedIncrement;
    [Range(0, 0.1f)]
    public float playerStandCheckTolerance;


    //* Player Function Controls
    [Header("Private Player Interaction Controls")]
    [Range(0, 10)]
    public float itemInteractionDistance;
    public string interactableItemTag;
    [Range(-1, 20)]
    public int interactableItemLayer;
    [Range(1, 25)]
    public float objectMoveSpeedDivider;
    [Range(1, 10)]
    public float playerHeadbobSpeedDivider;
    [Range(0, 10)]
    public float maxObjectOrbitSpeed;
    [Range(0, 10)]
    public float maxObjectLiftSpeed;
    [Range(0, 5)]
    public float playerLookInputSpeedTolerance;
    [Range(0, 4)]
    public float grabbedObjectDisplacementAllowance;
    [Range(0, 0.1f)]
    public float grabbedObjectPositioningResetTolerance;
    public AnimationCurve grabbedObjectPositionMoveSpeedCurve;
    public float grabbedObjectMovementPreservationMultiplier;


    [Header("Interactable Object Use Charge Settings")]
    public float objectUseChargeBarIncrement;
    public float objectUseUnchargeBarIncrement;
    public float objectUseChargeBarRefreshRate;


    [Header("Private Player Inventory Controls")]
    public float objectReleaseVectorScale;
    [Range(0, 20)]
    public float interactionRayLength;
    public float minInventoryTextTransparency;
    public float inventoryTextIncrementValue;
    public float inventoryTextRefreshRate;
    public float inventoryTextOutlineTransparency;
    [Range(0, 4)]
    public float objectDropYOffsetCheck;


    //* Player Crosshair Controls
    [Header("Crosshair Settings")]
    public float minCrossHairTransparency;
    [Tooltip("Larger Values make the fade smoother but take longer to complete")]
    public float crossHairIncrementValue;
    [Tooltip("Larger Value increase fade speed and lag")]
    public float crosshairRefreshRate;


    //* Player Interaction Text Controls
    [Header("Player Interaction Text Controls")]
    public float minInteractionTextTransparency;
    [Tooltip("Larger Values make the fade smoother but take longer to complete")]
    public float interactionTextIncrementValue;
    [Tooltip("Larger Value increase fade speed and lag")]
    public float interactionTextRefreshRate;
    public float interactionTextOutlineTransparency;


    [Header("Player HUD Slot Fade Controls")]
    public float minHUDSlotTransparency;
    public float HUDSlotIncrementValue;
    public float HUDSlotFadeRefreshRate;
    public float HUDSlotMinTransparencyDuration;


    [Header("Ceiling Fan Controls")]
    public float universalCeilingFanMaxRotationSpeed;
}