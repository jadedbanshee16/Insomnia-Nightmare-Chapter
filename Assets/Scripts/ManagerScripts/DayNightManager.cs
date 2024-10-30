using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DayNightManager : MonoBehaviour
{
    public GameObject sun;
    private PostProcessVolume pocVol;
    private GameManager manager;
    private ColorGrading colGrad = null;
    private LightManager[] lights;
    
    [SerializeField]
    private float cycleSpeed;
    [SerializeField]
    private float intensity;
    public float minIntensity;
    public float maxIntensity;

    public float fullDay;
    public float fullNight;

    private bool isDay;


    private void Start()
    {
        sun = GameObject.FindGameObjectWithTag("Sun");
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        pocVol = manager.gameObject.GetComponentInChildren<PostProcessVolume>();
        pocVol.profile.TryGetSettings(out colGrad);
        lights = GameObject.FindObjectsByType<LightManager>(FindObjectsSortMode.None);

        isDay = true;

        manager.setDay(true);

        intensity = maxIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        //Cycle the transform.
        sun.transform.RotateAround(Vector3.zero, Vector3.right, cycleSpeed * Time.deltaTime);
        sun.transform.LookAt(Vector3.zero);

        updateDay();

        //Change exposure depending on the position of the sun.
        colGrad.postExposure.value = Mathf.Lerp(minIntensity, maxIntensity, intensity);
        colGrad.saturation.value = Mathf.Lerp(-40, 20, intensity) * getSaturationLevel();
    }

    void updateDay()
    {
        //Find position of sun in cycleManager to change day thing. All scripts using is day will get from this script.
        if (sun.transform.position.y > 0 && !isDay)
        {
            isDay = true;
            //Set to day.
            manager.setDay(true);
            //saturation = 1;
            //Debug.Log("day");
        }
        else if (sun.transform.position.y < 0 && isDay)
        {
            isDay = false;
            manager.setDay(false);
            //saturation = 1;
            //Debug.Log("night");
        }

        //Constantly change intensity of light based on difference in numbers.
        if (manager.getDay())
        {
            //Between afternoon and night.
            //Get the difference between transform in y.
            if(sun.transform.position.y < fullDay)
            {
                float percentage = ((sun.transform.position.y / (fullDay - fullNight)));

                //float newPercentage = ((maxIntensity * 2) * percentage) - maxIntensity;

                //Debug.Log(newPercentage);

                intensity = percentage;
            }

        } else
        {
            //Between morning and day.
            //Get the difference between transform in y.
            if (sun.transform.position.y > fullNight)
            {
                float percentage = ((sun.transform.position.y / (fullDay - fullNight)));

                //float newPercentage = ((maxIntensity * 2) * percentage) - maxIntensity;

                intensity = percentage;
            }
        }
    }

    private float getSaturationLevel()
    {
        float lowestLevel = 1;

        for(int i = 0; i < lights.Length; i++)
        {
            if(lights[i].getCurrentLevel() < lowestLevel)
            {
                lowestLevel = lights[i].getCurrentLevel();
            }
        }

        return lowestLevel;
    }

    public void setSpeed(float s)
    {
        cycleSpeed = s;
    }

    public void setIntensity(float t)
    {
        intensity = Mathf.Clamp(t, minIntensity, maxIntensity);
    }

    public GameObject getSun()
    {
        return sun;
    }
}
