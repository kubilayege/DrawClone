using System.Collections;
using System.Collections.Generic;
using Core;
using Managers;
using MonoObjects.Interactable;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace MonoObjects.Agent
{
    public class PlayerAgent : Agent
    {
        [SerializeField] private bool useNavMesh;
        
        private void Start()
        {
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelFailedID,StopFail);
            ActionManager.Instance.AddAction(ActionIDHolder.OnSegmentCompleteID,Stop);
        }

        private void Stop()
        {
            animator.SetTrigger(Cheer);
            isActive = false;
            StopCoroutine(nameof(Move));
            navMeshAgent.enabled = false;
        }
        
        private void StopFail()
        {
            animator.SetTrigger(Idle);
            isActive = false;
            StopCoroutine(nameof(Move));
            navMeshAgent.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;
            if (!other.TryGetComponent(out Gate gate)) return;
            if (_gates.Contains(gate)) return;
                
            gate.Process(this);
        }

        public void FollowPath(Vector3[] path, Vector3 offset)
        {
            isActive = true;
            if(offset != Vector3.zero)
            {
                var tempPath = new List<Vector3>();
                tempPath.Add(transform.position);
                foreach (var point in path)
                {
                    if (NavMesh.SamplePosition(point + offset, out NavMeshHit hit,
                        offset.magnitude, 1))
                    {
                        tempPath.Add(hit.position);
                    }
                }

                path = tempPath.ToArray();
            }
            
            gameObject.SetActive(true);
            
            animator.SetTrigger(Running);
            _path.AddRange(path); 
            
            StartCoroutine(nameof(Move), path);
        }

        private IEnumerator Move(Vector3[] path)
        {
            if(path.Length != 0)
            {
                transform.position = path[0];
                _path.Remove(path[0]);
            }
            
            yield return Wait.FixedUpdate;
            yield return Wait.ForSeconds(0.1f);
            
            if(useNavMesh)
                navMeshAgent.enabled = true;
            

            var at = 1;
            while (at < path.Length)
            {
                _path.Remove(path[at]);
                
                yield return Wait.FixedUpdate;
                
                if(useNavMesh)
                    navMeshAgent.destination = path[at];
                
                
                while (!AgentReachedDestination(path[at]))
                {
                    if(!useNavMesh)
                    {
                        var forwardVector = (path[at] - transform.position);
                        var nextPosition = transform.position +
                                           forwardVector.normalized * Time.fixedDeltaTime * navMeshAgent.speed;

                        transform.forward = forwardVector;
                        rigidbody.MovePosition(nextPosition);
                    }
                    yield return Wait.FixedUpdate;
                }

                at++;
                timeSinceLastDestination = 0f;
            }

            if(!useNavMesh)
            {
                while (Vector3.Distance(
                    LevelManager.Instance.CurrentLevel.GetClosestEnemyBase(transform.position).transform.position,
                    transform.position) > 1f)
                {
                    var forwardVector = (LevelManager.Instance.CurrentLevel.GetClosestEnemyBase(transform.position)
                        .transform
                        .position - transform.position);
                    var nextPosition = transform.position +
                                       forwardVector.normalized * Time.fixedDeltaTime * navMeshAgent.speed;

                    transform.forward = forwardVector;
                    rigidbody.MovePosition(nextPosition);
                    yield return Wait.FixedUpdate;
                }
            }
            
            PathToBase();
        }

        private void PathToBase()
        {
            if (!gameObject.activeInHierarchy)
                return;
            
            var currentTargetBase = LevelManager.Instance.CurrentLevel.GetClosestEnemyBase(transform.position);
            currentTargetBase.OnDestruction(PathToBase);
            navMeshAgent.destination = currentTargetBase.transform.position;
        }
    }
}