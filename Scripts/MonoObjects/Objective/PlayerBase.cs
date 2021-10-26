using Core;
using Managers;
using MonoObjects.Agent;
using MonoObjects.Particle;
using UnityEngine;

namespace MonoObjects.Objective
{
    public class PlayerBase : Base
    {

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out EnemyAgent agent)) return;
            
            ObjectPoolManager.Instance.GetFromPool<PlayerBaseDestructionParticle>().Activate(transform.position);
            
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