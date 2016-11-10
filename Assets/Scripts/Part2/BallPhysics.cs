using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BallPhysics : MonoBehaviour
{
#region Variables
    //////////////////////
    // PUBLIC VARIABLES //
    //////////////////////

    // Landing spot for the ball off the ramp
    [Tooltip("Landing spot for the ball off the ramp")]
    public Transform m_desiredDestination;
    // The ramp
    [Tooltip("Reference to the ramp")]
    public Transform m_ramp = null;
    // The ramp's pivot to get the angle from
    [Tooltip("The ramp's pivot to get the angle from")]
    public Transform m_rampPivot = null;

    public Transform m_rampEdge = null;
    // The ramp's tag
    [Tooltip("The ramp's tag")]
    public string m_rampTag = "Ramp";


    ///////////////////////////////
    // INITIAL IMPULSE VARIABLES //
    ///////////////////////////////

    // Direction the object will go in
    private Vector3 m_direction = Vector3.right;
    // The initial impulse
    private Vector3 m_startImpulse = Vector3.zero;

    ////////////////////
    // RAMP VARIABLES //
    ////////////////////

    // Total time up the ramp
    private float m_totalTime = 1.0f;
    //desired velocity will be calculated
    private Vector3 m_desiredExitVelocity = Vector3.zero;
    // Is the ball moving
    private bool m_isMoving = false;
    // Is the ball on the ramp
    private bool m_isOnRamp = false;
    // The net force applied to the ball while on the ramp
    private Vector3 m_netForce = Vector3.zero;
    // The time remaining to get up the ramp
    private float m_timeRemaining = 0.0f;
    // The ramp's angle
    private float m_theta = 0.0f;

    /////////////////////
    // BALL REFERENCES // 
    /////////////////////
    
    // Power passed in from the power bar
    private float m_power = 1;
    private Rigidbody m_rb = null;
    private PhysicMaterial m_pm = null;
#endregion Variables

#region Monobehaviour
    void Start () 
    {
        m_rb = this.GetComponent<Rigidbody>();
        m_pm = GetComponent<Collider>().material;
        m_direction = transform.forward;
        m_timeRemaining = m_totalTime;

        // For testing
        //StartSimulation(1);
	}

    void FixedUpdate()
    {
        if (m_netForce.sqrMagnitude > float.Epsilon || m_isMoving)
        {
            UsingForceUpdate();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!m_isOnRamp)
        {
            if (other.collider.CompareTag(m_rampTag))
            {
                //m_rb.velocity = Vector3.zero;
                CalculateForce();
                m_isOnRamp = true;
            }
        }
    }

    /*void OnCollisonExit(Collision other)
    {
        if(m_isOnRamp)
        {
            if(other.collider.CompareTag(m_rampTag))
            {
                m_isOnRamp = false;
            }
        }
    }*/
#endregion Monobehaviour

#region Set Simulation Going
    // Start the simulation
    public void StartSimulation(float power)
    {
        m_power = power;

        // Calculate the length of the ramp
        Vector3 displacement = GetRampToRefObjectDisplacement(m_ramp, m_desiredDestination);
        Debug.Log(displacement);

        // Calculate the angle of the ramp
        Vector3 axisOfRotation;
        m_rampPivot.localRotation.ToAngleAxis(out m_theta, out axisOfRotation);
        m_theta *= Mathf.Deg2Rad;

        // Calculate the time to reach the target from the end of the ramp
        float time = CalculateQuadraticEquation(m_theta, Physics.gravity, displacement);

        // Calculate the needed exit velocity from the ramp to reach the target
        m_desiredExitVelocity = CalculateExitVelocity(time, m_theta, displacement);
        Debug.Log(m_desiredExitVelocity);
        // Slide the ball towards the ramp
        SlideProjectile(Vector3.Distance(transform.position, m_rampPivot.position) * 1.5f * power);
    }
#endregion Set Simulation Going

#region Initial Impulse
    // Slide/Roll the projectile
    public void SlideProjectile(float distance)
    {
        m_startImpulse = CalculateImpulse(distance, Physics.gravity.y, m_pm.dynamicFriction);

        m_rb.freezeRotation = true;
        m_rb.AddForce(m_startImpulse, ForceMode.VelocityChange);
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
            Debug.LogWarning("Start impulse is 0.  There is either no gravity or 0 friction.");
            return Vector3.zero;
        }
    }
#endregion Initial Impulse

