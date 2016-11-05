using UnityEngine;
using System.Collections;

public class Projectile2 : MonoBehaviour {

    public Transform m_destiation;
    public Transform m_ramp;
    public float m_totalTime;

    private Rigidbody m_rb;
    private PhysicMaterial m_pm;

    private Vector3 netForce;
	// Use this for initialization
	void Start () {
        m_rb = GetComponent<Rigidbody>();
        m_pm = GetComponent<Collider>().material;
        /*netForce = CalculateStaticPushForce();
        Debug.Log(netForce);
        netForce += CalculateDynamicPushForce();
        //netForce.y = 0;
        Debug.Log(netForce);*/

        //netForce = CalculateInitialVelocity(CalculateRampPushForce());
        //Debug.Log(netForce);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            m_rb.AddForce(CalculateAll(), ForceMode.VelocityChange);//(netForce, ForceMode.VelocityChange);
	}

    private Vector3 CalculateStaticPushForce()
    {
        float theta = Mathf.Abs(m_ramp.localRotation.eulerAngles.z);
        Vector3 forceNormal = m_rb.mass * Physics.gravity * -1.0f * Mathf.Sin(theta / 57.2958f);
        Vector3 forceGravity = forceNormal * -1.0f;
        Vector3 forceStatic = forceNormal * m_pm.staticFriction * Mathf.Cos(theta / 57.2958f);

        Vector3 displacement = m_destiation.position - transform.position;
        Vector3 initialVelocity = Vector3.zero;
        Vector3 desiredAcceleration = Vector3.zero;
        desiredAcceleration = (2.0f * (displacement - initialVelocity * m_totalTime)) / (m_totalTime * m_totalTime);
        Vector3 desiredForce = desiredAcceleration * m_rb.mass;
        desiredForce += forceNormal + forceStatic;

        return desiredForce;
    }

    private Vector3 CalculateDynamicPushForce()
    {
        float theta = Mathf.Abs(m_ramp.localRotation.eulerAngles.z);
        Vector3 forceNormal = m_rb.mass * Physics.gravity * -1.0f * Mathf.Sin(theta / 57.2958f);
        Vector3 forceGravity = forceNormal * -1.0f;
        Vector3 forceDynamic = forceNormal * m_pm.dynamicFriction * Mathf.Cos(theta / 57.2958f);

        Vector3 displacement = m_destiation.position - transform.position;
        Vector3 initialVelocity = m_rb.velocity;
        Vector3 desiredAcceleration = Vector3.zero;
        desiredAcceleration = (2.0f * (displacement - initialVelocity * m_totalTime)) / (m_totalTime * m_totalTime);
        Vector3 desiredForce = desiredAcceleration * m_rb.mass;
        desiredForce += forceNormal + forceDynamic;

        return desiredForce;
    }

    private float CalculateRampPushForce()
    {
        float staticPush = m_rb.mass * Physics.gravity.y * -1.0f * Mathf.Sin(Mathf.Abs(m_ramp.localRotation.eulerAngles.z) / 57.2958f) +
                           m_pm.staticFriction * m_rb.mass * Physics.gravity.y * -1.0f * Mathf.Cos(Mathf.Abs(m_ramp.localRotation.eulerAngles.z) / 57.2958f);

        float kineticPush = m_rb.mass * Physics.gravity.y * -1.0f * Mathf.Sin(Mathf.Abs(m_ramp.localRotation.eulerAngles.z) / 57.2958f) +
                           m_pm.dynamicFriction * m_rb.mass * Physics.gravity.y * -1.0f * Mathf.Cos(Mathf.Abs(m_ramp.localRotation.eulerAngles.z) / 57.2958f);

        return (staticPush + kineticPush);
    }

    private Vector3 CalculateInitialVelocity(float pushForce)
    {
        Debug.Log(pushForce);
        float displacement = Vector3.Distance(transform.position, m_destiation.position);
        float acceleration = pushForce / m_rb.mass;



        return Vector3.left * Mathf.Sqrt(Mathf.Abs((0) - 2 * acceleration * displacement));//(displacement - (0.5f * acceleration * m_totalTime * m_totalTime)) / m_totalTime;
    }

    private Vector3 CalculateAll()
    {
        float displacement = Vector3.Distance(transform.position, m_destiation.position);
        Debug.Log("d: " + displacement);

        float acceleration = (Mathf.Abs(Physics.gravity.y) * (Mathf.Sin((Mathf.Abs(m_ramp.localRotation.eulerAngles.z) * Mathf.PI) / 180) - Mathf.Cos((Mathf.Abs(m_ramp.localRotation.eulerAngles.z) * Mathf.PI) / 180) * m_pm.staticFriction)
                            + Mathf.Abs(Physics.gravity.y) * (Mathf.Sin((Mathf.Abs(m_ramp.localRotation.eulerAngles.z) * Mathf.PI) / 180) - Mathf.Cos((Mathf.Abs(m_ramp.localRotation.eulerAngles.z) * Mathf.PI) / 180) * m_pm.dynamicFriction))
                            * displacement;
        Debug.Log("a: " + acceleration);

        float initialVelocity = displacement - 0.5f * acceleration * m_totalTime *  Time.deltaTime;//Mathf.Sqrt(2 * acceleration * displacement);
        Debug.Log("vi: " + initialVelocity);

        return Vector3.left *  Mathf.Abs(initialVelocity);
    }
}
