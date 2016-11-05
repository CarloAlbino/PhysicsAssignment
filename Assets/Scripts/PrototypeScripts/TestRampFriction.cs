using UnityEngine;
using System.Collections;

public class TestRampFriction : MonoBehaviour {
    public float m_time = 2.0f;
    private Rigidbody m_rb;

    public float m_angle;
    public float m_friction;
    public float m_mass;

	void Start () 
    {
        m_rb = GetComponent<Rigidbody>();
	}
	
	void Update () 
    {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            Impulse();
        }
	}

    private float CalculateRampPushForce(float angle, float friction, float mass, float gravity)
    {
        return mass * gravity * Mathf.Sin(angle) + friction * mass * gravity * Mathf.Cos(angle);
    }

    private float CalculateRampAcceleration(float force, float mass)
    {
        return mass != 0 ? force / mass : 0.0f;
    }

    private float CalculateRampInitialVelocity(float finalVelocity, float acceleration, float time)
    {
        return time != 0 ? finalVelocity/time - (acceleration*time)/2 : 0.0f;
    }

    private void Impulse()
    {
        float impulseForce = CalculateRampPushForce(m_angle, m_friction, m_mass, Physics.gravity.y);
        impulseForce = CalculateRampAcceleration(impulseForce, m_mass);
        impulseForce = CalculateRampInitialVelocity(0, impulseForce, m_time);
        Vector3 m_direction = Vector3.forward *  impulseForce;
        m_rb.AddForce(m_direction, ForceMode.VelocityChange);
    }

}
