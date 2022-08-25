using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    //Profile 1
    [Header("Custom Cursor Profiles")]
    public CustomCursorProfile[] customCursorProfile;

    [Header("Debug")]
    public bool enableDebugMode;

    private CursorMode customCursorMode = CursorMode.Auto;

    private void Awake()
    {
        //customCursorProfile1();
        enableConfinedCursor();
    }

    //Cursor Profile Assignment Functions
    public void SetCursorProfile(int CursorProfileArrayIndex)
    {
        Cursor.SetCursor(customCursorProfile[CursorProfileArrayIndex].cursorProfileTexture , customCursorProfile[CursorProfileArrayIndex].cursorProfileHotspotOffset, customCursorMode);
    }

    //Cursor Ability Functions
    public void enableConfinedCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (enableDebugMode)
            print("Cursor Enabled");
    }

    public void fullCursorDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Locked;
        if (enableDebugMode)
            print("Cursor Disabled");
    }

    private void Update()
    {
        if (enableDebugMode)
        {
            print("Cursor Lock State : " + Cursor.lockState + " | Cursor Visible : " + Cursor.visible);
        }
    }
}

[System.Serializable]
public struct CustomCursorProfile
{
    public string cursorProfileName;
    public Texture2D cursorProfileTexture;
    public Vector2 cursorProfileHotspotOffset;
}
