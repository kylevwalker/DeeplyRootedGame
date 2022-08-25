using System.Collections;
using UnityEngine;

public class GameEventController : MonoBehaviour
{
    public GameObject player;
    public GameObject grandma;
    public GameObject enemy;
    public GameObject grandpa;

    [Header("Overall Event Settings")]
    public int initialEventIndex;

    /*
    [Header("Audio")]
    public AudioController audioController;
    */

    [Header("Game Events")]
    public GameEvent[] gameEvents;

    //Value Storage
    private GameEvent currentGameEvent;
    [HideInInspector]
    public int currentEventIndex;

    private void Start()
    {
        player = gameObject.GetComponent<GameControlsManager>().player;
        AssignCurrentEventByIndex(initialEventIndex);

    }

    private void Update()
    {
        AdvanceTypeIteration();
    }

    //DONE
    //ASSIGNS THE CURRENT EVENT INDEX USING THE PROVIDED EVENT INDEX
    private void AssignCurrentEventByIndex(int eventIndex)
    {
        currentEventIndex = eventIndex;
        foreach (GameEvent checkedGameEvent in gameEvents)
        {
            if (checkedGameEvent.eventIndex == eventIndex)
            {
                currentGameEvent = checkedGameEvent;
                CallEventActions();
            }
        }
    }

    //COMMANDS THE EVENT EXECUTER BASED ON THE EVENT ACTIONS
    private void CallEventActions()
    {
        foreach (GameAction currentGameActionToCheck in currentGameEvent.gameActions)
        {
            if (!currentGameActionToCheck.eventActionComplete)
            {
                ExecuteEventAction(currentGameActionToCheck);
            }
        }
    }

