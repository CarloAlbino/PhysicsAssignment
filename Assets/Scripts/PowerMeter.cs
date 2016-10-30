using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerMeter : MonoBehaviour {
    public float m_barSpeed = 5.0f;
    private Projectile m_ball;

    private float m_power = 0.0f;
    private Slider m_slider;
    private bool m_startPowerMeter = false;
    private Game m_game;

	void Start () 
    {
        m_game = FindObjectOfType<Game>();
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
        m_ball = m_game.GetBall();
        m_slider.maxValue = m_game.GetDistance() + (m_game.GetDistance() * 0.2f);
        m_startPowerMeter = true;
    }

    public void StopPowerMeter(bool useTorque = false)
    {
        m_startPowerMeter = false;
        m_ball.SlideProjectile(m_slider.value, useTorque);
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
