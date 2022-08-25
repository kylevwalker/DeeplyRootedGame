using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugMode;

    [Header("Connected Game Objects")]
    public Canvas playerHUDCanvas, playerHUDSlotCanvas;
    public RawImage playerCrosshair;
    public Text playerInteractionText;
    public Slider playerObjectUseChargeSlider;
    public Text playerSubtitlesText;

    [Header("Hot Slot Objects")]
    public Text[] hotSlotHUDText;

    [HideInInspector]
    private bool playerMoving;
    private bool textTransparent;
    private bool hideHUD;
    private GameObject player;
    private float textOutlineFadeIncrement;
    private char interactionKeyCharacter;
    private char grabKeyCharacter;
    [HideInInspector]
    public bool inInventory;

    //METHOD INITIALIZATION AND UPDATE
    private void Start()
    {
        player = gameObject.GetComponent<GameControlsManager>().player;
        textOutlineFadeIncrement = gameObject.GetComponent<GameControlsManager>().interactionTextOutlineTransparency / (playerInteractionText.color.a / gameObject.GetComponent<GameControlsManager>().interactionTextIncrementValue);
        UpdatePlayerInputDisplayCharacters();
        SetSubtitles("");
    }
    private void Update()
    {
        playerMoving = player.GetComponent<PlayerMovement>().playerMoving;
        PlayerCrosshairController();
        HUDVisibilityController();
        PlayerInteractionTextControls();
        UpdateHUDSlotText();
    }


    //WIP
    public void SetSubtitles(string subtitles)
    {
        if (enableDebugMode)
        {
            print(subtitles == "" ? "Reset / Empty Line" : subtitles);
        }
        playerSubtitlesText.text = subtitles;
    }


    //DONE
    private void HUDVisibilityController()
    {
        bool paused = gameObject.GetComponent<InGamePauseMenuManager>().playerCurrentlyPaused;
        inInventory = gameObject.GetComponent<GameControlsManager>().playerInventoryBag.GetComponent<PlayerInventoryController>().playerInInventory;
        if (inInventory || paused)
        {
            playerHUDCanvas.enabled = false;
            playerHUDSlotCanvas.enabled = false;
            playerInteractionText.enabled = false;
            hideHUD = true;
        }
        else
        {
            playerHUDCanvas.enabled = true;
            playerHUDSlotCanvas.enabled = true;
            playerInteractionText.enabled = true;
            hideHUD = false;
        }
    }

    //DONE
    private void UpdatePlayerInputDisplayCharacters()
    {
        interactionKeyCharacter = (char)gameObject.GetComponent<CustomPlayerControls>().playerItemInteractionKey;
        grabKeyCharacter = (char)gameObject.GetComponent<CustomPlayerControls>().playerItemGrabKey;
    }

    //DONE
    private void PlayerInteractionTextControls()
    {
        GameObject interactableObject = player.GetComponent<PlayerInteractionController>().interactableItem;
        if (interactableObject != null && interactableObject.GetComponent<InteractableItemController>() != null)
        {
            string objectName = interactableObject.GetComponent<InteractableItemController>().interactionName;
            bool pickupable = interactableObject.GetComponent<InteractableItemController>().pickupable;
            bool actionless = interactableObject.GetComponent<InteractableItemController>().itemType == InteractableItemController.ItemType.Actionless;
            bool animated = interactableObject.GetComponent<InteractableItemController>().animated;
            string interactionCharacterString = interactionKeyCharacter + (pickupable ? " | Pickup " : " | " + interactableObject.GetComponent<InteractableItemController>().interactionString) + objectName;
            bool moveable = interactableObject.GetComponent<InteractableItemController>().moveableObject;
            bool objectBeingGrabbed = player.GetComponent<PlayerInteractionController>().itemBeingGrabbed;
            string objectGrabCharacterString = grabKeyCharacter + (objectBeingGrabbed ? " | Drop " : " | Grab ") + objectName;
            string completeInteractionText = "";
            if (moveable)
            {
                completeInteractionText += objectGrabCharacterString;
            }
            if ((pickupable || !actionless) && (pickupable || animated))
            {
                completeInteractionText += moveable ? "\n" + interactionCharacterString : interactionCharacterString;
            }
            playerInteractionText.text = completeInteractionText;
            StartCoroutine(InteractionTextFadeIn());
        }
        else
        {
            StartCoroutine(InteractionTextFadeOut());
            if (textTransparent)
            {
                playerInteractionText.text = "";
            }
        }
    }

    //DONE
    private void PlayerCrosshairController()
    {
        if (!hideHUD)
        {
            Color crosshairColor = playerCrosshair.color;
            if (gameObject.GetComponent<CustomPlayerControls>().crosshairAppearWhenStill && gameObject.GetComponent<CustomPlayerControls>().crosshairAppearWhenMoving)
            {
                crosshairColor.a = gameObject.GetComponent<GameControlsManager>().minCrossHairTransparency;
                playerCrosshair.color = crosshairColor;
            }
            else if (gameObject.GetComponent<CustomPlayerControls>().crosshairAppearWhenStill)
            {
                if (playerMoving)
                {
                    StartCoroutine(FadeCrosshairOut());
                }
                else
                {
                    StartCoroutine(FadeCrosshairIn());
                }
            }
            else if (gameObject.GetComponent<CustomPlayerControls>().crosshairAppearWhenMoving)
            {
                if (playerMoving)
                {
                    StartCoroutine(FadeCrosshairIn());
                }
                else
                {
                    StartCoroutine(FadeCrosshairOut());
                }
            }
            else
            {
                crosshairColor.a = 0;
                playerCrosshair.color = crosshairColor;
            }
        }
    }
    private IEnumerator FadeCrosshairIn()
    {
        Color newCrosshairColor = playerCrosshair.color;
        if (playerCrosshair.color.a < gameObject.GetComponent<GameControlsManager>().minCrossHairTransparency)
        {
            newCrosshairColor.a += gameObject.GetComponent<GameControlsManager>().crossHairIncrementValue;
            playerCrosshair.color = newCrosshairColor;
            yield return new WaitForSecondsRealtime(1 / gameObject.GetComponent<GameControlsManager>().crosshairRefreshRate * Time.deltaTime);
        }
        yield return null;
    }
    private IEnumerator FadeCrosshairOut()
    {
        Color newCrosshairColor = playerCrosshair.color;
        if (playerCrosshair.color.a > 0)
        {
            newCrosshairColor.a -= gameObject.GetComponent<GameControlsManager>().crossHairIncrementValue;
            playerCrosshair.color = newCrosshairColor;
            yield return new WaitForSecondsRealtime(1 / gameObject.GetComponent<GameControlsManager>().crosshairRefreshRate * Time.deltaTime);
        }
        yield return null;
    }

    //DONE
    private IEnumerator InteractionTextFadeIn()
    {
        Color newTextColor = playerInteractionText.color;
        Color newEffectColor = playerInteractionText.GetComponent<Outline>().effectColor;
        if (playerInteractionText.color.a < gameObject.GetComponent<GameControlsManager>().minInteractionTextTransparency)
        {
            textTransparent = true;
            newTextColor.a += gameObject.GetComponent<GameControlsManager>().interactionTextIncrementValue;
            newEffectColor.a += textOutlineFadeIncrement;
            playerInteractionText.color = newTextColor;
            playerInteractionText.GetComponent<Outline>().effectColor = newEffectColor;
            yield return new WaitForSecondsRealtime(1 / gameObject.GetComponent<GameControlsManager>().interactionTextRefreshRate * Time.deltaTime);
        }
        textTransparent = false;
        yield return null;
    }
    private IEnumerator InteractionTextFadeOut()
    {
        Color newTextColor = playerInteractionText.color;
        Color newEffectColor = playerInteractionText.GetComponent<Outline>().effectColor;
        if (playerInteractionText.color.a > 0)
        {
            textTransparent = false;
            newTextColor.a -= gameObject.GetComponent<GameControlsManager>().interactionTextIncrementValue;
            newEffectColor.a -= textOutlineFadeIncrement;
            playerInteractionText.color = newTextColor;
            playerInteractionText.GetComponent<Outline>().effectColor = newEffectColor;
            yield return new WaitForSecondsRealtime(1 / gameObject.GetComponent<GameControlsManager>().interactionTextRefreshRate * Time.deltaTime);
        }
        textTransparent = true;
        yield return null;
    }

    //DONE
    private void UpdateHUDSlotText()
    {
        for (int i = 0; i < 3; i++)
        {
            if (gameObject.GetComponent<GameControlsManager>().player.GetComponent<PlayerHotSlotController>().hotSlotObjects[i] != null)
            {
                //hotSlotHUDText[i].text = gameObject.GetComponent<GameControlsManager>().player.GetComponent<PlayerHotSlotController>().hotSlotObjects[i].GetComponent<InteractableItemController>().interactionName;
            }
            else
            {
                //hotSlotHUDText[i].text = "";
            }
        }
    }
}
