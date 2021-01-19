using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rotation : MonoBehaviour
{

    public GameObject centerEye;
    public GameObject trackingSpace;
    public GameObject cylinder;
    public GameObject canvas;
    public Text text;

    public bool useCanvas;
    public bool controllerRotation;
    public bool continuousRotation;
    public bool headRotation;

    private float angularVelocity = 100f;
    private Vector3 center;
    private Vector3[] geometry;

    private float angle;
    private float sign;

    private Vector3 prevRotation = Vector3.zero;//Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 dims = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
        center = new Vector3(dims.x / 2, dims.y / 2, dims.z / 2);
        geometry = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);


        canvas.SetActive(useCanvas);
               
    }

    // Update is called once per frame
    void Update()
    {
        CanvasUpdates();
        ControllerInputs();
        RotateWithHead();
        ContinuousRotation();
    }

    /// <summary>
    /// In-Game Canvas updates
    /// </summary>
    private void CanvasUpdates()
    {
        if (!useCanvas)
            return;

        // Canvas stuff
        // Display message
        canvas.transform.LookAt(centerEye.transform);
        canvas.transform.Rotate(0, 180, 0);
        canvas.transform.position = centerEye.transform.position + centerEye.transform.forward * 2;

        Vector3 point = ClosestPoint();

        text.text = "\nAngle: " + angle.ToString();
        text.text = text.text + "\nDistance: " + ClosestDistance();
        text.text = text.text + "\nRotate?: " + ShouldRotate();
    }

    /// <summary>
    /// Get input from controllers to rotate user
    /// </summary>
    private void ControllerInputs()
    {
        if (!controllerRotation)
            return;

        // Basic Turning from controllers
        Vector2 rotate = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (Input.GetKey(KeyCode.E))
        {
            rotate.x += 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rotate.x -= 1;
        }
        transform.RotateAround(centerEye.transform.position, Vector3.up, rotate.x * angularVelocity / 100);
    }

    /// <summary>
    /// Applies a rotation on the user whenever they look around to make them face away from the boundary
    /// </summary>
    private void RotateWithHead()
    {
        // Determine if should rotate
        if (!headRotation || !ShouldRotate())
        {
            prevRotation = centerEye.transform.localRotation.eulerAngles;
            return;
        }

        Vector3 curr = centerEye.transform.localRotation.eulerAngles;
        if(curr.y != prevRotation.y)
        {
            // The larger the differnce, the bigger the turn we make

            float per = (AmountRotated(curr, prevRotation) / 360) * 1.25f;

            
            //angle = Vector3.Angle(ClosestNormal(), centerEye.transform.forward);

            // Calculate which direction to rotate
            Vector3 lookDir = centerEye.transform.forward;
            lookDir.y = 0;
            Vector3 wallDir = ClosestNormal();
            wallDir.y = 0;


            sign = (lookDir.x * wallDir.z - lookDir.z * wallDir.x) < 0 ? 1 : -1;

            transform.RotateAround(centerEye.transform.position, Vector3.up, sign * per * angularVelocity);

            prevRotation = curr;
        }
    }

    /// <summary>
    /// Applies a continuous rotation about the user to make them face away from the boundary
    /// </summary>
    private void ContinuousRotation()
    {
        // Determine if should rotate
        if (!continuousRotation || !ShouldRotate())
            return;

        
        // Scale based on distance so max speed is 1000 and min speed is 4000
        float scale = 2500;
        /*
        if (ClosestDistance() < 1f)
        {
            scale = 1000 + (4000 - 1000) * (ClosestDistance());
        }
        */
        

        // Apply continuous rotation
        Vector3 lookDir = centerEye.transform.forward;
        lookDir.y = 0;
        Vector3 wallDir = ClosestNormal();
        wallDir.y = 0;
        
        sign = (lookDir.x * wallDir.z - lookDir.z * wallDir.x) < 0 ? 1 : -1;

        transform.RotateAround(centerEye.transform.position, Vector3.up, sign * angularVelocity / scale);


    }

    /// <summary>
    /// Returns magnitude of rotation around y-axis between the two Quaternion vectors in Quaternion
    /// </summary>
    /// <param name="curr"></param>
    /// <param name="prev"></param>
    /// <returns></returns>
    private float AmountRotated(Quaternion curr, Quaternion prev)
    {
        float rot = Mathf.Abs( (curr.y + 1) - (prev.y + 1) );

        return Mathf.Min(rot, 2 - rot);
    }

    /// <summary>
    /// Returns magnitude of rotation around y-axis between the two Vector3 in eulerAngles
    /// </summary>
    /// <param name="curr"></param>
    /// <param name="prev"></param>
    /// <returns></returns>
    private float AmountRotated(Vector3 curr, Vector3 prev)
    {
        float rot = Mathf.Abs(curr.y - prev.y);

        return Mathf.Min(rot, 360 - rot);
    }

    /// <summary>
    /// Transforms point from tracking space to world space
    /// </summary>
    /// <param name="tracking"></param>
    /// <returns></returns>
    private Vector3 TrackingToWorld(Vector3 tracking)
    {
        return trackingSpace.transform.TransformPoint(tracking);
    }

    /// <summary>
    /// Transforms direction from tracking space to world space
    /// </summary>
    /// <param name="tracking"></param>
    /// <returns></returns>
    private Vector3 TrackingToWorldDir(Vector3 tracking)
    {
        return trackingSpace.transform.TransformDirection(tracking);
    }

    /// <summary>
    /// Returns closest point on boundary to the user in world space
    /// </summary>
    /// <returns></returns>
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
                if (dist < closestDist)
                {
                    closestPoint = point;
                    closestDist = dist;
                }
            }
        }

        return closestPoint;
        //return OVRManager.boundary.TestNode(OVRBoundary.Node.Head, OVRBoundary.BoundaryType.OuterBoundary).ClosestPoint;
    }

    /// <summary>
    /// Returns normal from user to closest point on boundary
    /// </summary>
    /// <returns></returns>
    private Vector3 ClosestNormal()
    {
        return ClosestPoint() - centerEye.transform.position;
    }

    /// <summary>
    /// Return magnitude of distance between closest point on boundary to user
    /// </summary>
    /// <returns></returns>
    private float ClosestDistance()
    {
        return Vector3.Distance(centerEye.transform.position, ClosestPoint());
    }

    /// <summary>
    /// Determines if rotation should occur
    /// </summary>
    /// <returns></returns>
    private bool ShouldRotate()
    {
        return AngleBetween() < 170f;
    }

    /// <summary>
    /// Determines angle between direction user is facing and the normal to the closest point on boundary
    /// </summary>
    /// <returns></returns>
    private float AngleBetween()
    {
        // Determine if should rotate
        Vector3 norm = ClosestNormal();
        norm.y = 0;
        Vector3 curr = centerEye.transform.forward;
        curr.y = 0;
        angle = Mathf.Abs(Vector3.Angle(curr, norm));
        return angle;
    }

}
