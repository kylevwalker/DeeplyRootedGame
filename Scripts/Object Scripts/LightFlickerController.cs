using System.Collections;
using UnityEngine;


public class LightFlickerController : MonoBehaviour
{
    [Header("Light Flicker Settings")]
    public float flickerFrequencyMaxWait;
    public float flickerFrequencyMinWait;
    [Range(1, 10)]
    public int flickerRepetitionMax;
    [Range(1, 10)]
    public int flickerRepetitionMin;
    public float flickerOffDurationMax;
    public float flickerOffDurationMin;
    public float flickerMaxTrailingOnDuration;
    public float flickerMinTrailingOnDuration;

    //Object References
    private Light[] objectLights;
    private AudioSource audioSource;



    void Start()
    {
        objectLights = gameObject.GetComponentsInChildren<Light>();
        audioSource = gameObject.GetComponentInChildren<AudioSource>();

        StartCoroutine(LightUpdate());
    }



    private IEnumerator LightUpdate()
    {
        while (true)
        {
            Flicker[] flickerSequence = GenerateFlickerSequence();

            StartCoroutine(FlickerFunction(flickerSequence));

            yield return new WaitForSeconds(Random.Range(flickerFrequencyMinWait, flickerFrequencyMaxWait));

            print("Update Flicker Here");
        }
    }


    //DONE
    private Flicker[] GenerateFlickerSequence()
    {
        Flicker[] currentFlickerSequence = new Flicker[Random.Range(flickerRepetitionMin, flickerRepetitionMax)];

        for (int index = 0; index < currentFlickerSequence.Length; index++)
        {
            Flicker newFlicker;
            newFlicker.offDuration = Random.Range(flickerOffDurationMin, flickerOffDurationMax);
            newFlicker.trailingOnDuration = Random.Range(flickerMinTrailingOnDuration, flickerMaxTrailingOnDuration);

            currentFlickerSequence[index] = newFlicker;
        }

        return currentFlickerSequence;
    }


    //DONE
    private IEnumerator FlickerFunction(Flicker[] flickerSequence)
    {
        foreach (Flicker flicker in flickerSequence)
        {
            audioSource.Stop();

            foreach (Light light in objectLights)
            {
                light.enabled = false;
            }

            yield return new WaitForSeconds(flicker.offDuration);

            audioSource.Play();

            foreach (Light light in objectLights)
            {
                light.enabled = true;
            }

            yield return new WaitForSeconds(flicker.trailingOnDuration);
        }
    }
}

public struct Flicker
{
    public float offDuration;
    public float trailingOnDuration;
}