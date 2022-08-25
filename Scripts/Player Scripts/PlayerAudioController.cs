using System;
using System.Collections;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Referenced Objects")]
    public GameObject gameManager;
    public AudioSource playerMovementAudioPlayer;
    public GameObject playerCamera;

    [Header("Debugging")]
    public bool enableDebugMode;

    [Header("Player Audio Controls")]
    [Range(0, 4)]
    public int defaultAudioTypeIndex;
    public MovementAudioTypeContainer[] playerAudioContainers;
    private MovementAudioTypeContainer currentAudioContainer;

    [Header("Player Step Audio Controls")]
    public AnimationCurve playerMoveSpeedToStepAudioVolumeCurve;
    private AudioClip currentStepAudioClip = null;

    [Header("Player Land Audio Controls")]
    public AnimationCurve playerGravityMovementToLandAudioVolumeCurve;
    private AudioClip currentLandAudioClip = null;

    [Header("Player Jump Audio Controls")]
    public AnimationCurve playerMoveSpeedToJumpAudioVolumeCurve;
    private AudioClip currentJumpAudioClip = null;

    private bool previousUpdateCameraLowestPosition = true;
    private bool allowNextStepAudioClipStartOverride = true;
    private bool previousFrameGrounded = true;
    private float previousPlayerGravityMovement;


    //DONE
    private void Update()
    {
        SetCurrentAudioTypeSet();
        PlayerStepAudioController();
        PlayerLandAudioController();
    }

    //DONE
    private void PlayerStepAudioController()
    {
        IEnumerator NextStepOverrideTimer(float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            allowNextStepAudioClipStartOverride = true;
        }

        if (previousUpdateCameraLowestPosition && !playerCamera.GetComponent<PlayerHeadBobbing>().cameraAtLowestPoint)
        {
            allowNextStepAudioClipStartOverride = true;
        }

        if (gameObject.GetComponentInParent<PlayerMovement>().playerMoving && gameObject.GetComponentInParent<PlayerMovement>().playerGrounded)
        {
            if (playerCamera.GetComponent<PlayerHeadBobbing>().cameraAtLowestPoint || gameObject.GetComponentInParent<PlayerMovement>().firstWalkFrame)
            {
                if (allowNextStepAudioClipStartOverride)
                {
                    allowNextStepAudioClipStartOverride = false;
                    currentStepAudioClip = GetAudioClip(currentAudioContainer.standardStepAudioClips, currentAudioContainer.specialStepAudioClips, currentAudioContainer.allowStepAudioClipRepetition, currentAudioContainer.standardStepAudioClipBiasPercentage, currentStepAudioClip);
                    float stepAudioVolumeScale = playerMoveSpeedToStepAudioVolumeCurve.Evaluate(gameObject.GetComponentInParent<PlayerMovement>().currentLocalPlayerMovementSpeed) * currentAudioContainer.stepClipVolumeMultiplier;
                    playerMovementAudioPlayer.PlayOneShot(currentStepAudioClip, stepAudioVolumeScale);
                    StartCoroutine(NextStepOverrideTimer(currentStepAudioClip.length));
                }
            }
        }

        previousUpdateCameraLowestPosition = playerCamera.GetComponent<PlayerHeadBobbing>().cameraAtLowestPoint;
    }

    //DONE
    private void PlayerLandAudioController()
    {
        if (!previousFrameGrounded && gameObject.GetComponentInParent<PlayerMovement>().playerGrounded)
        {
            bool landAudioClipAvailable = currentAudioContainer.landAudioClips.Length > 0;
            AudioClip[] potentialLandAudioClipsArray = landAudioClipAvailable ? currentAudioContainer.landAudioClips : playerAudioContainers[defaultAudioTypeIndex].landAudioClips;
            bool allowLandAudioClipRepetition = landAudioClipAvailable ? currentAudioContainer.allowLandAudioClipRepetition : playerAudioContainers[defaultAudioTypeIndex].allowLandAudioClipRepetition;
            float landAudioVolumeScale = playerGravityMovementToLandAudioVolumeCurve.Evaluate(previousPlayerGravityMovement) * (landAudioClipAvailable ? currentAudioContainer.landAudioClipVolumeMultiplier : playerAudioContainers[defaultAudioTypeIndex].landAudioClipVolumeMultiplier);
            currentLandAudioClip = GetAudioClip(potentialLandAudioClipsArray, null, allowLandAudioClipRepetition, 100, currentLandAudioClip);
            playerMovementAudioPlayer.PlayOneShot(currentLandAudioClip, landAudioVolumeScale);
            if (enableDebugMode)
            {
                print("Land Audio Played" + " | Volume Scale : " + landAudioVolumeScale + " | Clip Name : " + currentLandAudioClip + " | Previous Player Gravity Movement : " + previousPlayerGravityMovement);
            }
        }
        previousFrameGrounded = gameObject.GetComponentInParent<PlayerMovement>().playerGrounded;
        if (Mathf.Abs(gameObject.GetComponentInParent<PlayerMovement>().playerGravityMovement.y) > Mathf.Epsilon)
        {
            previousPlayerGravityMovement = Mathf.Abs(gameObject.GetComponentInParent<PlayerMovement>().playerGravityMovement.y);
        }
    }

    //DONE
    public void PlayerJumpAudioController()
    {
        bool jumpAudioClipAvailable = currentAudioContainer.jumpAudioClips.Length > 0;
        AudioClip[] potentialJumpAudioClipsArray = jumpAudioClipAvailable ? currentAudioContainer.jumpAudioClips : playerAudioContainers[defaultAudioTypeIndex].jumpAudioClips;
        bool allowClipRepetition = jumpAudioClipAvailable ? currentAudioContainer.allowJumpAudioClipRepetition : playerAudioContainers[defaultAudioTypeIndex].allowJumpAudioClipRepetition;
        float jumpAudioVolumeScale = playerMoveSpeedToJumpAudioVolumeCurve.Evaluate(gameObject.GetComponentInParent<PlayerMovement>().currentLocalPlayerMovementSpeed) * (jumpAudioClipAvailable ? currentAudioContainer.jumpAudioClipVolumeMultiplier : playerAudioContainers[defaultAudioTypeIndex].jumpAudioClipVolumeMultiplier);
        currentJumpAudioClip = GetAudioClip(potentialJumpAudioClipsArray, null, allowClipRepetition, 100, currentJumpAudioClip);
        playerMovementAudioPlayer.PlayOneShot(currentJumpAudioClip, jumpAudioVolumeScale);
    }

    //DONE
    private AudioClip GetAudioClip(AudioClip[] standardAudioClipArray, AudioClip[] specialAudioClipArray, bool allowAudioClipRepetition, float standardAudioClipBiasPercentage, AudioClip previousAudioClip)
    {
        bool usingspecialAudio = specialAudioClipArray != null && (specialAudioClipArray.Length > 0 && UnityEngine.Random.Range(0, 101) > standardAudioClipBiasPercentage);
        return SelectRandomAudio(usingspecialAudio ? specialAudioClipArray : standardAudioClipArray, allowAudioClipRepetition, previousAudioClip);
    }

    //DONE
    private AudioClip SelectRandomAudio(AudioClip[] audioClipArray, bool allowRepetition, AudioClip previousAudioClip)
    {
        if (audioClipArray.Length < 1)
        {
            return null;
        }
        AudioClip potentialClip;
        do
        {
            potentialClip = audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)];
        }
        while (potentialClip == previousAudioClip && !allowRepetition && audioClipArray.Length > 1);
        return potentialClip;
    }

    //DONE
    private void SetCurrentAudioTypeSet()
    {
        if (currentAudioContainer.audioTypeTag != gameObject.GetComponentInParent<PlayerMovement>().currentGroundTag && gameObject.GetComponentInParent<PlayerMovement>().playerGrounded)
        {
            MovementAudioTypeContainer currentFoundSet = playerAudioContainers[defaultAudioTypeIndex];
            foreach (MovementAudioTypeContainer possibleAudioContainer in playerAudioContainers)
            {
                if (possibleAudioContainer.audioTypeTag == gameObject.GetComponentInParent<PlayerMovement>().currentGroundTag)
                {
                    currentFoundSet = possibleAudioContainer;
                }
            }
            currentAudioContainer = currentFoundSet;
        }
    }

    //DONE
    [Serializable]
    public struct MovementAudioTypeContainer
    {
        [Header("Audio Type ID")]
        public string audioTypeTag;

        [Header("Step Audio Settings")]
        [Range(0, 2)]
        public float stepClipVolumeMultiplier;
        public bool allowStepAudioClipRepetition;
        [Range(0, 100)]
        public int standardStepAudioClipBiasPercentage;
        public AudioClip[] standardStepAudioClips;
        public AudioClip[] specialStepAudioClips;

        [Header("Jump Audio Settings")]
        [Range(0, 2)]
        public float jumpAudioClipVolumeMultiplier;
        public bool allowJumpAudioClipRepetition;
        public AudioClip[] jumpAudioClips;

        [Header("Landing Audio Settings")]
        [Range(0, 2)]
        public float landAudioClipVolumeMultiplier;
        public bool allowLandAudioClipRepetition;
        public AudioClip[] landAudioClips;
    }

    //WIP
    [Serializable]
    public struct SpatialAudioContainer
    {
        
    }
}