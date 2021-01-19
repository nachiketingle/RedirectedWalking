using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    private float velocity = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        float up = 0;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
        Vector3 dir = Camera.main.transform.TransformVector(movement);
        dir.y = 0;
        dir = dir.normalized;

        dir = new Vector3(dir.x, up, dir.z);

        transform.position += dir * velocity;

        
    }
}