    //EXECUTES THE EVENT ACTIONS
    private void ExecuteEventAction(GameAction currentGameActionToExecute)
    {
        print(currentGameActionToExecute.ActionName);

        currentGameActionToExecute.eventActionComplete = true;
        GameObject referencedObject = currentGameActionToExecute.referencedObject;
        string referencedString = currentGameActionToExecute.referencedString;
        bool referencedBool = currentGameActionToExecute.referencedBool;
        int referencedInt = currentGameActionToExecute.referencedInt;
        Vector3 referencedVector = currentGameActionToExecute.referencedVector;

        //Switch through events and execute them here
        switch (currentGameActionToExecute.gameActionType)
        {
            //DONE
            case GameAction.GameEventTypes.setObjectEnableState:
                {
                    referencedObject.SetActive(referencedBool);
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setObjectToggleState:
                {
                    referencedObject.GetComponent<InteractableItemController>().itemToggleState = referencedBool;
                    //ObjectActions.ObjectActionController(referencedObject, gameObject);
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setDoorLockState:
                {
                    referencedObject.GetComponent<InteractableItemController>().objectLocked = referencedBool;
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setPlayerMovementState:
                {
                    player.GetComponent<PlayerMovement>().enabled = referencedBool;
                    player.GetComponentInChildren<PlayerLook>().enabled = referencedBool;
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setAnimationBoolState:
                {
                    referencedObject.GetComponent<Animator>().SetBool(referencedString, referencedBool);
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setObjectPickupable:
                {
                    referencedObject.GetComponent<InteractableItemController>().pickupable = referencedBool;
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setPlayerSpawnPosition:
                {
                    player.GetComponent<PlayerMovement>().currentPlayerSpawnPosition = referencedVector;
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setPlayerSpawnRotation:
                {
                    player.GetComponent<PlayerMovement>().currentPlayerSpawnRotationEulers = referencedVector;
                    break;
                }
            //WIP
            case GameAction.GameEventTypes.setEnemyBehaviorIndex:
                {
                    //enemy.GetComponent<EnemyAIController>().
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.setGrandmaBehaviorIndex:
                {
                    grandma.GetComponent<GrandmaAIController>().SetBehaviorByIndex(referencedInt);
                    break;
                }
            //WIP
            case GameAction.GameEventTypes.setGrandpaBehaviorIndex:
                {
                    //grandpa.GetComponent<GrandpaAIController>().
                    break;
                }
            //WIP
            case GameAction.GameEventTypes.setBackgroundAudioIndex:
                {
                    break;
                }
            //DONE
            case GameAction.GameEventTypes.callObjectAction:
                {
                    ObjectActions.ObjectActionController(referencedObject, gameObject);
                    break;
                }
            case GameAction.GameEventTypes.setObjectAnimatedState:
                {
                    referencedObject.GetComponent<InteractableItemController>().animated = referencedBool;
                    break;
                }
            case GameAction.GameEventTypes.setObjectPosition:
                {
                    referencedObject.transform.position = referencedVector;
                    break;
                }
            case GameAction.GameEventTypes.setObjectRotation:
                {
                    referencedObject.transform.eulerAngles = referencedVector;
                    break;
                }
            case GameAction.GameEventTypes.destroyObjectController:
                {
                    ObjectActions.DestroyController(referencedObject, 0);
                    break;
                }
            case GameAction.GameEventTypes.setObjectLayer:
                {
                    referencedObject.layer = referencedInt;
                    break;
                }
            default:
                break;
        }
    }

    //DONE
    //COMMANDS ASSIGNMENT OF THE CURRENT EVENT TO THE FOLLOWING EVENT
    private void AdvanceTypeIteration()
    {
        //DEBUG BELOW
        if (gameEvents.Length == 0 || currentGameEvent.advanceActions.Length == 0)
        {
            return;
        }
        //DEBUG ABOVE
        foreach (AdvanceAction currentAdvanceActionToCheck in currentGameEvent.advanceActions)
        {
            //Checking Advance Conditiions
            if (AdvanceCheck(currentAdvanceActionToCheck))
            {
                if (currentEventIndex != currentAdvanceActionToCheck.followingEventIndex)
                {
                    //print("Event Index #" + currentEventIndex + " Complete | Proceeding To Event Index #" + currentAdvanceActionToCheck.followingEventIndex);
                    AssignCurrentEventByIndex(currentAdvanceActionToCheck.followingEventIndex);
                }
            }
        }
    }

    //IN PROGRESS
    //CHECKS THE CONDITIONS TO PROCEED TO THE FOLLOWING EVENT
    private bool AdvanceCheck(AdvanceAction eventAdvanceType)
    {
        AdvanceAction.AdvanceTypes currentAdvanceCondition = eventAdvanceType.advanceCondition;
        GameObject currentConnectedGameObject = eventAdvanceType.connectedObject;
        float referenceFloatValue = eventAdvanceType.referencedValue;
        switch (currentAdvanceCondition)
        {
            //DONE
            case AdvanceAction.AdvanceTypes.toggleStateTrue:
                {
                    return (currentConnectedGameObject.GetComponent<InteractableItemController>().itemToggleState);
                }
            //DONE
            case AdvanceAction.AdvanceTypes.toggleStateFalse:
                {
                    return (!currentConnectedGameObject.GetComponent<InteractableItemController>().itemToggleState);
                }
            //DONE
            case AdvanceAction.AdvanceTypes.objectInInventory:
                {
                    return (currentConnectedGameObject.GetComponent<InteractableItemController>().itemInInventory);
                }
            //DONE
            case AdvanceAction.AdvanceTypes.interactionWithCharacter:
                {
                    if (currentConnectedGameObject.GetComponent<CharacterInteractionContainer>().interactedWithPlayer)
                    {
                        currentConnectedGameObject.GetComponent<CharacterInteractionContainer>().interactedWithPlayer = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            //DONE
            case AdvanceAction.AdvanceTypes.playerWithinRangeOfObjectCenter:
                {
                    Vector3 offsetVector = currentConnectedGameObject.transform.position - player.transform.position;
                    Debug.DrawRay(currentConnectedGameObject.transform.position, -offsetVector.normalized * referenceFloatValue, Color.green);
                    if (offsetVector.magnitude > referenceFloatValue)
                    {
                        Debug.DrawRay(player.transform.position, offsetVector.normalized * (offsetVector.magnitude - referenceFloatValue), Color.red);
                    }
                    return (offsetVector.magnitude <= referenceFloatValue);
                }
            //DONE
            case AdvanceAction.AdvanceTypes.grandmaWithinRangeOfObjectCenter:
                {
                    Vector3 offsetVector = grandma.transform.position - currentConnectedGameObject.transform.position;
                    //Debug.DrawRay(player.transform.position, offsetVector.normalized * referenceFloatValue, Color.yellow);
                    return (offsetVector.magnitude <= referenceFloatValue);
                }
            //DONE
            case AdvanceAction.AdvanceTypes.enemyWithinRangeOfObjectCenter:
                {
                    Vector3 offsetVector = enemy.transform.position - currentConnectedGameObject.transform.position;
                    //Debug.DrawRay(player.transform.position, offsetVector.normalized * referenceFloatValue, Color.yellow);
                    return (offsetVector.magnitude <= referenceFloatValue);
                }
            //DONE
            case AdvanceAction.AdvanceTypes.durationPassed:
                {
                    if (!waitStarted)
                    {
                        StartCoroutine(WaitDuration(referenceFloatValue));
                    }
                    if (waitComplete)
                    {
                        waitStarted = false;
                        waitComplete = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case AdvanceAction.AdvanceTypes.grandmaDialogueComplete:
                {
                    return grandma.GetComponent<GrandmaAIController>().currentCharacterBehaviorContainer.dialogueComplete;
                }
            //DONE
            default:
                {
                    return false;
                }
        }
    }

    //DONE
    private bool waitStarted = false;
    private bool waitComplete = false;
    private IEnumerator WaitDuration(float duration)
    {
        waitStarted = true;
        waitComplete = false;
        yield return new WaitForSeconds(duration);
        waitComplete = true;
    }
}

//DONE
[System.Serializable]
public struct GameEvent
{
    [Header("Event ID")]
    public string eventName;
    public int eventIndex;
    [HideInInspector]
    public bool eventStarted;
    [HideInInspector]
    public bool eventComplete;

    [Header("Event Actions")]
    public GameAction[] gameActions;

    [Header("Event Advances")]
    [Tooltip("Used to control the flow of events")]
    public AdvanceAction[] advanceActions;
}

//In Progress | Add event types
[System.Serializable]
public struct GameAction
{
    //DATA TYPES
    public enum GameEventTypes { setObjectEnableState, setObjectToggleState, setDoorLockState, setPlayerMovementState, setAnimationBoolState, setObjectPickupable, setPlayerSpawnPosition, setPlayerSpawnRotation, setEnemyBehaviorIndex, setGrandmaBehaviorIndex, setGrandpaBehaviorIndex, setBackgroundAudioIndex, callObjectAction, setObjectPosition, setObjectRotation, destroyObjectController, setObjectAnimatedState, setObjectLayer}

    [Header("Action ID")]
    public string ActionName;

    [Header("Game Actions")]
    public GameEventTypes gameActionType;

    [Header("Actions Controls")]
    [Tooltip("Used with :\nSet Component Enable State\nSet Animation Bool State\nSet Player Position Locked")]
    public bool referencedBool;
    [Tooltip("Used With:\nSet Animation Bool State")]
    public string referencedString;
    [Tooltip("Used with :\n Set Component Enable State\nSet Animation Bool State")]
    public int referencedInt;
    [Tooltip("Used with :\nSet Object Enable State\nSet Object Toggle State\nSet Door Lock State\nSet Object Interactability\nSet Object Pickupable")]
    public GameObject referencedObject;
    [Tooltip("Used with :\n Set Component Enable State\nSet Animation Bool State")]
    public Vector3 referencedVector;

    //Hidden Event State Info
    [HideInInspector]
    public bool eventActionComplete;
}

//DONE
[System.Serializable]
public struct AdvanceAction
{
    public enum AdvanceTypes { toggleStateTrue, toggleStateFalse, objectInInventory, interactionWithCharacter, playerWithinRangeOfObjectCenter, grandmaWithinRangeOfObjectCenter, enemyWithinRangeOfObjectCenter, durationPassed, grandmaDialogueComplete}

    [Header("Event Advancement")]
    public string advancementName;

    public AdvanceTypes advanceCondition;
    [Tooltip("Used With ")]
    public GameObject connectedObject;
    public float referencedValue;

    public int followingEventIndex;
}




/*
//AUDIO STRUCTS
//DONE
[System.Serializable]
public struct AudioController
{
    [Header("Audio Settings")]
    [Tooltip("How long to wait before playing the next audio clip")]
    public float audioSpacingMinWaitDuration;

    [Header("Audio Presets")]
    public AudioPreset[] audioPresets;
}

//DONE
[System.Serializable]
public struct AudioPreset
{
    public enum AudioBehaviorTypes { onInteraction, timeFromEventStart, playerWithinCoordinateRange, playerWithinObjectPositionRange, objectToggleStateTrue, ObjectToggleStateFalse }

    [Header("Audio Preset ID")]
    public string audioPresetName;
    public int audioPresetIndex;

    [Header("Audio Attributes")]
    public AudioClip audioClip;
    public AudioBehaviorTypes audioBehaviorType;
    public Vector2 audioReplayWaitDurationRange;
    public GameObject connectedObject;
}
*/
