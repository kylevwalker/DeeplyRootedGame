using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoadManager : MonoBehaviour
{
    [Header("Scene Load Info")]
    public int gameSceneIndex;

    [Header("Load Screen UI GameObjects")]
    public GameObject gameloadingSymbol;

    [Header("Current Game Load Status")]
    public bool currentlyLoadingGame;

    [Header("Load Screen Settings")]
    public float loadingSymbolBlinkIncrementValue;
    public float loadingSymbolBlinkRefreshRate;
    public float loadingSymbolMinimumTransparency;
    public float loadingSymbolMaximumTransparency;
    public float loadingSymbolBlinkPauseDuration;

    private void Start()
    {
        gameloadingSymbol.SetActive(false);
        currentlyLoadingGame = false;
    }

    [Serializable]
    public class SceneLoadIndexContainer
    {
        public int sceneLoadIndex;
        public SceneLoadIndexContainer(int saveIndex)
        {
            sceneLoadIndex = saveIndex;
        }
    }

    public void loadScene(int gameSaveIndex)
    {
        currentlyLoadingGame = true;
        canvasManagement();

        SceneLoadIndexContainer sceneLoadIndexContainer = new SceneLoadIndexContainer(gameSaveIndex);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string dataSavePath = Application.persistentDataPath + "/CurrentGameSaveIndex.Index";
        FileStream fileStream = new FileStream(dataSavePath, FileMode.Create);
        binaryFormatter.Serialize(fileStream, sceneLoadIndexContainer);
        fileStream.Close();
        print(gameSaveIndex);

        StartCoroutine(LevelCoroutine(gameSceneIndex));
    }

    //Still need to integrate saving game data (get all of type and serialize) and getting saved data
    IEnumerator LevelCoroutine(int sceneBuildNumber)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneBuildNumber);
        async.allowSceneActivation = false;

        Color newLoadingSymbolColor = gameloadingSymbol.GetComponent<RawImage>().color;
        newLoadingSymbolColor.a = 0;
        gameloadingSymbol.GetComponent<RawImage>().color = newLoadingSymbolColor;
        gameloadingSymbol.SetActive(true);
        do
        {
            while (newLoadingSymbolColor.a < loadingSymbolMinimumTransparency)
            {
                newLoadingSymbolColor.a += loadingSymbolBlinkIncrementValue;
                gameloadingSymbol.GetComponent<RawImage>().color = newLoadingSymbolColor;
                yield return new WaitForSecondsRealtime(1 / loadingSymbolBlinkRefreshRate * Time.deltaTime);
            }
            yield return new WaitForSeconds(loadingSymbolBlinkPauseDuration);
            while (newLoadingSymbolColor.a > loadingSymbolMaximumTransparency)
            {
                newLoadingSymbolColor.a -= loadingSymbolBlinkIncrementValue;
                gameloadingSymbol.GetComponent<RawImage>().color = newLoadingSymbolColor;
                yield return new WaitForSecondsRealtime(1 / loadingSymbolBlinkRefreshRate * Time.deltaTime);
            }
        }
        while (async.progress < 0.9f);
        gameloadingSymbol.SetActive(false);
        //Retrieve Game Save Data Here
        async.allowSceneActivation = true;
        yield return null;
    }

    private void canvasManagement()
    {
        gameObject.GetComponent<CursorManager>().fullCursorDisable();
        gameObject.GetComponent<GameMenuCanvasManager>().GoToCanvas(gameObject.GetComponent<GameMenuCanvasManager>().gameLoadScreenCanvas);
    }
}

