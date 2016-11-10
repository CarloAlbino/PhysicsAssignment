using UnityEngine;
using System.Collections;

public class NewTestScript : MonoBehaviour {
    Rigidbody m_rb;
	// Use this for initialization
	void Start () {
        m_rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            m_rb.AddForce(transform.right * force, ForceMode.VelocityChange);
	}

    public float force;
}
