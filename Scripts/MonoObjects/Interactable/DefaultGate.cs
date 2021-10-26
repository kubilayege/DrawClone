using DG.Tweening;
using Managers;
using MonoObjects.Agent;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MonoObjects.Interactable
{
    public class DefaultGate : Gate
    {
        public override void OnValidate()
        {
            base.OnValidate();
            if (infoText != null)
            {
                infoText.text = gateType == GateType.Decrement
                    ? $"-{amount}"
                    : $"{(gateType == GateType.Additive ? "+" : "x")}{amount}";
            }
        }
        
        public override void Process(Agent.Agent playerAgent)
        {
            if (playerAgent.ContainsGate(this))
            {
                return;
            }
            
            base.Process(playerAgent);
            
            if (gateType == GateType.Decrement)
            {
                ProcessDecrement(playerAgent);
                return;
            }

            var agentCount = amount - (int)gateType;

            var offset = Vector3.zero;
            for (int i = 0; i < agentCount; i++)
            {
                var cloneAgent = ObjectPoolManager.Instance.GetFromPool<PlayerAgent>();
                
                // offset.x += Random.Range(-1, 2) * Random.Range(minDistance, maxDistance);
                // offset.z += Random.Range(-1, 2) * Random.Range(minDistance, maxDistance);

                offset = GetRandomOffset();

                if (NavMesh.SamplePosition(playerAgent.transform.position + offset, out NavMeshHit hit,
                    offset.magnitude, 1))
                {
                    
                    cloneAgent.transform.position = hit.position;
                    cloneAgent.transform.localScale = Vector3.zero;
                }
                
                // while (!LevelManager.Instance.CurrentLevel.IsPointInNavMesh(playerAgent.transform.position + offset))
                // {
                //     offset = GetRandomOffset();
                // }
                
                
                
                cloneAgent.FollowPath(playerAgent.GetPath(), offset);
                cloneAgent.SetGates(playerAgent.GetGates());
                
                DOTween.Kill(cloneAgent.GetInstanceID());
                cloneAgent.transform.DOScale(Vector3.one, spawnDuration).SetEase(spawnEase).SetId(cloneAgent.GetInstanceID());
            }
        }

        private Vector3 GetRandomOffset()
        {
            Vector3 offset;
            offset = new Vector3(Random.Range(-minDistance, maxDistance), 0,
                Random.Range(-minDistance, maxDistance));

            offset = Vector3.ClampMagnitude(offset, maxDistance);
            if (offset.magnitude < minDistance)
            {
                offset = offset.normalized * minDistance;
            }

            return offset;
        }

        private void ProcessDecrement(Agent.Agent playerAgent)
        {
            if (amount == 1)
            {
                _collider.enabled = false;
                transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    
                });
                
                infoText.text = "0";
                return;
            }
            amount--;
            
            infoText.text = $"-{amount}";
            
            playerAgent.ReturnToPool();
        }
    }
}