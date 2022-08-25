using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGamePauseMenuManager : MonoBehaviour
{
    [Header("Canvasses")]
    public Canvas mainPauseMenuCanvas;
    public Canvas inGameSettingsMenuCanvas;

    [Header("Referenced GameObjects")]
    public GameObject Player;

    [Header("Sub Settings Canvasses")]
    public GameObject gameplaySettingsCanvas;
    public GameObject controlsSettingsCanvas;
    public GameObject graphicsSettingsCanvas;
    public GameObject displaySettingsCanvas;
    public GameObject audioSettingsCanvas;

    [Header("Current Pause Status")]
    [HideInInspector]
    public bool playerCurrentlyPaused;
    private GameObject playerInventoryBag;
    private bool playerInInventory;

    private void Start()
    {
        gameObject.GetComponent<SceneFadeInController>().enabled = true;
        InGamePauseMenuClose();
        gameObject.GetComponent<CursorManager>().fullCursorDisable();
        playerInventoryBag = gameObject.GetComponent<GameControlsManager>().playerInventoryBag;
    }

    private void Update()
    {
        PlayerPauseInputCheck();
        playerInInventory = playerInventoryBag.GetComponent<PlayerInventoryController>().playerInInventory;
    }

    private void PlayerPauseInputCheck()
    {
        if (playerCurrentlyPaused == false)
        {
            if (Input.GetKeyDown(gameObject.GetComponent<CustomPlayerControls>().playerPauseKey))
            {
                if (playerInInventory)
                {
                    playerInventoryBag.GetComponent<PlayerInventoryController>().ExitInventory();
                }
                else
                {
                    InGamePauseMenuLaunch();
                }
            }
        }
        else
        {
            if (playerCurrentlyPaused == true)
            {
                if (Input.GetKeyDown(gameObject.GetComponent<CustomPlayerControls>().playerPauseKey))
                {
                    if (mainPauseMenuCanvas.GetComponent<Canvas>().enabled == true)
                    {
                        InGamePauseMenuClose();
                    }
                    else
                    {
                        GoToSubSettingsCanvas(gameplaySettingsCanvas);
                        GoToCanvas(mainPauseMenuCanvas);
                    }
                }
            }
        }
    }

    public void InGamePauseMenuLaunch()
    {
        GoToCanvas(mainPauseMenuCanvas);
        //gameObject.GetComponent<CursorManager>().customCursorProfile1();
        playerCurrentlyPaused = true;

        //Player.GetComponent<PlayerMovement>().enabled = false;
        Player.GetComponentInChildren<PlayerLook>().enabled = false;
        Player.GetComponentInChildren<PlayerHeadBobbing>().enabled = false;
        gameObject.GetComponent<CursorManager>().enableConfinedCursor();
    }

    public void InGamePauseMenuClose()
    {
        GoToSubSettingsCanvas(gameplaySettingsCanvas);
        gameObject.GetComponent<CursorManager>().fullCursorDisable();
        playerCurrentlyPaused = false;

        DisableAllCanvas();
        Player.GetComponent<PlayerMovement>().enabled = true;
        Player.GetComponentInChildren<PlayerLook>().enabled = true;
        Player.GetComponentInChildren<PlayerHeadBobbing>().enabled = true;
    }

    public void GoToCanvas(Canvas destinationCanvas)
    {
        DisableAllCanvas();
        destinationCanvas.GetComponent<Canvas>().enabled = true;
    }

    public void DisableAllCanvas()
    {
        mainPauseMenuCanvas.GetComponent<Canvas>().enabled = false;
        inGameSettingsMenuCanvas.GetComponent<Canvas>().enabled = false;
    }

    //Sub Canvas Management
    public void GoToSubSettingsCanvas(GameObject destinationCanvas)
    {
        DisableAllSubSettingsCanvasses();
        destinationCanvas.GetComponent<Canvas>().enabled = true;
    }
    private void DisableAllSubSettingsCanvasses()
    {
        gameplaySettingsCanvas.GetComponent<Canvas>().enabled = false;
        controlsSettingsCanvas.GetComponent<Canvas>().enabled = false;
        graphicsSettingsCanvas.GetComponent<Canvas>().enabled = false;
        displaySettingsCanvas.GetComponent<Canvas>().enabled = false;
        audioSettingsCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
