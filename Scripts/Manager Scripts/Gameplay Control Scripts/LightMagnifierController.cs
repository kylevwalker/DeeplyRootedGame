using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMagnifierController : MonoBehaviour
{
    [Header("Referenced Gameobjects")]
    public GameObject player;
    public GameObject lightIntakePlane;
    public Light outputLight;

    [Header("Object Settings")]
    public float objectLightStrengthMultiplier;
    public float objectAccuracyMultiplier;
    public float objectEffectiveRange;

    void Update()
    {
        if (gameObject.GetComponent<InteractableItemController>().objectInHand || player.GetComponent<PlayerInteractionController>().grabbedGameObject == gameObject)
        {

        }
    }
}
