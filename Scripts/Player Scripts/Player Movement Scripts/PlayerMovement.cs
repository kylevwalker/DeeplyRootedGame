using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Referenced Game Objects")]
    public GameObject gameManager;
    public GameObject playerCamera;
    private CharacterController playerCharacterController;
    public GameObject playerBone;
    public GameObject playerBoneParent;

    [HideInInspector]
    public Vector3 currentPlayerSpawnPosition;
    [HideInInspector]
    public Vector3 currentPlayerSpawnRotationEulers;

    [Header("Debug")]
    public bool enableDebugMode;

    //Current Player Movement States
    [HideInInspector]
    public bool playerCurrentlySprinting;
    private bool playerCurrentlyCrouching;
    private bool playerCrouchInput;
    [HideInInspector]
    public bool playerMoving;
    private bool playerCanStand;
    [HideInInspector]
    public bool playerGrounded;
    [HideInInspector]
    public bool playerJumping;
    private bool playerSprintingBeforeJump;
    private bool playerGrabbingObject;

    //Value Storage
    [HideInInspector]
    public float currentPlayerHeadBobSpeed;
    private float playerHeadBobbingBaseSpeed;
    private float basePlayerMovementSpeed;
    [HideInInspector]
    public float currentLocalPlayerMovementSpeed;
    private float currentOverallMovementMultiplierValue;
    [HideInInspector]
    public Vector3 playerGravityMovement;
    private Vector3 playerMovement;
    [HideInInspector]
    private Vector3 initialPlayerPosition;
    [HideInInspector]
    public string currentGroundTag;
    [HideInInspector]
    public bool firstWalkFrame;
    private float previousWalkFrameMagnitude;

    //METHOD INITIALIZATION AND UPDATE
    void Start()
    {
        playerCurrentlySprinting = false;
        playerCurrentlyCrouching = false;
        playerMoving = false;
        playerCanStand = false;
        playerJumping = false;
        playerSprintingBeforeJump = false;

        playerCharacterController = gameObject.GetComponent<CharacterController>();
        basePlayerMovementSpeed = gameManager.GetComponent<GameControlsManager>().playerWalkSpeed;
        playerHeadBobbingBaseSpeed = gameManager.GetComponent<GameControlsManager>().playerHeadBobbingBaseSpeed;
    }
    void Update()
    {
        PlayerPauseAndInventoryCheck();
        PlayerGravity();
        UpdateMovementStates();
        DebugMovemement();
        UpdateCharacterControllerBounds();
    }

    //DONE
    private void UpdateCharacterControllerBounds()
    {
        float yBounds = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.y;
        playerCharacterController.height = yBounds;
        playerCharacterController.center = Vector3.up * yBounds / 2;
    }

    //*** UPDATE & DEBUGGING *** DONE
    private void DebugMovemement()
    {
        if (enableDebugMode)
            print("Moving : " + playerMoving + " | Grounded : " + playerGrounded + " | Able To Stand : " + playerCanStand + " | Crouching : " + playerCurrentlyCrouching + " | Move Speed : " + currentLocalPlayerMovementSpeed);
    }

    private void PlayerPauseAndInventoryCheck()
    {
        bool paused = gameManager.GetComponent<InGamePauseMenuManager>().playerCurrentlyPaused;
        bool inInventory = gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().playerInInventory;
        if (!paused && !inInventory)
        {
            PlayerMovementMultiplierManagement();
            PlayerStepFunction();
            PlayerJumpFunction();
            PlayerCrouchFunction();
        }
    }

    private void LateUpdate()
    {
        initialPlayerPosition = transform.position;
        if (playerSprintingBeforeJump && !gameManager.GetComponent<CustomPlayerControls>().holdToSprint && playerGrounded)
        {
            playerSprintingBeforeJump = false;
            playerCurrentlySprinting = true;
        }
    }
    private void UpdateMovementStates()
    {
        playerMoving = PlayerCurrentlyMoving();
        playerCanStand = PlayerAbleToStand();
        playerGrounded = PlayerCurrentlyGrounded();
        playerGrabbingObject = gameObject.GetComponent<PlayerInteractionController>().itemBeingGrabbed;
    }

    //*** STEPPING *** DONE
    private void PlayerStepFunction()
    {
        Vector3 playerMovementInput = Vector3.zero;

        bool playerForwardInput = Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().playerForwardMovement);
        bool playerBackInput = Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().playerBackMovement);
        bool playerRightInput = Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().playerRightMovement);
        bool playerLeftInput = Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().playerLeftMovement);

        float directionSpeedScaleValue = 1;

        if (playerForwardInput && !playerBackInput)
        {
            playerMovementInput.z = 1;
        }
        else if (playerBackInput && !playerForwardInput)
        {
            playerMovementInput.z = -1;
            if (!playerRightInput && !playerLeftInput)
            {
                directionSpeedScaleValue = gameManager.GetComponent<GameControlsManager>().playerBackWalkSpeedDivider;
            }
        }

        void ModulateDiagonalSpeedScale() 
        {
            if (!playerBackInput && !playerForwardInput)
            {
                directionSpeedScaleValue = gameManager.GetComponent<GameControlsManager>().playerSideWalkSpeedDivider;
            }
            else
            {
                if (playerForwardInput)
                {
                    directionSpeedScaleValue = gameManager.GetComponent<GameControlsManager>().playerForwardDiagonalWalkSpeedDivider;
                }
                else
                {
                    directionSpeedScaleValue = gameManager.GetComponent<GameControlsManager>().playerBackDiagonalWalkSpeedDivider;
                }
            }
        }

        if (playerRightInput && !playerLeftInput)
        {
            playerMovementInput.x = 1;
            ModulateDiagonalSpeedScale();
        }
        else if (playerLeftInput && !playerRightInput)
        {
            playerMovementInput.x = -1;
            ModulateDiagonalSpeedScale();
        }

        playerMovementInput.Normalize();
        playerMovementInput /= directionSpeedScaleValue;

        playerMovement = playerMovementInput * currentLocalPlayerMovementSpeed * Time.deltaTime;
        if (gameObject.GetComponent<PlayerInteractionController>().itemBeingGrabbed)
        {
            playerMovement /= gameManager.GetComponent<GameControlsManager>().objectMoveSpeedDivider;
        }
        playerMovement = transform.TransformDirection(playerMovement);
        playerCharacterController.Move(playerMovement);

        firstWalkFrame = (playerMovement.magnitude != 0 && previousWalkFrameMagnitude == 0);

        previousWalkFrameMagnitude = Mathf.Abs(playerMovement.magnitude);
    }

    //*** JUMPING *** DONE
    private void PlayerJumpFunction()
    {
        if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerJumpKey))
        {
            if (!playerCurrentlyCrouching && playerGrounded && !playerJumping && !playerGrabbingObject)
            {
                if (gameObject.GetComponentInChildren<PlayerAudioController>() != null)
                {
                    gameObject.GetComponentInChildren<PlayerAudioController>().PlayerJumpAudioController();
                }
                if (!gameManager.GetComponent<CustomPlayerControls>().holdToSprint)
                {
                    playerSprintingBeforeJump = true;
                }
                playerJumping = true;
            }
        }
    }

    //*** CROUCHING *** DONE
    private void PlayerCrouchFunction()
    {
        gameObject.GetComponent<Animator>().SetBool("Player Crouched", playerCurrentlyCrouching);
        gameObject.GetComponent<Animator>().SetBool("Player Crouch Input", playerCrouchInput);
        if (playerGrounded)
        {
            if (gameManager.GetComponent<CustomPlayerControls>().holdToCrouch)
            {
                if (Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().playerCrouchKey))
                {
                    playerCurrentlyCrouching = true;
                    playerCrouchInput = true;
                    gameObject.GetComponent<Animator>().speed = 1;
                }
                else
                {
                    playerCrouchInput = false;
                    if (playerCanStand)
                    {
                        if (gameObject.GetComponent<Animator>().speed == 0)
                        {
                            gameObject.GetComponent<Animator>().speed = 1;
                        }
                        playerCurrentlyCrouching = false;
                    }
                    else
                    {
                        playerCurrentlyCrouching = true;
                        if (gameObject.GetComponent<Animator>().speed == 1)
                        {
                            gameObject.GetComponent<Animator>().speed = 0;
                        }
                    }
                }
            }
            else
            {
                if (playerCurrentlyCrouching)
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerCrouchKey))
                    {
                        playerCrouchInput = false;
                        if (playerCanStand)
                        {
                            if (gameObject.GetComponent<Animator>().speed == 0)
                            {
                                gameObject.GetComponent<Animator>().speed = 1;
                            }
                            playerCurrentlyCrouching = false;
                        }
                        else
                        {
                            playerCurrentlyCrouching = true;
                            if (gameObject.GetComponent<Animator>().speed == 1)
                            {
                                gameObject.GetComponent<Animator>().speed = 0;
                            }
                        }
                    }
                }
                else if (!playerCurrentlyCrouching)
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerCrouchKey))
                    {
                        playerCrouchInput = true;
                        playerCurrentlyCrouching = true;
                    }
                }
            }
        }
    }
    private void CrouchedMovementSpeedIncrement()
    {
        if (currentLocalPlayerMovementSpeed > gameManager.GetComponent<GameControlsManager>().playerCrouchedMovementSpeed)
        {
            currentLocalPlayerMovementSpeed -= Mathf.Abs(gameManager.GetComponent<GameControlsManager>().playerCrouchedMovementSpeedIncrement) * Time.deltaTime;
        }
        else
        {
            currentLocalPlayerMovementSpeed = gameManager.GetComponent<GameControlsManager>().playerCrouchedMovementSpeed;
        }
    }

    //*** GRAVITY *** DONE
    private void PlayerGravity()
    {
        if (playerJumping)
        {
            if (playerGrounded || playerGravityMovement.y < 0)
            {
                playerJumping = false;
            }
            playerGravityMovement.y = gameManager.GetComponent<GameControlsManager>().playerJumpStrength;
        }
        else if (playerGrounded)
        {
            playerGravityMovement.y = Mathf.Epsilon;
        }
        else
        {
            if (Mathf.Abs(playerGravityMovement.y) < gameManager.GetComponent<GameControlsManager>().playerTerminalVelocity)
            {
                playerGravityMovement.y += gameManager.GetComponent<GameControlsManager>().playerGravityForce * Time.deltaTime;
            }
        }

        playerGravityMovement = transform.TransformDirection(playerGravityMovement);
        playerCharacterController.Move(playerGravityMovement * Time.deltaTime);
    }

    //*** GROUNDING *** DONE    
    public bool PlayerCurrentlyGrounded()
    {
        float checkRadius = gameObject.GetComponent<Collider>().bounds.extents.x;
        float checkTolerance = gameManager.GetComponent<GameControlsManager>().playerGroundCheckTolerance;

        Vector3 origin = playerBone.transform.position;

        Vector3 playerGroundSphereCastOrigin = transform.position + new Vector3(0, -gameObject.GetComponent<Collider>().bounds.extents.y + checkRadius, 0);
        if (enableDebugMode) { Debug.DrawRay(origin, Vector3.down * (checkRadius + checkTolerance), Color.yellow); }
        if (Physics.SphereCast(origin, checkRadius, Vector3.down, out RaycastHit potentialGroundHit, checkTolerance))
        {
            GameObject potentialGround = potentialGroundHit.transform.root.gameObject;
            if (potentialGround == gameObject.GetComponent<PlayerInteractionController>().grabbedGameObject)
            {
                gameObject.GetComponent<PlayerInteractionController>().ObjectRelease(false);
                return false;
            }
            else if (potentialGround.GetComponent<Rigidbody>() != null && Mathf.Abs(potentialGround.GetComponent<Rigidbody>().velocity.y) > gameManager.GetComponent<GameControlsManager>().potentialGroundYVelocityTolerance)
            {
                return false;
            }
            else
            {
                currentGroundTag = potentialGround.tag;
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    //*** STAND ABILITY CHECK *** DONE | CLEANUP OLD CODE
    private bool PlayerAbleToStand()
    {
        float checkRadius = gameObject.GetComponent<Collider>().bounds.extents.x;

        Vector3 origin = playerBoneParent.transform.position - (Vector3.up * checkRadius);

        float checkTolerance = gameManager.GetComponent<GameControlsManager>().playerStandCheckTolerance;

        float playerStandSpherecastLength = checkRadius + checkTolerance;

        if (enableDebugMode) { Debug.DrawRay(origin, Vector3.up * playerStandSpherecastLength, Color.red); }

        return !Physics.SphereCast(origin, checkRadius, Vector3.up, out RaycastHit standCastHit, playerStandSpherecastLength) || (standCastHit.collider.gameObject.GetComponent<InteractableItemController>() != null && standCastHit.collider.GetComponentInParent<InteractableItemController>().gameObject == gameObject.GetComponent<PlayerHotSlotController>().objectInHand);
    }

    //*** MOVEMENT CHECK *** DONE
    private bool PlayerCurrentlyMoving()
    {
        float tolerance = gameManager.GetComponent<GameControlsManager>().playerMoveCheckTolerance;
        Vector3 newPlayerPosition = transform.position;
        newPlayerPosition.y = 0;
        initialPlayerPosition.y = 0;
        float moveMagnitude = (initialPlayerPosition - newPlayerPosition).magnitude;
        return (moveMagnitude > tolerance || moveMagnitude < -tolerance);
    }

    //*** Movement Multiplier Management *** DONE
    private void PlayerMovementMultiplierManagement()
    {
        if (gameManager.GetComponent<CustomPlayerControls>().holdToSprint)
        {
            if (Input.GetKey(gameManager.GetComponent<CustomPlayerControls>().playerSprintKey))
            {
                playerCurrentlySprinting = true;
            }
            else
            {
                playerCurrentlySprinting = false;
            }
        }
        else
        {
            if (playerCurrentlySprinting)
            {
                if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerSprintKey))
                {
                    playerCurrentlySprinting = false;
                }
            }
            else if (!playerCurrentlySprinting)
            {
                if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerSprintKey))
                {
                    playerCurrentlySprinting = true;
                }
            }
        }

        if (playerCurrentlyCrouching || !playerGrounded || playerGrabbingObject)
        {
            playerCurrentlySprinting = false;
        }

        if (playerCurrentlySprinting)
        {
            if (currentOverallMovementMultiplierValue < gameManager.GetComponent<GameControlsManager>().playerMovementMaximumMultiplierValue)
            {
                if (currentOverallMovementMultiplierValue + (gameManager.GetComponent<GameControlsManager>().playerMovementMultiplierAccelerationSpeed * Time.deltaTime) > gameManager.GetComponent<GameControlsManager>().playerMovementMaximumMultiplierValue)
                {
                    currentOverallMovementMultiplierValue = gameManager.GetComponent<GameControlsManager>().playerMovementMaximumMultiplierValue;
                }
                else
                {
                    currentOverallMovementMultiplierValue += gameManager.GetComponent<GameControlsManager>().playerMovementMultiplierAccelerationSpeed * Time.deltaTime;
                }
            }
        }
        else if (playerGrounded)
        {
            if (currentOverallMovementMultiplierValue > 0)
            {
                if (currentOverallMovementMultiplierValue - (gameManager.GetComponent<GameControlsManager>().playerMovementMultiplierDeaccelerationSpeed * Time.deltaTime) < 0)
                {
                    currentOverallMovementMultiplierValue = 0;
                }
                else
                {
                    currentOverallMovementMultiplierValue -= gameManager.GetComponent<GameControlsManager>().playerMovementMultiplierDeaccelerationSpeed * Time.deltaTime;
                }
            }
        }

        if (!playerMoving)
        {
            currentOverallMovementMultiplierValue = 0;
        }
        if (!playerGrounded)
        {
            currentPlayerHeadBobSpeed = 0f;
        }
        else if (playerCurrentlyCrouching)
        {
            currentOverallMovementMultiplierValue = 0;
            CrouchedMovementSpeedIncrement();
            currentPlayerHeadBobSpeed = playerHeadBobbingBaseSpeed;
        }
        else
        {
            if (currentOverallMovementMultiplierValue != 0)
            {
                currentLocalPlayerMovementSpeed = basePlayerMovementSpeed * (1 + currentOverallMovementMultiplierValue) * gameManager.GetComponent<GameControlsManager>().playerMovementSprintMultiplier;
                currentPlayerHeadBobSpeed = playerHeadBobbingBaseSpeed * (1 + currentOverallMovementMultiplierValue * gameManager.GetComponent<GameControlsManager>().playerHeadBobSpeedMultiplier);
            }
            else
            {
                currentLocalPlayerMovementSpeed = basePlayerMovementSpeed;
                currentPlayerHeadBobSpeed = playerHeadBobbingBaseSpeed;
            }
        }
        if (!playerMoving)
        {
            currentPlayerHeadBobSpeed = 0f;
        }
    }
}