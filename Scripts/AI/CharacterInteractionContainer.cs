using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractionContainer : MonoBehaviour
{
    [HideInInspector]
    public bool interactedWithPlayer = false;
    [HideInInspector]
    public bool currentlyInteractable = false;
}
