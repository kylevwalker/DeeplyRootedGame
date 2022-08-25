using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeInController : MonoBehaviour
{
    [Header("Load Screen UI GameObjects")]
    public GameObject sceneFadeInCanvas;
    private Color newFadeCanvasImageColor;

    [Header("Fade In Options")]
    public bool useFade;
    [Tooltip("Larger Values make the fade smoother but take longer to complete")]
    public float sceneColorFadeInIncrementValue; 
    [Tooltip("Larger Value increase fade speed and lag")]
    public int sceneFadeRefreshRate; 

    void Start()
    {
        gameObject.GetComponent<CursorManager>().fullCursorDisable();
        if (useFade)
        {
            sceneFadeInCanvas.GetComponent<Canvas>().enabled = true;
            newFadeCanvasImageColor = sceneFadeInCanvas.GetComponentInChildren<RawImage>().color;
            StartCoroutine(FadeCanvasOut());
        }
        else
        {
            sceneFadeInCanvas.GetComponent<Canvas>().enabled = false;
        }
    }

    private IEnumerator FadeCanvasOut()
    {
        while (sceneFadeInCanvas.GetComponentInChildren<RawImage>().color.a > 0)
        {
            newFadeCanvasImageColor.a -= sceneColorFadeInIncrementValue * Time.deltaTime;
            sceneFadeInCanvas.GetComponentInChildren<RawImage>().color = newFadeCanvasImageColor;
            yield return new WaitForSeconds(1 / sceneFadeRefreshRate);
        }
        sceneFadeInCanvas.GetComponent<Canvas>().enabled = false;
        gameObject.GetComponent<SceneFadeInController>().enabled = false;
        yield return null;
    }
}
