using System.Collections;
using Core;
using Managers;
using UnityEngine;
using Utils;

namespace MonoObjects.Particle
{
    public class PlayerBaseDestructionParticle : ParticleEffect
    {
        private IEnumerator Start()
        {
            yield return Wait.ForSeconds(seconds: _particleSystem.main.duration);
            ReturnToPool();
        }

        public override MonoPooled Init()
        {
            gameObject.SetActive(false);
            return this;
        }

        public override void ReturnToPool()
        {
            ObjectPoolManager.Instance.ReturnToPool(this);
        }

        public void Activate(Vector3 at)
        {
            gameObject.SetActive(true);
            transform.position = at;
            _particleSystem.Play();
        }
    }
}