using Managers;
using MonoObjects.Agent;
using MonoObjects.Particle;
using UnityEngine;

namespace MonoObjects.Interactable
{
    public class Obstacle : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerAgent agent)) return;
            
            ObjectPoolManager.Instance.GetFromPool<PlayerDeathParticle>().Activate(agent.transform.position);
            agent.ReturnToPool();
        }
    }
}