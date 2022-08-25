using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuCanvasManager : MonoBehaviour
{
    [Header("Main Canvasses")]
    public GameObject startMenuCanvas;
    public GameObject loadGameMenuCanvas;
    public GameObject settingsMenuCanvas;
    public GameObject gameLoadScreenCanvas;

    [Header("Sub Settings Canvasses")]
    public GameObject gameplaySettingsCanvas;
    public GameObject controlsSettingsCanvas;
    public GameObject graphicsSettingsCanvas;
    public GameObject displaySettingsCanvas;
    public GameObject audioSettingsCanvas;

    [Header("Sub Components")]
    public GameObject gameQuitWarningImage;

    private void Awake()
    {
        GoToCanvas(startMenuCanvas);
        ExitQuitGamePrompt();
    }

    private void Update()
    {
        CanvasEscapeKeyInputCheck();
    }

    public void GoToCanvas(GameObject destinationCanvas)
    {
        DisableAllMainCanvasses();
        ExitQuitGamePrompt();
        destinationCanvas.GetComponent<Canvas>().enabled = true;
    }

    private void DisableAllMainCanvasses()
    {
        startMenuCanvas.GetComponent<Canvas>().enabled = false;
        loadGameMenuCanvas.GetComponent<Canvas>().enabled = false;
        settingsMenuCanvas.GetComponent<Canvas>().enabled = false;
        gameLoadScreenCanvas.GetComponent<Canvas>().enabled = false;
    }

    //Sub Setting canvas Management
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

    public void QuitGame()
    {
        Debug.Log("Game Quit, Only Works In Export");
        Application.Quit();
    }

    private void CanvasEscapeKeyInputCheck()
    {
        if (Input.GetKeyDown(gameObject.GetComponent<CustomPlayerControls>().playerPauseKey) && !gameObject.GetComponent<SceneLoadManager>().currentlyLoadingGame)
        {
            if (loadGameMenuCanvas.GetComponent<Canvas>().enabled == true || settingsMenuCanvas.GetComponent<Canvas>().enabled == true)
            {
                GoToCanvas(startMenuCanvas);
            } 
            else if (startMenuCanvas.GetComponent<Canvas>().enabled == true)
            {
                if (gameQuitWarningImage.activeSelf)
                {
                    ExitQuitGamePrompt();
                }
                else
                {
                    EnableQuitGamePrompt();
                }
            }
        }
    }

    public void EnableQuitGamePrompt()
    {
        gameQuitWarningImage.SetActive(true);
    }

    public void ExitQuitGamePrompt()
    {
        gameQuitWarningImage.SetActive(false);
    }
}
