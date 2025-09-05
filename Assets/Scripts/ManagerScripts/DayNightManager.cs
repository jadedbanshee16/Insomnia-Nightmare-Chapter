using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightManager : MonoBehaviour
{
    public GameObject sun;
    private Volume pocVol;
    private GameManager manager;
    private ColorAdjustments colGrad = null;
    private LightManager[] lights;
    
    [SerializeField]
    private float cycleSpeed;
    private float intensity;
    private float saturation;
    public float minIntensity;
    public float maxIntensity;

    [SerializeField]
    private float[] sunIntensity;

    [SerializeField]
    private Light[] sunLights;

    public float fullDay;
    public float fullNight;

    private bool isDay;


    private void Start()
    {
        sun = GameObject.FindGameObjectWithTag("Sun");
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();



        /*sunLights = new Light[sun.transform.GetChild(0).childCount];

        for(int i = 0; i < sunLights.Length; i++)
        {
            sunLights[i] = sun.transform.GetChild(0).GetChild(i).GetComponent<Light>();
        }*/

        pocVol = manager.gameObject.GetComponentInChildren<Volume>();
        VolumeProfile prof = pocVol.sharedProfile;
        prof.TryGet<ColorAdjustments>(out colGrad);
        lights = GameObject.FindObjectsByType<LightManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        isDay = manager.getDay();

        //Find position of sun in cycleManager to change day thing. All scripts using is day will get from this script.
        if (isDay)
        {
            sun.transform.GetChild(0).gameObject.SetActive(true);
            //saturation = 1;
            intensity = 1;
            //Debug.Log("day");
        }
        else
        {
            sun.transform.GetChild(0).gameObject.SetActive(false);
            intensity = 0;
            //saturation = 1;
            //Debug.Log("night");
        }

        //intensity = maxIntensity;

        //Adjust lights to be brighter at night.
        for (int i = 0; i < lights.Length; i++)
        {
            //lights[i].setIntensity(intensity * 2);
        }
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
        saturation = Mathf.Lerp(-40, 20, intensity) * getSaturationLevel();
        colGrad.saturation.value = saturation;

        if(intensity < 0.5)
        {
            //Change intensity of lights in sun.
            for (int i = 0; i < sunLights.Length; i++)
            {
                if (i == 0)
                {
                    sunLights[i].intensity = sunIntensity[0] * (intensity * 2) + 1;
                }
                else
                {
                    sunLights[i].intensity = sunIntensity[1] * (intensity * 2);
                }
            }

            //Adjust lights to be brighter at night.
            for (int i = 0; i < lights.Length; i++)
            {
                //lights[i].setIntensity(intensity * 2);
            }
        }
    }

    void updateDay()
    {
        //Find position of sun in cycleManager to change day thing. All scripts using is day will get from this script.
        if (sun.transform.position.y > -20 && !isDay)
        {
            isDay = true;
            //Set to day.
            manager.setDay(true);
            sun.transform.GetChild(0).gameObject.SetActive(true);
            //saturation = 1;
            //Debug.Log("day");
        }
        else if (sun.transform.position.y < -20 && isDay)
        {
            isDay = false;
            manager.setDay(false);
            sun.transform.GetChild(0).gameObject.SetActive(false);
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
