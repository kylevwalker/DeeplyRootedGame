using System.Collections;
using UnityEngine;

public class PlayerHeadBobbing : MonoBehaviour
{
    [Header("Referenced Game Objects")]
    public GameObject player;
    public GameObject gameManager;

    [Header("Camera Position Read Settings")]
    public float headBobMinPositionTolerance;

    [HideInInspector]
    public float currentHeadBobbingSpeed;
    private float timer = 0.0f;
    private float playerHeadBobbingHeight;
    private float playerHeadBobbingMidPoint;
    [HideInInspector]
    public bool cameraAtLowestPoint;

    //METHOD INITIALIZATION AND UPDATE
    private void Start()
    {
        currentHeadBobbingSpeed = gameManager.GetComponent<GameControlsManager>().playerHeadBobbingBaseSpeed;
        playerHeadBobbingHeight = gameManager.GetComponent<GameControlsManager>().playerHeadBobbingHeight;
    }
    void Update()
    {
        PlayerHeadBob();
        //print("Camera At Lowest Point : " + cameraAtLowestPoint);
    }

    //DONE
    private void PlayerHeadBob()
    {
        playerHeadBobbingMidPoint = 0;
        currentHeadBobbingSpeed = gameObject.GetComponentInParent<PlayerMovement>().currentPlayerHeadBobSpeed;

        if (player.GetComponent<PlayerInteractionController>().itemBeingGrabbed)
        {
            currentHeadBobbingSpeed /= gameManager.GetComponent<GameControlsManager>().playerHeadbobSpeedDivider;
        }

        float sinWaveValue = 0.0f;
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        Vector3 cameraPosition = transform.localPosition;

        if (Mathf.Abs(horizontalAxis) == 0 && Mathf.Abs(verticalAxis) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            sinWaveValue = Mathf.Sin(timer);
            timer += currentHeadBobbingSpeed * Time.deltaTime * 10;
            if (timer > Mathf.PI * 2)
            {
                timer = 0f;
            }
        }

        if (sinWaveValue != 0)
        {
            float translateChange = sinWaveValue * playerHeadBobbingHeight;
            float totalAxes = Mathf.Abs(horizontalAxis) + Mathf.Abs(verticalAxis);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cameraPosition.y = playerHeadBobbingMidPoint + translateChange;
        }
        else
        {
            cameraPosition.y = playerHeadBobbingMidPoint;
        }
        transform.localPosition = cameraPosition;



        cameraAtLowestPoint = gameObject.transform.localPosition.y <= (headBobMinPositionTolerance * 0.00001 ) + playerHeadBobbingMidPoint - playerHeadBobbingHeight;
    }
}