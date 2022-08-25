using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GrandmaAIController : MonoBehaviour
{
    [Header("Referenced Gameobjects")]
    public GameObject gameManager;

    [Header("Character Settings")]
    [Range(0.01f, 1)]
    public float characterSubtitleDurationScale;
    public float characterDestinationTolerance;

    private GameObject player;
    private NavMeshAgent grandmaNavMeshAgent;
    [HideInInspector]
    public CharacterBehaviorContainer currentCharacterBehaviorContainer;
    private int currentCharacterBehaviorIndex;
    [HideInInspector]
    public int currentCharacterDialogueTextIndex;
    private bool playerAtDestination;

    [Header("Behavior Containers")]
    public CharacterBehaviorContainer[] characterBehaviorContainers;

    private void Start()
    {
        player = gameManager.GetComponent<GameControlsManager>().player;
        grandmaNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        ActiveNavigationController();
    }

    //DONE
    public void SetBehaviorByIndex(int behaviorIndex)
    {
        gameObject.GetComponent<CharacterInteractionContainer>().interactedWithPlayer = false;

        foreach (CharacterBehaviorContainer container in characterBehaviorContainers)
        {
            if (container.BehaviorContainerIndex == behaviorIndex)
            {
                //New Container Found and Assigned
                currentCharacterBehaviorContainer = container;
                currentCharacterBehaviorIndex = currentCharacterBehaviorContainer.BehaviorContainerIndex;
                currentCharacterBehaviorContainer.dialogueComplete = false;

                //ResetDialogue(false);
                ResetDialogue(false);

                ExecuteCharacterBehaviors();

                break;
            }
        }
    }

    //WIP
    private void ExecuteCharacterBehaviors()
    {
        gameObject.GetComponent<CharacterInteractionContainer>().currentlyInteractable = currentCharacterBehaviorContainer.playerInteractableEnabled;

        //Setting Movement
        playerAtDestination = !currentCharacterBehaviorContainer.move;

        //Setting Dialogue
        if (currentCharacterBehaviorContainer.sequentialCharacterDialogue.Length > 0)
        {
            if (currentCharacterBehaviorContainer.dialogueStartPromptType == CharacterBehaviorContainer.DialogueStartPromptType.immediate)
            {
                //Executing automatic dialogue sequence
                ExecuteDialogue(true);
            }
        }

        //Setting Animation Bool States
        foreach (CharacterAnimationBool currentBool in currentCharacterBehaviorContainer.animationBools)
        {
            if (currentBool.animationBoolToggleType == CharacterAnimationBool.AnimationBoolToggleType.immediate)
            {
                gameObject.GetComponent<Animator>().SetBool(currentBool.animationBoolName, currentBool.animationBoolState);
            }
        }
    }

    //DONE
    public void ExecuteDialogue(bool automaticProgression)
    {
        if (!currentCharacterBehaviorContainer.dialogueComplete)
        {
            if (currentCharacterDialogueTextIndex == 0)
            {
                //Setup Audio Loop
                gameObject.GetComponent<AudioSource>().clip = currentCharacterBehaviorContainer.dialogueLoopingAudioClip;
                gameObject.GetComponent<AudioSource>().loop = true;
                gameObject.GetComponent<AudioSource>().Play();
            }

            StartCoroutine(DialogueExecution(automaticProgression));
        }
        else
        {
            ResetDialogue(true);
        }
    }

    //DONE
    private void ResetDialogue(bool dialogueComplete)
    {
        gameObject.GetComponent<AudioSource>().Stop();
        gameObject.GetComponent<AudioSource>().clip = null;
        gameObject.GetComponent<AudioSource>().loop = false;
        currentCharacterBehaviorContainer.dialogueComplete = dialogueComplete;

        gameManager.GetComponent<PlayerHUDController>().SetSubtitles("");
        currentCharacterDialogueTextIndex = 0;
    }

    //DONE
    private IEnumerator DialogueExecution(bool automaticProgression)
    {
        if (!currentCharacterBehaviorContainer.dialogueComplete && currentCharacterDialogueTextIndex < currentCharacterBehaviorContainer.sequentialCharacterDialogue.Length)
        {
            gameManager.GetComponent<PlayerHUDController>().SetSubtitles(currentCharacterBehaviorContainer.sequentialCharacterDialogue[currentCharacterDialogueTextIndex]);
            
            currentCharacterDialogueTextIndex += 1;

            if (automaticProgression)
            {
                yield return new WaitForSeconds(characterSubtitleDurationScale * currentCharacterBehaviorContainer.sequentialCharacterDialogue[currentCharacterDialogueTextIndex - 1].Length);
                ExecuteDialogue(true);
            }
        }
        else if (!currentCharacterBehaviorContainer.dialogueComplete && currentCharacterDialogueTextIndex >= currentCharacterBehaviorContainer.sequentialCharacterDialogue.Length) 
        {
            ResetDialogue(true);
        }
    }

    //WIP
    private void DestinationCheck()
    {
        if ((gameObject.transform.position - currentCharacterBehaviorContainer.destinationPosition).magnitude <= characterDestinationTolerance)
        {
            playerAtDestination = true;
            grandmaNavMeshAgent.isStopped = true;

            foreach (CharacterAnimationBool currentBool in currentCharacterBehaviorContainer.animationBools)
            {
                gameObject.GetComponent<Animator>().SetBool(currentBool.animationBoolName, currentBool.animationBoolState);
            }

            //SET PLAYER ROTATION DESTINATON HERE *****
        }
    }

    //WIP
    private void ActiveNavigationController()
    {
        if (!playerAtDestination && currentCharacterBehaviorContainer.move)
        {
            DestinationCheck();


            if ((grandmaNavMeshAgent.nextOffMeshLinkData.startPos - gameObject.transform.position).magnitude <= characterDestinationTolerance && grandmaNavMeshAgent.nextOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeJumpAcross)
            {
                grandmaNavMeshAgent.isStopped = true;

                print("At door");
                //Play at door
            }
            else
            {
                grandmaNavMeshAgent.SetDestination(currentCharacterBehaviorContainer.destinationPosition);
            }
        }
        else
        {
            //rotate character to destination position
        }
    }
}

//DONE
[Serializable]
public struct CharacterBehaviorContainer
{
    public enum DialogueStartPromptType { immediate, playerInteraction }

    [Header("Container ID")]
    public string containerName;
    public int BehaviorContainerIndex;

    [Header("Dialogue")]
    public string[] sequentialCharacterDialogue;
    public AudioClip dialogueLoopingAudioClip;
    [HideInInspector]
    public bool dialogueComplete;

    [Header("Player Interaction")]
    public bool playerInteractableEnabled;
    public DialogueStartPromptType dialogueStartPromptType;

    [Header("Movement")]
    public bool move;
    public Vector3 destinationPosition;
    public Vector3 destinationRotation;
    public GameObject objectToInteractWith;
    [HideInInspector]
    public bool objectToggleDestination;
    public bool closeObjectAfterDuration;
    public float closeWaitDuration;
    [HideInInspector]
    public bool interactionComplete;

    [Header("Animation")]
    public CharacterAnimationBool[] animationBools;
}

[Serializable]
public struct CharacterAnimationBool
{
    public enum AnimationBoolToggleType { immediate, atDestination }

    public string animationBoolName;
    public bool animationBoolState;
    public AnimationBoolToggleType animationBoolToggleType;
}