using Managers;
using MonoObjects.Agent;
using MonoObjects.Particle;
using UnityEngine;

namespace MonoObjects.Objective
{
    public class EnemyBase : Base
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerAgent agent)) return;
            
            ObjectPoolManager.Instance.GetFromPool<EnemyBaseDestructionParticle>().Activate(transform.position);

            agent.ReturnToPool();
            if (!Damage()) return;

            if(BaseDestructionDelegate(this))
            {
                foreach (var baseDestructionDelegate in OnBaseDestructionDelegates)
                {
                    baseDestructionDelegate();
                }
            }

            gameObject.SetActive(false);
        }
    }
}