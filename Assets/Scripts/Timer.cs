using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text canvasText;

    private float time;
    private bool timerOn;

    // Start is called before the first frame update
    void Start()
    {
        timerOn = false;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.One))
        {
            timerOn = !timerOn;
        }
        else if(OVRInput.GetDown(OVRInput.Button.Two))
        {
            time = 0;
            timerOn = false;
            
        }

        if(timerOn)
        {
            time += Time.deltaTime;
        }

        canvasText.text = time.ToString();
    }
}
