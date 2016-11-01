using UnityEngine;
using System.Collections;

public class TestRampFriction : MonoBehaviour {
    public float power;
    private Rigidbody m_rb;
	// Use this for initialization
	void Start () {
        m_rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            Impulse();
        }
	}

    private float CalculatePushForce(float angle, float friction, float mass, float gravity)
    {
        return mass * gravity * Mathf.Sin(angle) + friction * mass * gravity * Mathf.Cos(angle);
    }


    private void Impulse()
    {
        Vector3 m_direction = Vector3.right * (Mathf.Abs(Mathf.Sqrt(2)) * Mathf.Abs(Mathf.Sqrt(15)) * Mathf.Abs(Mathf.Sqrt(Mathf.Abs(-9.81f))) * Mathf.Abs(Mathf.Sqrt(0.6f)) * Mathf.Sin(45));
        m_rb.AddForce(m_direction, ForceMode.VelocityChange);
    }

}
