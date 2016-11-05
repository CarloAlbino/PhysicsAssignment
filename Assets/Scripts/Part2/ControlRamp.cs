using UnityEngine;
using System.Collections;

public class ControlRamp : MonoBehaviour {

    public Transform m_rampPivot;
    public Transform m_ramplength;

    public float m_minLength, m_maxLength;

    public float m_rotationSpeed;
    public float m_stretchSpeed;

    private bool m_canControlRamp = true;

	void Update () 
    {
        if (m_canControlRamp)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal > 0.1f || horizontal < -0.1f)
            {
                StretchRamp(horizontal);
            }

            if (vertical > 0.1f || vertical < -0.1f)
            {
                RotateRamp(vertical);
            }
        }
	}

    public void StartRampControl()
    {
        m_canControlRamp = true;
    }

    public void StopRampControl()
    {
        m_canControlRamp = false;
    }

    private void RotateRamp(float direction)
    {
        Vector3 newRotation = m_rampPivot.localRotation.eulerAngles;
        int multiplier = newRotation.z > 0 ? -1 : 1;

        newRotation.z = newRotation.z + (direction * m_rotationSpeed * Time.deltaTime);

        m_rampPivot.localRotation = Quaternion.Euler(newRotation);
    }

    private void StretchRamp(float direction)
    {
        Vector3 newScale = m_ramplength.localScale;

        newScale.x = Mathf.Clamp(newScale.x + (direction * m_stretchSpeed * Time.deltaTime), m_minLength, m_maxLength);

        m_ramplength.localScale = newScale;
    }
}
