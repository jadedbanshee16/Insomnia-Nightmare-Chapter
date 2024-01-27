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
    
    public float cycleSpeed;
    public float intensity;
    public float minIntensity;
    private bool isChange;

    public float fullDay;
    public float fullNight;

    private Color col;

    private void Start()
    {
        sun = GameObject.FindGameObjectWithTag("Sun");
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        pocVol = manager.gameObject.GetComponentInChildren<PostProcessVolume>();
        pocVol.profile.TryGetSettings(out colGrad);

        isChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Cycle the transform.
        sun.transform.RotateAround(Vector3.zero, Vector3.right, cycleSpeed * Time.deltaTime);
        sun.transform.LookAt(Vector3.zero);

        updateDay();

        //Using default color.
        colGrad.colorFilter.value = ambientLightChange();
    }

    void updateDay()
    {
        //Find position of sun in cycleManager to change day thing. All scripts using is day will get from this script.
        if (sun.transform.position.y > 0 && !isChange)
        {
            isChange = true;
            //Set to day.
            manager.setDay(true);
        }
        else if (sun.transform.position.y < 0 && !isChange)
        {
            isChange = true;
            manager.setDay(false);
        } else
        {
            isChange = false;
        }

        //Constantly change intensity of light based on difference in numbers.
        if (manager.getDay())
        {
            //Between afternoon and night.
            //Get the difference between transform in y.
            if(sun.transform.position.y < fullDay)
            {
                intensity = Mathf.Clamp(((sun.transform.position.y / (fullDay - fullNight))), minIntensity, 1f);
            }

        } else
        {
            //Between morning and day.
            //Get the difference between transform in y.
            if (sun.transform.position.y > fullNight)
            {
                intensity = Mathf.Clamp(((sun.transform.position.y / (fullDay - fullNight))), minIntensity, 1f);
            }
        }
    }

    Color ambientLightChange()
    {
        //Flip intensity and set it between 255 and 25.
        float inten = intensity;
        Color color = new Color(inten, inten, inten);

        return color;
    }
}
