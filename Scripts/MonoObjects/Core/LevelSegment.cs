using System.Collections.Generic;
using Core;
using Managers;
using MonoObjects.Agent;
using MonoObjects.Interactable;
using MonoObjects.Objective;
using UnityEngine;

namespace MonoObjects.Core
{
    public class LevelSegment : MonoBehaviour
    {
        [SerializeField] private Transform enemyBaseParent;
        [SerializeField] private Transform playerBaseParent;
        [SerializeField] private Transform enemySpawnerParent;
        [SerializeField] private Transform gateParent;
        [SerializeField] private Collider drawStartZoneCollider;
        
        [Header("Segment Settings")]
        [SerializeField] private int activateGateAtOnce;

        public delegate bool BaseDestructionDelegate(Base destructedBase); 

        private List<Base> _playerBaseTargets;
        private List<Base> _levelEnemyTargets;
        private List<EnemySpawner> _enemySpawners;
        private Queue<Gate> _gates;
        public Vector3 PlayerIdlePosition => _playerIdlePosition;
        public Vector3 PlayerIdleForward => _playerIdleForward;

        private Vector3 _playerIdlePosition;
        private Vector3 _playerIdleForward;
        private void Awake()
        {
            _levelEnemyTargets = new List<Base>();
            _playerBaseTargets = new List<Base>();
            _enemySpawners = new List<EnemySpawner>();
            _gates = new Queue<Gate>();
            
            enemyBaseParent.GetComponentsInChildren(_levelEnemyTargets);
            playerBaseParent.GetComponentsInChildren(_playerBaseTargets);
            enemySpawnerParent.GetComponentsInChildren(_enemySpawners);
            foreach (var componentsInChild in gateParent.GetComponentsInChildren<Gate>())
            {
                _gates.Enqueue(componentsInChild);
            }

            var playerBaseTransform = _playerBaseTargets[0].transform;
            _playerIdleForward = playerBaseTransform.forward;
            _playerIdlePosition = playerBaseTransform.position + _playerIdleForward * 3.5f +
                                  playerBaseTransform.right * 5f + playerBaseTransform.up * 1.31f;
        }

        public void ActivateSegment()
        {
            drawStartZoneCollider.enabled = true;
            foreach (var enemySpawner in _enemySpawners)
            {
                enemySpawner.StartSpawning();
            }

            for (int i = 0; i < activateGateAtOnce; i++)
            {
                _gates.Dequeue().Activate(this);
            }

            foreach (var levelEnemyTarget in _levelEnemyTargets)
            {
                levelEnemyTarget.Init(OnBaseDestruct);
            }
            
            foreach (var playerBaseTarget in _playerBaseTargets)
            {
                playerBaseTarget.Init(OnBaseDestruct);
            }
        }

        public Base GetClosestEnemyBasePosition(Vector3 from)
        {
            var distance = float.MaxValue;
            var currentTargetBase = _levelEnemyTargets[0];
            foreach (var targetBase in _levelEnemyTargets)
            {
                var magnitude = (targetBase.transform.position - @from).magnitude;
                if (magnitude < distance)
                {
                    currentTargetBase = targetBase;
                    distance = magnitude;
                }
            }

            return currentTargetBase;
        }
        
        public Base GetClosestPlayerBasePosition(Vector3 from)
        {
            var distance = float.MaxValue;
            var currentTargetBase = _playerBaseTargets[0];
            foreach (var targetBase in _playerBaseTargets)
            {
                var magnitude = (targetBase.transform.position - @from).magnitude;
                if (magnitude < distance)
                {
                    currentTargetBase = targetBase;
                    distance = magnitude;
                }
            }

            return currentTargetBase;
        }

        public void StopSegment()
        {
            drawStartZoneCollider.enabled = false;

            foreach (var enemySpawner in _enemySpawners)
            {
                enemySpawner.StopSpawning();
            }
            
            foreach (var gate in _gates)
            {
                gate.DeActivate();
            }

            foreach (var target in _levelEnemyTargets)
            {
                target.gameObject.SetActive(false);
            }
        }

        public void GateDeActive(Gate gate)
        {
            _gates.Enqueue(gate);
            _gates.Dequeue().Activate(this);
        }


        private bool OnBaseDestruct(Base baseDestructed)
        {
            if (_levelEnemyTargets.Contains(baseDestructed) )
            {
                if(_levelEnemyTargets.Count == 1)
                {
                    Debug.Log("AgentBaseDone");

                    ActionManager.Instance.TriggerAction(ActionIDHolder.OnSegmentCompleteID);
                    return false;
                }
                else
                {
                    _levelEnemyTargets.Remove(baseDestructed);
                }
            }
            
            
            if (_playerBaseTargets.Contains(baseDestructed))
            {
                if(_playerBaseTargets.Count == 1)
                {
                    Debug.Log("PlayerBaseDone");
                    ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelFailedID);
                    return false;
                }
                else
                {
                    _playerBaseTargets.Remove(baseDestructed);
                }
            }
            
            return true;
        }
        
    }
}