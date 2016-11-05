using UnityEngine;
using System.Collections;

public class ShootBall : MonoBehaviour {

    public Transform m_target;

    private bool m_canShoot = false;

    public GameObject m_playerWait, m_playerJump;
    public Transform m_shootPosition;

    private Color m_oldColor;
    public MeshRenderer m_backboard;
    public GameObject m_light;

    void Start()
    {
        m_oldColor = m_backboard.material.color;
        m_light.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        Projectile ball = other.GetComponent<Projectile>();
        if(ball != null)
        {
            m_canShoot = true;
            StartCoroutine(WaitToShoot(ball));
        }
        else
        {
            Ball ball2 = other.GetComponent<Ball>();
            if(ball2 != null)
            {
                m_canShoot = true;
                StartCoroutine(WaitToShoot(ball2));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Projectile ball = other.GetComponent<Projectile>();
        if (ball != null)
        {
            m_canShoot = false;
        }
        else
        {
            Ball ball2 = other.GetComponent<Ball>();
            if (ball2 != null)
            {
                m_canShoot = false;
            }
        }
    }

    private IEnumerator WaitToShoot(Projectile ball)
    {
        yield return new WaitForSeconds(1);
        if(m_canShoot)
        {
            Jump();
            ball.transform.position = m_shootPosition.position;
            ball.LaunchProjectileTo(m_target);
            m_backboard.material.color = Color.red;
            m_light.SetActive(true);

        }
    }

    private IEnumerator WaitToShoot(Ball ball)
    {
        yield return new WaitForSeconds(1);
        if (m_canShoot)
        {
            Jump();
            ball.transform.position = m_shootPosition.position;
            ball.LaunchProjectileTo(m_target);
            m_backboard.material.color = Color.red;
            m_light.SetActive(true);

        }
    }

    public void Jump()
    {
        Vector3 hidePos = m_playerJump.transform.position;
        m_playerJump.transform.position = m_playerWait.transform.position;
        m_playerWait.transform.position = hidePos;
        StartCoroutine(EndJump());
    }

    private IEnumerator EndJump()
    {
        yield return new WaitForSeconds(1.5f);
        Vector3 courtPos = m_playerJump.transform.position;
        m_playerJump.transform.position = m_playerWait.transform.position;
        m_playerWait.transform.position = courtPos;
    }

    public void ResetBackboard()
    {
        m_backboard.material.color = m_oldColor;
        m_light.SetActive(false);
    }
}
