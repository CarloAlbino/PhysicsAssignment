using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public Projectile m_ball;
    public Transform m_target;
    public PowerMeter m_powerMeter;
    public ShootBall m_shootBall;

    private bool m_start = false;
    private bool m_useTorque = false;
    private Vector3 m_ballPosition;
    private Quaternion m_ballRotation;
    private Rigidbody m_ballRB;


    void Start()
    {
        m_ballPosition = m_ball.transform.position;
        m_ballRotation = m_ball.transform.rotation;
        m_ballRB = m_ball.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            m_useTorque = !m_useTorque;
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            m_ball.transform.position = m_ballPosition;
            m_ball.transform.rotation = m_ballRotation;
            m_ballRB.velocity = Vector3.zero;
            m_ballRB.angularVelocity = Vector3.zero;
            m_powerMeter.ResetBar();
            if(m_shootBall != null)
                m_shootBall.ResetBackboard();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!m_start)
            {
                m_powerMeter.StartPowerMeter();
                m_start = true;
            }
            else
            {
                m_powerMeter.StopPowerMeter(m_useTorque);
                m_start = false;
            }
        }

    }

    public Projectile GetBall()
    {
        return m_ball;
    }

    public float GetDistance()
    {
        return Vector3.Distance(m_ball.transform.position, m_target.position);
    }

}
