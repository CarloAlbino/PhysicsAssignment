using UnityEngine;
using System.Collections;

public class ShootBall : MonoBehaviour {

    public Transform m_target;

    private bool m_canShoot = false;

    public GameObject m_playerWait, m_playerJump;
    public Transform m_shootPosition;

    void OnTriggerEnter(Collider other)
    {
        Projectile ball = other.GetComponent<Projectile>();
        if(ball != null)
        {
            m_canShoot = true;
            StartCoroutine(WaitToShoot(ball));
        }
    }

    void OnTriggerExit(Collider other)
    {
        Projectile ball = other.GetComponent<Projectile>();
        if (ball != null)
        {
            m_canShoot = false;
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
}
