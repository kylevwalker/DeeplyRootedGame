using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Referenced GameObjects")]
    public GameObject gameManager;
    public Transform player;

    private float xAxisClamp;
    [HideInInspector]

    //METHOD INITIALIZATION AND UPDATE
    private void Awake()
    {
        xAxisClamp = 0.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        CameraRotation();
    }

    //DONE
    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(gameManager.GetComponent<GameControlsManager>().mouseXInputName) * gameManager.GetComponent<CustomPlayerControls>().playerMouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(gameManager.GetComponent<GameControlsManager>().mouseYinputName) * gameManager.GetComponent<CustomPlayerControls>().playerMouseSensitivity * Time.deltaTime;

        if (player.GetComponent<PlayerInteractionController>().itemBeingGrabbed)
        {
            mouseX = Mathf.Clamp(mouseX, -gameManager.GetComponent<GameControlsManager>().maxObjectOrbitSpeed, gameManager.GetComponent<GameControlsManager>().maxObjectOrbitSpeed);
            mouseY = Mathf.Clamp(mouseY, -gameManager.GetComponent<GameControlsManager>().maxObjectLiftSpeed, gameManager.GetComponent<GameControlsManager>().maxObjectLiftSpeed);
        }

        if (gameManager.GetComponent<CustomPlayerControls>().invertMouseX)
        {
            mouseX = -mouseX;
        }
        if (gameManager.GetComponent<CustomPlayerControls>().invertMouseY)
        {
            mouseY = -mouseY;
        }

        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        transform.Rotate(Vector3.left * mouseY);
        player.Rotate(Vector3.up * mouseX);
    }

    //DONE
    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}


