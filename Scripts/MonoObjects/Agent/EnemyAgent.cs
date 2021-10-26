using Core;
using Managers;
using MonoObjects.Objective;
using MonoObjects.Particle;
using UnityEngine;

namespace MonoObjects.Agent
{
    public class EnemyAgent : Agent
    {

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerAgent agent))
            {
                ObjectPoolManager.Instance.GetFromPool<EnemyDeathParticle>().Activate(transform.position);
                ObjectPoolManager.Instance.GetFromPool<PlayerDeathParticle>().Activate(agent.transform.position);
                
                agent.ReturnToPool();
                ReturnToPool();
            }
        }

        private void Start()
        {
            ActionManager.Instance.AddAction(ActionIDHolder.OnSegmentCompleteID,Stop);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelFailedID,StopFail);
        }

        private void StopFail()
        {
            
            animator.SetTrigger(Idle);
            navMeshAgent.enabled = false;
        }  
        
        private void Stop()
        {
            animator.SetTrigger(Cheer);
            navMeshAgent.enabled = false;
        }   
        
        public void AssignPath(Vector3 startPos, Base targetBase)
        {
            gameObject.SetActive(true);
            transform.position = startPos;
            navMeshAgent.enabled = true;
            
            animator.SetTrigger(Running);

            targetBase.OnDestruction(PathToBase);
            navMeshAgent.destination = targetBase.transform.position;
            navMeshAgent.isStopped = false;
        }

        private void PathToBase()
        {
            if (!gameObject.activeInHierarchy)
                return;
            
            var targetBase = LevelManager.Instance.CurrentLevel.GetClosestPlayerBase(transform.position);
            if (targetBase == null)
                return;
            targetBase.OnDestruction(PathToBase);
            
            navMeshAgent.destination = targetBase.transform.position;
        }
    }
}