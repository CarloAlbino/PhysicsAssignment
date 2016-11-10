using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Ball : MonoBehaviour {

    // For Sliding
    public Transform m_desiredDestination = null;
    public Transform m_rampPivot = null;
    public Transform m_ramp = null;
    public float m_totalTime = 1.0f;

    private Rigidbody m_rb = null;
    private PhysicMaterial m_pm = null;
    private Vector3 m_netForce = Vector3.zero;
    private float m_desiredAcceleration = 0.0f;
    private bool m_isMoving = false;
    private float m_timeRemaining = 0.0f;
    private bool m_isOnFloor = false;

    private float m_power = 0;

    void Start()
    {
        m_rb = this.GetComponent<Rigidbody>();
        m_pm = GetComponent<Collider>().material;
    }

    void FixedUpdate()
    {
        if (m_netForce.sqrMagnitude > float.Epsilon)
        {
            UsingForceUpdate();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("Floor"))
        {
            m_isOnFloor = true;
        }
        else
        {
            m_isOnFloor = false;
            m_timeRemaining = m_totalTime;
        }
    }

    public void StartSimulation(float power)
    {
        m_power = power;
        m_timeRemaining = m_totalTime;
        CalculateForce();
    }

    private void CalculateForce()
    {
        if (m_isMoving)
        {
            if (m_ramp != null)
            {
                CalculateRampDynamicFrictionPushForce();
            }
            else
            {
                CalculateDynamicPushForce();
            }
        }
        else
        {
            if (m_ramp != null)
            {
                CalculateRampStaticFrictionPushForce();
            }
            else
            {
                CalculateStaticPushForce();
            }
        }
    }

    private void UsingForceUpdate()
    {
        if (m_rb.velocity.magnitude < float.Epsilon)
        {
            m_isMoving = false;
        }
        else if (m_isMoving == false && m_rb.velocity.magnitude > float.Epsilon)
        {
            m_isMoving = true;
            CalculateForce();
        }

        if (m_netForce.sqrMagnitude > float.Epsilon)
        {
            m_rb.AddForce(m_netForce * m_power);
            m_timeRemaining -= Time.fixedDeltaTime;
            Debug.Log("Time remaining: " + m_timeRemaining);
            if (m_timeRemaining < float.Epsilon)
            {
                m_netForce = Vector3.zero;
            }
        }
    }

    private void CalculateRampStaticFrictionPushForce()
    {
        //use this to get our displacement
        Collider coll = m_ramp.GetComponent<Collider>();
        Vector3 rampEdge = Vector3.zero;
        Vector3 ourPositionOnRamp = Vector3.zero;
        if (coll)
        {
            Vector3 xPos = m_ramp.position + (m_ramp.right * m_ramp.localScale.x * 5.0f);
            rampEdge = coll.ClosestPointOnBounds(xPos);
            rampEdge.y += 1.0f;
            ourPositionOnRamp = coll.ClosestPointOnBounds(transform.position);
        }

        Vector3 rampDirection = m_rampPivot.rotation.eulerAngles;
        float theta = rampDirection.z * Mathf.Deg2Rad;
        Vector3 forceGravity = m_rb.mass * Physics.gravity * Mathf.Sin(theta) * -1.0f;
        Vector3 forceNormal = m_rb.mass * Physics.gravity * Mathf.Cos(theta) * -1.0f;
        Vector3 forceStatic = forceNormal * m_pm.staticFriction;
        Vector3 direction = rampEdge - ourPositionOnRamp;
        Vector3 displacement = direction;
        direction.Normalize();
        forceStatic = direction * forceStatic.magnitude;
        forceGravity = direction * forceGravity.magnitude;
        Vector3 initialVelocity = Vector3.zero;
        Vector3 desiredAcceleration = Vector3.zero;
        desiredAcceleration = (2.0f * (displacement - (initialVelocity * m_timeRemaining))) / (m_timeRemaining * m_timeRemaining);
        Vector3 desiredForce = desiredAcceleration * m_rb.mass;
        desiredForce += forceGravity + forceStatic;
        m_netForce = desiredForce;
    }

    private void CalculateRampDynamicFrictionPushForce()
    {
        Collider coll = m_ramp.GetComponent<Collider>();
        Vector3 rampEdge = Vector3.zero;
        Vector3 ourPositionOnRamp = Vector3.zero;
        if (coll)
        {
            Vector3 xPos = m_ramp.position + (m_ramp.right * m_ramp.localScale.x * 5.0f);
            rampEdge = coll.ClosestPointOnBounds(xPos);
            rampEdge.y += 1.0f;
            ourPositionOnRamp = coll.ClosestPointOnBounds(transform.position);
        }

        Vector3 rampDirection = m_rampPivot.rotation.eulerAngles;
        float theta = rampDirection.z * Mathf.Deg2Rad;
        Vector3 forceGravity = m_rb.mass * Physics.gravity * Mathf.Sin(theta) * -1.0f;
        Vector3 forceNormal = m_rb.mass * Physics.gravity * Mathf.Cos(theta) * -1.0f;
        Vector3 forceDynamic = forceNormal * m_pm.dynamicFriction;
        Vector3 direction = rampEdge - ourPositionOnRamp;
        Vector3 displacement = direction;
        direction.Normalize();

        //our push force is being applied along the plane, our gravity and friciton forces only have a y component
        //we only want the magnitude(length aka total force) from these vectors 
        //we then want that force distributed on our directional axis, aka the ramp
        forceDynamic = direction * forceDynamic.magnitude;
        forceGravity = direction * forceGravity.magnitude;

        Vector3 initialVelocity = m_rb.velocity;
        Vector3 desiredAcceleration = Vector3.zero;
        desiredAcceleration = (2.0f * (displacement - initialVelocity * m_timeRemaining)) / (m_timeRemaining * m_timeRemaining);
        Vector3 desiredForce = desiredAcceleration * m_rb.mass;
        desiredForce += forceGravity + forceDynamic;
        m_netForce = desiredForce;
    }

    private void CalculateStaticPushForce()
    {
        Vector3 forceNormal = m_rb.mass * Physics.gravity * -1.0f;
        Vector3 forceStatic = forceNormal * m_pm.staticFriction;
        Vector3 direction = GetDirection();
        float displacement = Mathf.Abs(direction.x);
        float initialVelocity = 0.0f;
        m_desiredAcceleration = (2.0f * (displacement - initialVelocity * m_timeRemaining)) / (m_timeRemaining * m_timeRemaining);
        float desiredForce = m_desiredAcceleration * m_rb.mass;
        direction.Normalize();
        m_netForce = direction;
        m_netForce *= (desiredForce + forceStatic.magnitude);
    }

    private void CalculateDynamicPushForce()
    {
        Vector3 forceNormal = m_rb.mass * Physics.gravity * -1.0f;
        Vector3 forceDynamic = forceNormal * m_pm.dynamicFriction;
        Vector3 direction = GetDirection();
        float displacement = Mathf.Abs(direction.x);
        float initialVelocity = m_rb.velocity.x;
        m_desiredAcceleration = (2.0f * (displacement - initialVelocity * m_timeRemaining)) / (m_timeRemaining * m_timeRemaining);
        float desiredForce = m_desiredAcceleration * m_rb.mass;
        direction.Normalize();
        m_netForce = direction;
        m_netForce *= (desiredForce + forceDynamic.magnitude);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = new Vector3();
        direction = m_desiredDestination.position - transform.position;
        direction.y = 0.0f;
        direction.z = 0.0f;
        return direction;
    }

    // For Calculating Launch

    // Launch in the air aiming for a target, pass in a target
    public void LaunchProjectileTo(Transform target)
    {
        m_rb.freezeRotation = false;
        m_rb.velocity = Vector3.zero;

        float y = CalculateYInitialVelocity((target.position.y - transform.position.y) + (target.position.y - transform.position.y), Physics.gravity.y); // Add a bit to create an arc
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

}
