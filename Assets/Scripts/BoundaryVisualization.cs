using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryVisualization : MonoBehaviour
{
    public GameObject centerEye;

    public GameObject boundaryCylinder;
    public GameObject closestCylinder;
    public GameObject trackingSpace;

    private Vector3 center;
    private Vector3[] geometry;
    private GameObject[] geometryCylinders;
    private GameObject selected;
    private Vector3 selectedPoint = Vector3.zero;
    private Vector3 trackingPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 dims = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
        center = new Vector3(dims.x / 2, dims.y / 2, dims.z / 2);
        geometry = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);

        ResetGeometry();

        selected = Instantiate(closestCylinder, selectedPoint, Quaternion.identity);
        OVRManager.VrFocusAcquired += ResetGeometry;
        OVRManager.boundary.SetVisible(false);
    }

    // Update is called once per frame
    void Update()
    {
        VisualizeClosestPoint();
        VisualizeBoundary();
    }

    // Put Closest Point Cylinder
    private void VisualizeClosestPoint()
    {
        trackingPoint = ClosestPoint();
        //selectedPoint = TrackingToWorld(trackingPoint);
        selected.transform.position = trackingPoint;

    }

    private void ResetGeometry()
    {
        geometryCylinders = new GameObject[geometry.Length];
        Vector3 point;
        for (int i = 0; i < geometry.Length; i++)
        {
            if (i % 2 == 0)
            {
                point = geometry[i];
                point = TrackingToWorld(point);
                geometryCylinders[i] = Instantiate(boundaryCylinder, point, Quaternion.identity);
            }
        }
    }

    // Show Boundary
    private void VisualizeBoundary()
    {

        Vector3 point;
        for (int i = 0; i < geometry.Length; i++)
        {
            if (geometryCylinders[i] != null)
            {
                point = geometry[i];
                point = TrackingToWorld(point);
                point.y = 0;
                geometryCylinders[i].transform.position = point;
            }
        }
    }

    private Vector3 TrackingToWorld(Vector3 tracking)
    {
        return trackingSpace.transform.TransformPoint(tracking);
    }

    private Vector3 ClosestPoint()
    {
        Vector3 point, closestPoint = Vector3.zero;
        float dist, closestDist = float.PositiveInfinity;

        for (int i = 0; i < geometry.Length; i++)
        {
            if (i % 2 == 0)
            {
                point = geometry[i];
                point = TrackingToWorld(point);
                dist = Vector3.Distance(centerEye.transform.position, point);
                if ( dist < closestDist)
                {
                    closestPoint = point;
                    closestDist = dist;
                }
            }
        }

        return closestPoint;
        //return OVRManager.boundary.TestNode(OVRBoundary.Node.Head, OVRBoundary.BoundaryType.OuterBoundary).ClosestPoint;
    }
}