#region Ramp and Launch to Target
    // Called in Fixed Update, calculate and add force continuously while on the ramp
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
            m_rb.AddForce(m_netForce);
            m_timeRemaining -= Time.fixedDeltaTime;
            Debug.Log("Time remaining: " + m_timeRemaining);
            if (m_timeRemaining < float.Epsilon)
            {
                m_netForce = Vector3.zero;
            }
        }
    }
    
    // Caluculate the forces needed to go up the ramp
    private void CalculateForce()
    {
        /*if (m_isMoving)
        {
            //if (m_isOnRamp)
            {
                CalculateRampDynamicFrictionPushForce();
            }
        }
        else
        {
            //if (m_isOnRamp)
            {
                CalculateRampStaticFrictionPushForce();
            }
        }*/
        CalculateRampDynamicFrictionPushForce();
    }

    // Calculate the distance from the edge of the ramp to the target, used in the jump off of the ramp
    private Vector3 GetRampToRefObjectDisplacement(Transform ramp, Transform refObject)
    {
        /*Vector3 rampEdge = Vector3.zero;

        rampEdge = ramp.position;
        Vector3 rampDirection = ramp.right * ramp.lossyScale.x/*.localScale.x*//* * 0.5f;
        rampEdge += rampDirection;

        return refObject.position - rampEdge;*/

        return refObject.position - m_rampEdge.position;
    }

    // Calculates the time it will take to reach the target from the edge of the ramp
    private float CalculateQuadraticEquation(float theta, Vector3 acceleration, Vector3 displacement)
    {
        float time = 0.0f;
        //Viy = (x * tan theta) / t
        //0.5at^2 + 0t + xtantheta - d = 0

        time = (Mathf.Sqrt(-4.0f * 0.5f * acceleration.y * (displacement.x * Mathf.Tan(theta) - displacement.y))) / acceleration.y;
        time = Mathf.Abs(time);

        return time;
    }

    // Calculates the exit velocity needed to launch off the edge of the ramp to the target
    private Vector3 CalculateExitVelocity(float time, float theta, Vector3 displacement)
    {
        float vX = displacement.x / time;
        float v = vX / Mathf.Cos(theta);
        float vY = v * Mathf.Sin(theta);

        return new Vector3(vX, vY, 0);
    }

    // Calculates the force needed to overcome the static friction to start moving the ball up the ramp
    private void CalculateRampStaticFrictionPushForce()
    {
        //use this to get our displacement
        Collider coll = m_ramp.GetComponent<Collider>();
        Vector3 rampEdge = Vector3.zero;
        Vector3 ourPositionOnRamp = Vector3.zero;
        if (coll)
        {
            Vector3 xPos = m_ramp.position + (m_ramp.right * m_ramp.localScale.x);
            rampEdge = m_rampEdge.position;
            //rampEdge = coll.ClosestPointOnBounds(xPos);
            //rampEdge.y += 1.0f;
            ourPositionOnRamp = coll.ClosestPointOnBounds(transform.position);
        }

        Vector3 forceGravity = m_rb.mass * Physics.gravity * Mathf.Sin(m_theta) * -1.0f;
        Vector3 forceNormal = m_rb.mass * Physics.gravity * Mathf.Cos(m_theta) * -1.0f;
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
        //force required to move up the ramp, if there is no friciton to overcome
        desiredForce += forceGravity + forceStatic;
        m_netForce = desiredForce;
        m_netForce *= m_power; // m_power comes from the power meter and will affect the launch velocity
    }

    // Calculates the force needed to keep moving the ball up the ramp
    private void CalculateRampDynamicFrictionPushForce()
    {
        Collider coll = m_ramp.GetComponent<Collider>();
        Vector3 rampEdge = Vector3.zero;
        Vector3 ourPositionOnRamp = Vector3.zero;
        if (coll)
        {
            Vector3 xPos = m_ramp.position  + (m_ramp.right * m_ramp.localScale.x);
            //rampEdge = coll.ClosestPointOnBounds(xPos);
            //rampEdge.y += 1.0f;
            rampEdge = m_rampEdge.position;
            ourPositionOnRamp = coll.ClosestPointOnBounds(transform.position);
        }
        Debug.Log(rampEdge);
        Vector3 forceGravity = m_rb.mass * Physics.gravity * Mathf.Sin(m_theta) * -1.0f;
        Vector3 forceNormal = m_rb.mass * Physics.gravity * Mathf.Cos(m_theta) * -1.0f;
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
        Vector3 finalVel = m_desiredExitVelocity;
        Vector3 newAcceleration = ((finalVel.normalized * finalVel.sqrMagnitude) - (initialVelocity.normalized * initialVelocity.sqrMagnitude)) / (2.0f * displacement.magnitude);

        //force required to move up the ramp, if there is no friciton to overcome
        Vector3 desiredForce = newAcceleration * m_rb.mass;
        desiredForce += forceGravity + forceDynamic;

        m_timeRemaining = (finalVel.magnitude - initialVelocity.magnitude) / newAcceleration.magnitude;

        m_netForce = desiredForce;
        m_netForce *= m_power; // m_power comes from the power meter and will affect the launch velocity
    }
#endregion Ramp and Launch to Target

#region Shoot The Ball
    // Launch in the air aiming for a target, pass in a target
    public void LaunchProjectileTo(Transform target)
    {
        m_rb.freezeRotation = false;
        m_rb.velocity = Vector3.zero;

        float y = CalculateYInitialVelocity((target.position.y - transform.position.y) + (target.position.y - m_rampPivot.transform.position.y), Physics.gravity.y); // Add a bit to create an arc
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
#endregion Shoot The Ball
}
