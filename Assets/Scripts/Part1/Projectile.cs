using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    ////////////////
    // First Part //
    ////////////////

    // Direction the object will go in
    public Vector3 m_direction = Vector3.right;
    // Axis around which the object will rotate
    public Vector3 m_rotationAxis = Vector3.forward;

    // For Friction
    public Collider m_plane;
    private float m_planeFriction;

    // For adding force
    private Vector3 m_impulse;
    private Vector3 m_torque;
    private Vector3 m_radius;

    // To calculate distance
    private Vector3 m_initialPosition;
    private float m_distance;

    /*
    /////////////////
    // Second Part //
    /////////////////

    // For Launching in the air
    private Transform m_launchTarget;
    private Vector3 m_launchVelocity;
    private float m_xVelocityInitial;
    private float m_yVelocityInitial;
    private float m_xDistance;
    private float m_yDistance;
    */

    ////////////////////////
    // Rigidbody reference//
    ////////////////////////

    private Rigidbody m_rb;

	void Start () 
    {
        // Set the reference to the rigidbody
        m_rb = GetComponent<Rigidbody>();
        // Set the friction coefficient
        m_planeFriction = m_plane.material.dynamicFriction;
        // Normalize the direction vector
        m_direction = m_direction.normalized;
        // Get the radius of the projectile;
        m_radius = transform.localScale;
        m_radius *= 0.5f;
        // Save the start position
        m_initialPosition = transform.position;

        m_direction = transform.forward;
        m_rotationAxis = transform.right;
	}

    // Slide/Roll the projectile
    public void SlideProjectile(float distance, bool useTorque = false)
    {
        m_distance = distance;
        m_impulse = CalculateImpulse(distance, Physics.gravity.y, m_planeFriction);

        if (!useTorque)
        {
            m_rb.freezeRotation = true;
            m_rb.AddForce(m_impulse, ForceMode.VelocityChange);
        }
        else
        {
            m_torque = CalculateTorque(m_impulse, m_radius);
            m_rb.AddTorque(m_torque, ForceMode.VelocityChange);
        }
    }

    // Calculate Impulse for sliding on a plane
    private Vector3 CalculateImpulse(float distance, float gravity, float friction)
    {
        if (gravity != 0 || friction != 0)
        {
            return m_direction * (Mathf.Abs(Mathf.Sqrt(2)) * Mathf.Abs(Mathf.Sqrt(distance)) * Mathf.Abs(Mathf.Sqrt(Mathf.Abs(gravity))) * Mathf.Abs(Mathf.Sqrt(friction)));
        }
        else
        {
            return Vector3.zero;
        }
    }

    // Calculate Torque for rolling on a plane
    private Vector3 CalculateTorque(Vector3 linearVelocity, Vector3 radius)
    {
        return radius.magnitude != 0 ? m_rotationAxis * (linearVelocity.magnitude / radius.magnitude) : Vector3.zero;
    }


    // Launch in the air aiming for a target, pass in a target
    public void LaunchProjectileTo(Transform target)
    {
        m_rb.freezeRotation = false;
        m_rb.velocity = Vector3.zero;

        float y = CalculateYInitialVelocity((target.position.y - transform.position.y) + (target.position.y - m_plane.transform.position.y), Physics.gravity.y); // Add a bit to create an arc
        float time = CalculateTimeToPeak(y, Physics.gravity.y) * 2;
        Vector3 impulse = CalculateXZInitialVelocity(target.position.x - transform.position.x, target.position.z - transform.position.z, time);
        impulse.y = y;

        m_rb.AddForce(impulse, ForceMode.VelocityChange);
    }

    // Calculate the initial velocity on Y (to get over a wall, creates an arc)
    private float CalculateYInitialVelocity(float yDisplacement, float gravity, float finalYVelocity = 0.0f)
    {
        return yDisplacement != 0 ? Mathf.Sqrt(Mathf.Abs((finalYVelocity * finalYVelocity) - 2 * gravity * yDisplacement)) : 0.0f;
    }

    // Calculate the time it takes to reach the peak of Y
    private float CalculateTimeToPeak(float initialYVelocity, float gravity, float finalYVelocity = 0.0f)
    {
        return gravity != 0 ? (finalYVelocity - initialYVelocity) / gravity : 0.0f;
    }

    // Calculate the initial velocity on X and Z
    private Vector3 CalculateXZInitialVelocity(float xDisplacement, float zDisplacement, float time)
    {
        return time != 0 ? new Vector3(xDisplacement / time, 0.0f, zDisplacement / time) : Vector3.zero;
    }

    // Functions to find the actual distance including the offset of scale
    private float CalculateXDistance(Transform pointA, Transform pointB)
    {
        // Start with 0 vectors.
        Vector3 A = Vector3.zero;
        Vector3 B = Vector3.zero;

        // Calculate and pass in the x positions.
        A.x = pointA.position.x + pointA.localScale.x * 0.5f;
        B.x = pointB.position.x - pointB.localScale.x * 0.5f;

        // Calculate and return the distance.
        return Vector3.Distance(A, B);
    }

    private float CalculateYDistance(Transform pointA, Transform pointB)
    {
        // Start with 0 vectors.
        Vector3 A = Vector3.zero;
        Vector3 B = Vector3.zero;

        // Calculate and pass in the y positions.
        A.y = pointA.position.y + pointA.localScale.y * 0.5f;
        B.y = pointB.position.y - pointB.localScale.y * 0.5f;

        // Calculate and return the distance.
        return Vector3.Distance(A, B);
    }

    private float CalculateDistance(Transform pointA, Transform pointB)
    {
        // Start with 0 vectors.
        Vector3 A = Vector3.zero;
        Vector3 B = Vector3.zero;

        // Calculate and pass in the x positions.
        A.x = pointA.position.x + pointA.localScale.x * 0.5f;
        B.x = pointB.position.x - pointB.localScale.x * 0.5f;

        // Calculate and pass in the y positions.
        A.y = pointA.position.y + pointA.localScale.y * 0.5f;
        B.y = pointB.position.y - pointB.localScale.y * 0.5f;

        // Calculate and return the distance.
        return Vector3.Distance(A, B);
    }

    /*
    public Transform ramp;
    public Transform pivot;

    public void LaunchUpRamp()
    {
        float yHeight = Mathf.Abs(Mathf.Sin(pivot.localRotation.eulerAngles.z / 57.2958f) * ramp.localScale.x);
        Debug.Log("Ramp local scale x: " + ramp.localScale.x);
        Debug.Log("Ramp x direction: " + ramp.TransformDirection(ramp.right));
        Debug.Log("Player x direction: " + m_direction);
        Debug.Log("Ramp angle: " + pivot.localRotation.eulerAngles.z);
        Debug.Log("Ramp height: " + yHeight);
        float y = CalculateYInitialVelocity(yHeight * 3, Mathf.Abs(Physics.gravity.y));
        Debug.Log("Initial YieldInstruction velocity: " + y);
        float time = CalculateTimeToPeak(y, Mathf.Abs(Physics.gravity.y));
        Vector3 edgeOfRamp = transform.TransformPoint(transform.position.x + transform.localScale.x, transform.position.y + transform.localScale.y,
                                                        transform.position.z + transform.localScale.z);
        Debug.Log(edgeOfRamp);
        Vector3 impulse = CalculateXZInitialVelocity(edgeOfRamp.x - target.position.x, edgeOfRamp.z- target.position.z, time * 2);
        impulse.y = y;


        // Get final velocity x.  This is the initial veloctiy of the arc when leaving the jump.
        float finalVelocityX = impulse.x;
        float finalVelocityZ = impulse.z;
        Debug.Log("fvx: " + finalVelocityX);

        Vector3 rampImpulse = CalculateImpulseUpARamp(ramp.localScale.x, pivot.localRotation.eulerAngles.z,
                                                    m_planeFriction, Physics.gravity.y, finalVelocityX);
        Debug.Log("Ramp Impulse: " + rampImpulse);
        m_rb.AddForce(rampImpulse, ForceMode.VelocityChange);
        //m_rb.AddTorque(CalculateTorque(rampImpulse, m_radius), ForceMode.VelocityChange);
    }

    // Calculate the impulse when there is a ramp involved
    private Vector3 CalculateImpulseUpARamp(float distance, float angle, float friction, float gravity, float finalXVelocity)
    {
        float acceleration = Mathf.Abs(gravity) * distance * (Mathf.Sin(angle / 57.2958f) * (Mathf.Cos(angle / 57.2958f) * friction));
        float initialXVelocity = Mathf.Sqrt(Mathf.Abs((finalXVelocity * finalXVelocity) - 2 * acceleration * distance));
        Debug.Log(initialXVelocity);

        return m_direction * initialXVelocity;
    }

    // Calculate ramp angle
    private float CalculateRampAngle(Vector3 floorVector, Vector3 rampVector)
    {
        return Mathf.Acos(Vector3.Dot(floorVector, rampVector) / Vector3.Magnitude(floorVector) * Vector3.Magnitude(rampVector)) * 57.2958f;
    }

    // Calculate the torque when there is a ramp involved

    */

}
