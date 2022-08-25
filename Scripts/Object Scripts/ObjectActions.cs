using UnityEngine;

public static class ObjectActions
{
    //DONE
    public static void ScrewDriverAction(GameObject gameManager, GameObject player)
    {
        GameObject potentialVent = player.GetComponent<PlayerInteractionController>().interactableItem;
        if (potentialVent != null)
        {
            if (potentialVent.GetComponentInChildren<InteractableItemController>().itemType == InteractableItemController.ItemType.ventCover)
            {
                if (potentialVent.GetComponent<InteractableItemController>().itemToggleState == false)
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerUseHeldItemKey))
                    {
                        potentialVent.GetComponent<InteractableItemController>().itemToggleState = true;
                        SimpleOpenAction(potentialVent);
                        DestroyController(potentialVent, 0);
                    }
                }
            }
        }
    }

    //DONE
    public static void KeyAction(GameObject key, GameObject gameManager, GameObject player)
    {
        if (player.GetComponent<PlayerInteractionController>().interactableItem != null)
        {
            if (player.GetComponent<PlayerInteractionController>().interactableItem.GetComponent<InteractableItemController>().objectLocked)
            {
                GameObject lockedObject = player.GetComponent<PlayerInteractionController>().interactableItem;
                if (lockedObject.GetComponent<InteractableItemController>().objectLocked == true)
                {
                    if (Input.GetKeyDown(gameManager.GetComponent<CustomPlayerControls>().playerUseHeldItemKey))
                    {
                        if (lockedObject.GetComponent<InteractableItemController>().lockKeyName == key.name)
                        {
                            AudioClip unlockAudioClip = key.GetComponent<InteractableItemController>().objectOpenAudioClip;
                            player.GetComponent<PlayerInteractionController>().StartCoroutine(player.GetComponent<PlayerInteractionController>().KeyUseAction(lockedObject, unlockAudioClip));

                            if (key.GetComponent<InteractableItemController>().destroyKeyAfterUse)
                            {
                                DestroyKey(key, gameManager, player);
                            }
                        }
                    }
                }
            }
        }
    }

    //DONE
    public static void SimpleOpenAction(GameObject gameObjectSelf)
    {
        bool toggleState = gameObjectSelf.GetComponent<InteractableItemController>().itemToggleState;
        gameObjectSelf.GetComponentInParent<Animator>().SetBool("Open State", toggleState);
        if (gameObjectSelf.GetComponentInChildren<AudioSource>() != null)
        {
            AudioClip actionAudioClip = toggleState ? gameObjectSelf.GetComponent<InteractableItemController>().objectOpenAudioClip : gameObjectSelf.GetComponent<InteractableItemController>().objectCloseAudioClip;

            if (actionAudioClip != null)
            {
                gameObjectSelf.GetComponentInChildren<AudioSource>().PlayOneShot(actionAudioClip, 1);
            }
        }
        if (gameObjectSelf.GetComponent<InteractableItemController>().referencedLight != null)
        {
            gameObjectSelf.GetComponent<InteractableItemController>().referencedLight.enabled = toggleState;
        }
    }

    //DONE
    public static void ReferencedLightStateUpdate(GameObject gameObjectSelf)
    {
        if (gameObjectSelf.GetComponent<InteractableItemController>().referencedLight != null)
        {
            gameObjectSelf.GetComponent<InteractableItemController>().referencedLight.enabled = gameObjectSelf.GetComponent<InteractableItemController>().itemToggleState;
        }
    }

    //DONE
    public static void RemoveObjectFromInventory(GameObject gameObjectSelf, bool setToPlayerPosition, bool resetVelocity, GameObject gameManager, GameObject player)
    {
        gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().ReleaseObject();

        GameObject[] initialInventoryArray = gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().objectsInInventory;
        GameObject[] newInventoryArray = new GameObject[initialInventoryArray.Length - 1];

        int index = 0;
        foreach (GameObject objectToCheck in initialInventoryArray)
        {
            if (objectToCheck != gameObjectSelf)
            {
                newInventoryArray[index] = objectToCheck;
                index++;
            }
        }

        gameManager.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().objectsInInventory = newInventoryArray;

        if (setToPlayerPosition)
        {
            gameObjectSelf.transform.position = player.transform.position;
        }

        gameObjectSelf.GetComponent<InteractableItemController>().itemInInventory = false;

        if (resetVelocity)
        {
            gameObjectSelf.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    //DONE
    public static void ObjectActionController(GameObject gameObjectSelf, GameObject gameManager)
    {
        if (!gameObjectSelf.GetComponent<InteractableItemController>().itemInInventory)
        {
            if (gameObjectSelf.GetComponent<InteractableItemController>().objectLocked)
            {
                gameObjectSelf.GetComponentInChildren<AudioSource>().PlayOneShot(gameObjectSelf.GetComponentInChildren<InteractableItemController>().lockedInteractionAudioClip, 1);
                gameObjectSelf.GetComponent<InteractableItemController>().itemToggleState = false;
            }
            else
            {
                switch (gameObjectSelf.GetComponent<InteractableItemController>().itemType)
                {
                    case InteractableItemController.ItemType.SimpleOpenObject:
                        {
                            SimpleOpenAction(gameObjectSelf);
                            break;
                        }
                    case InteractableItemController.ItemType.character:
                        {
                            if (gameObjectSelf.GetComponent<CharacterInteractionContainer>().currentlyInteractable)
                            {
                                if (gameObjectSelf.GetComponent<GrandmaAIController>().currentCharacterBehaviorContainer.dialogueStartPromptType == CharacterBehaviorContainer.DialogueStartPromptType.playerInteraction)
                                {
                                    if (gameObjectSelf.GetComponent<GrandmaAIController>().currentCharacterDialogueTextIndex == gameObjectSelf.GetComponent<GrandmaAIController>().currentCharacterBehaviorContainer.sequentialCharacterDialogue.Length - 1 && !gameObjectSelf.GetComponent<GrandmaAIController>().currentCharacterBehaviorContainer.dialogueComplete)
                                    {
                                        gameObjectSelf.GetComponent<GrandmaAIController>().ExecuteDialogue(false);
                                    }
                                    else
                                    {
                                        //Executing interactive dialogue sequence
                                        gameObjectSelf.GetComponent<GrandmaAIController>().currentCharacterBehaviorContainer.dialogueComplete = true;
                                        gameObjectSelf.GetComponent<GrandmaAIController>().ExecuteDialogue(false);
                                    }
                                }
                                else
                                {
                                    gameObjectSelf.GetComponent<CharacterInteractionContainer>().interactedWithPlayer = true;
                                }
                            }
                            break;
                        }
                    case InteractableItemController.ItemType.dumbwaiterDoor:
                        {
                            foreach (DumbwaiterFloor floor in gameManager.GetComponent<DumbwaiterController>().dumbwaiterFloors)
                            {
                                if (floor.door == gameObjectSelf)
                                {
                                    DumbwaiterDoorInteraction(floor.floorIndex, gameObjectSelf, gameManager);
                                }
                            }
                            break;
                        }
                    case InteractableItemController.ItemType.dumbwaiterRope:
                        {
                            /*
                            foreach (DumbwaiterFloor floor in gameManager.GetComponent<DumbwaiterController>().dumbwaiterFloors)
                            {
                                if (floor.door == gameObjectSelf)
                                {
                                    DumbwaiterDoorInteraction(floor.floorIndex, gameObjectSelf, gameManager);
                                }
                            }
                            */
                            DumbwaiterRopeAction(gameObjectSelf.GetComponent<InteractableItemController>().floorDestinationIndex, gameManager);
                            break;
                        }
                }
            }
        }
    }

    //WIP | ADD FUTURE OBJECTS
    public static void ObjectUseController(GameObject gameObjectSelf, GameObject gameManager, GameObject player)
    {
        switch (gameObjectSelf.GetComponent<InteractableItemController>().itemType)
        {
            case InteractableItemController.ItemType.Actionless:
                {
                    break;
                }
            case InteractableItemController.ItemType.Key:
                {
                    KeyAction(gameObjectSelf, gameManager, player);
                    break;
                }
            case InteractableItemController.ItemType.shotgun:
                {
                    break;
                }
            case InteractableItemController.ItemType.screwDriver:
                {
                    ScrewDriverAction(gameManager, player);
                    break;
                }
        }
    }

    //DONE
    public static void DestroyKey(GameObject key, GameObject gameManager, GameObject player)
    {
        player.GetComponent<PlayerHotSlotController>().RemoveObjectFromHotSlotPosition(player.GetComponent<PlayerHotSlotController>().heldObjectIndex, false);
        RemoveObjectFromInventory(key, false, true, gameManager, player);
        Object.Destroy(key);
    }

    //DONE
    public static void DestroyController(GameObject controllerObject, int newLayerIndex)
    {
        Object.Destroy(controllerObject.GetComponent<InteractableItemController>());
        controllerObject.layer = newLayerIndex;
    }

    //WIP | ADD USAGE FADE
    public static void DumbwaiterRopeAction(int destinationFloor, GameObject gameManager)
    {
        DumbwaiterFloor[] dumbwaiterFloors = gameManager.GetComponent<DumbwaiterController>().dumbwaiterFloors;
        foreach (DumbwaiterFloor floor in dumbwaiterFloors)
        {
            if (floor.floorIndex == destinationFloor)
            {
                //Sets the exited door to use interaction length because the dumbwaiter is at that floor already
                floor.door.GetComponent<InteractableItemController>().interactionUseCharge = gameManager.GetComponent<DumbwaiterController>().dumbwaiterUseInteractionLength;
                gameManager.GetComponent<DumbwaiterController>().currentFloor = destinationFloor;
                //Opens the door exited by the player
                floor.door.GetComponent<InteractableItemController>().itemToggleState = true;
                SimpleOpenAction(floor.door);

                GameObject player = gameManager.GetComponent<GameControlsManager>().player;
                player.GetComponent<CharacterController>().enabled = false;
                player.transform.position = floor.playerSpawnLocation;
                player.GetComponent<CharacterController>().enabled = true;
                player.transform.eulerAngles = floor.playerSpawnRotation;
            }
            else
            {
                //Sets all other doors to call interaction length because the dumwaiter is not at that floor and must be called up
                floor.door.GetComponent<InteractableItemController>().interactionUseCharge = gameManager.GetComponent<DumbwaiterController>().dumbdwaiterCallInteractionLength;
                floor.door.GetComponent<InteractableItemController>().itemToggleState = false;
                SimpleOpenAction(floor.door);
            }
        }
    }

    //DONE
    public static void DumbwaiterDoorInteraction(int calledDumbwaiterFloor, GameObject dumbwaiterDoor, GameObject gameManager)
    {
        foreach (DumbwaiterFloor floor in gameManager.GetComponent<DumbwaiterController>().dumbwaiterFloors)
        {
            if (floor.floorIndex == calledDumbwaiterFloor)
            {
                floor.door.GetComponent<InteractableItemController>().interactionUseCharge = gameManager.GetComponent<DumbwaiterController>().dumbwaiterUseInteractionLength;
            }
            else
            {
                floor.door.GetComponent<InteractableItemController>().interactionUseCharge = gameManager.GetComponent<DumbwaiterController>().dumbdwaiterCallInteractionLength;
            }
        }
        gameManager.GetComponent<DumbwaiterController>().currentFloor = calledDumbwaiterFloor;
        SimpleOpenAction(dumbwaiterDoor);
    }
}