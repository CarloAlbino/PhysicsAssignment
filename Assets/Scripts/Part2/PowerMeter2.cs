using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerMeter2 : MonoBehaviour {

    public float m_barSpeed = 5.0f;
    [Range(0.0f, 1.0f)]
    public float m_extraPercentage = 0.3f;

    private float m_power = 0.0f;
    private Slider m_slider;
    private bool m_startPowerMeter = false;

	void Start () 
    {
        m_slider = GetComponent<Slider>();
	}

	void Update () 
    {
        if (m_startPowerMeter)
        {
            ActivatePowerMeter();
        }
	}

    public void StartPowerMeter()
    {
        m_startPowerMeter = true;
    }

    public float StopPowerMeter()
    {
        m_startPowerMeter = false;
        return (m_slider.value + m_slider.maxValue * m_extraPercentage)/m_slider.maxValue;
    }

    public void ResetBar()
    {
        m_slider.value = 0;
        m_power = 0;
    }

    private void ActivatePowerMeter()
    {
        m_power += m_barSpeed * Time.deltaTime;
        m_slider.value = Mathf.PingPong(m_power, m_slider.maxValue);
    }
}
