using System.Collections.Generic;
using Core;
using Managers;
using MonoObjects.Interactable;
using UnityEngine;
using UnityEngine.AI;

namespace MonoObjects.Agent
{
    public abstract class Agent : MonoPooled
    {
        [SerializeField] public NavMeshAgent navMeshAgent;
        [SerializeField] protected float maxStopTime;
        [SerializeField] protected Animator animator;
        
        
        [SerializeField] protected Rigidbody rigidbody;

        protected float timeSinceLastDestination;
        protected List<Gate> _gates;
        [SerializeField]protected List<Vector3> _path;
        [SerializeField] protected bool isActive;
        
        
        protected static readonly int Running = Animator.StringToHash("Running");
        protected static readonly int Idle = Animator.StringToHash("DynIdle");
        protected static readonly int Cheer = Animator.StringToHash("WaveDance");
        
        public override MonoPooled Init()
        {
            gameObject.SetActive(false);
            _gates = new List<Gate>();
            _path = new List<Vector3>();
            isActive = true;
            return this;
        }

        public override void ReturnToPool()
        {
            isActive = true;
            gameObject.SetActive(false);
            _gates = new List<Gate>();
            _path = new List<Vector3>();
            ObjectPoolManager.Instance.ReturnToPool(this);
        }
        
        public bool ContainsGate(Gate gate)
        {
            if (_gates.Contains(gate)) return true;
            
            _gates.Add(gate);
            return false;
        }

        public Vector3[] GetPath()
        {
            return _path.ToArray();
        }

        public void SetGates(Gate[] gates)
        {
            _gates.AddRange(gates);
        }
        
        public Gate[] GetGates()
        {
            return _gates.ToArray();
        }
        
        protected bool AgentReachedDestination(Vector3 destination)
        {
            timeSinceLastDestination += Time.fixedDeltaTime;
            
            if (timeSinceLastDestination >= maxStopTime)
                return true;
            
            return !(Vector3.Distance(transform.position, destination) > navMeshAgent.stoppingDistance);
        }
    }
}