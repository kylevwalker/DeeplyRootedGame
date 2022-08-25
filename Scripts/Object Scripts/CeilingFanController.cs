using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingFanController : MonoBehaviour
{
    [Header("Referenced Gameobjects")]
    public GameObject gameManager;
    public bool reverseDirection;

    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, (reverseDirection ? -1 : 1) * gameManager.GetComponent<GameControlsManager>().universalCeilingFanMaxRotationSpeed * Time.deltaTime));
    }
}
