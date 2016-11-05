using UnityEngine;
using System.Collections;

public class SimulationControl : MonoBehaviour {

    public Ball m_ball;
    public ControlRamp m_ramp;
    public PowerMeter2 m_powerMeter;

    private bool m_startSim = false;
    private float m_power = 0;

    private Vector3 m_ballPosition;
    private Quaternion m_ballRotation;

    void Start()
    {
        // Save the position and rotation of the ball for reseting the simulation
        m_ballPosition = m_ball.transform.position;
        m_ballRotation = m_ball.transform.rotation;
    }

	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // Return ramp control to the player
            m_power = m_powerMeter.StopPowerMeter();
            m_powerMeter.ResetBar();
            m_ramp.StartRampControl();
            m_power = 0;
            m_startSim = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!m_startSim)
            {
                // Start power bar and stop ramp control
                m_ramp.StopRampControl();
                m_powerMeter.StartPowerMeter();
                m_startSim = true;
            }
            else
            {
                // Stop power bar and start the simulation
                m_power = m_powerMeter.StopPowerMeter();
                m_ball.StartSimulation(m_power);
                m_startSim = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            // Reset the simulation
            m_ball.transform.position = m_ballPosition;
            m_ball.transform.rotation = m_ballRotation;
            m_powerMeter.ResetBar();
            m_ramp.StartRampControl();
            m_power = 0;
            m_startSim = false;
        }
	}
}
