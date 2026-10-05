using UnityEngine;
using UnityEngine.InputSystem;

public class IntroBrickParticles : MonoBehaviour
{
    public ParticleSystem BrickParticles;

    private bool _particlesHavePlayed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!_particlesHavePlayed)
        {
            BrickParticles.Play();
            _particlesHavePlayed = true;
        }
    }
}
