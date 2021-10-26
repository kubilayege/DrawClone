using Core;
using UnityEngine;

namespace MonoObjects.Particle
{
    public abstract class ParticleEffect : MonoPooled
    {
        [SerializeField] protected ParticleSystem _particleSystem;
        
    }
}