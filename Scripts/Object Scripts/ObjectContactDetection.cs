using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContactDetection : MonoBehaviour
{
    //DONE
    private void OnCollisionEnter(Collision collision)
    {
        gameObject.GetComponent<InteractableItemController>().objectContact = true;
    }

    //DONE
    private void OnCollisionExit(Collision collision)
    {
        gameObject.GetComponent<InteractableItemController>().objectContact = false;
    }
}
