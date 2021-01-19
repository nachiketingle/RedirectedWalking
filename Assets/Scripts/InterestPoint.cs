using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestPoint : MonoBehaviour
{
    public GameObject floor;
    public GameObject pointCylinderPrefab;

    private GameObject cylinder;

    float maxX, minX;
    float maxZ, minZ;

    // Start is called before the first frame update
    void Start()
    {
        maxX = floor.transform.position.x + floor.transform.lossyScale.x / 2;
        minX = floor.transform.position.x - floor.transform.lossyScale.x / 2;
        maxZ = floor.transform.position.z + floor.transform.lossyScale.z / 2;
        minZ = floor.transform.position.z - floor.transform.lossyScale.z / 2;

        cylinder = Instantiate(pointCylinderPrefab, RandomPoint(), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            cylinder.transform.position = RandomPoint();
        }
    }

    private Vector3 RandomPoint()
    {
        return new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
    }
}
