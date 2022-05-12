using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSetting : MonoBehaviour
{
    public ParticleSystem Rain;
    public float time;
    public bool Enter;
    public bool Run;
    public float hSliderValue;

    void Start()
    {
        time = -1;
        Run = true;
        hSliderValue = Rain.emission.rateOverTime.constant;
    }

    void Update()
    {
        if (time >= 0)
        {
            time += Time.deltaTime;
            if (time >= 2)
            {
                Run = true;
                Enter = false;
                time = -1;
            }
        }

        if (Run)
        {
            Run = false;
            if (Enter)
            {
                Rain.Stop();
                var rain = Rain.emission;
                if (Level_1.minRain)
                {
                    rain.rateOverTime = hSliderValue /5;
                }
                else
                {
                    rain.rateOverTime = hSliderValue;
                }
                Rain.Play();
            }
            else
            {
                Rain.Stop();
                var rain = Rain.emission;
                rain.rateOverTime = 0;
                Rain.Play();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                Enter = true;
                Run = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                time = 0;
            }
        }
    }
}
