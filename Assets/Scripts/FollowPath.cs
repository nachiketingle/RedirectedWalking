using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public float vel;
    public bool move;

    private Vector3 orgPos;
    // Start is called before the first frame update
    void Start()
    {
        orgPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            move = !move;
        }
        else if(OVRInput.GetDown(OVRInput.Button.Four))
        {
            move = false;
            transform.position = orgPos;
        }


    }

    private void FixedUpdate()
    {
        if (move)
        {
            transform.Translate(vel, 0, 0);
        }
    }
}
