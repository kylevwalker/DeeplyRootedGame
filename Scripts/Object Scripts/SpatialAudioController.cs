using UnityEngine;

public class SpatialAudioController : MonoBehaviour
{
    private AudioSource objectAudioSource;
    private float audioMaxRange;
    private float baseAudioVolume;
    private GameObject gameManager;
    private GameObject player;

    [Header("Spatial Audio Volume Modulation Settings")]
    public float minBlockingObjectDimension;

    private void Start()
    {
        gameManager = FindObjectOfType<GameControlsManager>().gameObject;
        player = gameManager.GetComponent<GameControlsManager>().player;
        objectAudioSource = gameObject.GetComponentInChildren<AudioSource>();
        audioMaxRange = objectAudioSource.maxDistance;
        baseAudioVolume = objectAudioSource.volume;
    }

    private void Update()
    {
        Vector3 objectDisplacementVector = player.transform.position - gameObject.transform.position;
        float objectDisplacement = objectDisplacementVector.magnitude;

        if (objectDisplacement <= audioMaxRange)
        {
            objectAudioSource.enabled = true;
            StaticSpatialAudioModulation.modulateObjectVolume(player, gameObject, baseAudioVolume);
        }
        else
        {
            objectAudioSource.enabled = false;
        }
    }
}

public static class StaticSpatialAudioModulation
{
    public static void modulateObjectVolume(GameObject player, GameObject audioSourceObject, float baseAudioVolume)
    {
        GameObject sourceObject = audioSourceObject.GetComponentInParent<Transform>().gameObject;
        float minBlockingObjectDimension = audioSourceObject.GetComponentInParent<SpatialAudioController>().minBlockingObjectDimension;

        Vector3 objectDisplacementVector = player.transform.position - sourceObject.transform.position;
        float objectDisplacement = objectDisplacementVector.magnitude;

        RaycastHit[] audioRaycastHits = Physics.RaycastAll(sourceObject.transform.position, objectDisplacementVector, objectDisplacement);
        Debug.DrawRay(sourceObject.transform.position, objectDisplacementVector, Color.green);

        float currentVolume = baseAudioVolume;

        foreach (RaycastHit raycastHit in audioRaycastHits)
        {
            
            GameObject hitObject = raycastHit.transform.gameObject;
            Collider objectCollider = hitObject.GetComponentInParent<Collider>();
            Vector3 objectDimensions = objectCollider.bounds.size;

            float objectVolume = objectDimensions.x * objectDimensions.y * objectDimensions.z;

            Debug.Log("Hit : " + hitObject);

            if (hitObject.GetComponentInParent<Transform>().gameObject == sourceObject || objectVolume < minBlockingObjectDimension)
            {
                return;
            }
            else
            {
                currentVolume -= 0.175f;
            }
        }

        audioSourceObject.GetComponent<AudioSource>().volume = currentVolume;
    }
}
