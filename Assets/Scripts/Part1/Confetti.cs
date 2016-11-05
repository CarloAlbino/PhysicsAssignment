using UnityEngine;
using System.Collections;

public class Confetti : MonoBehaviour {

    private ParticleSystem m_particles;

	// Use this for initialization
	void Start () {
        m_particles = GetComponentInChildren<ParticleSystem>();
	}
    
    void OnTriggerEnter()
    {
        m_particles.Play();
    }
}
