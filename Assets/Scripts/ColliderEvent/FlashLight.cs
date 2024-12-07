using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public GameObject flashlight;

    public AudioSource turnOn;
    public AudioSource turnOff;

    public bool on;
    public bool off;


    // Start is called before the first frame update
    void Start()
    {
        off = true;
        flashlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (off && Input.GetButtonDown("T"))
        {
            flashlight.SetActive(true);
            turnOn.Play();
            off = false;
            on = true;
        }
        else if (on && Input.GetButtonDown("T"))
        {
            flashlight.SetActive(false);
            turnOff.Play();
            off = true;
            on = false;
        }
    }
}
