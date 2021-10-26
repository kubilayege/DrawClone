using System;
using System.Collections.Generic;
using System.Numerics;
using Core;
using DG.Tweening;
using Managers;
using MonoObjects.Agent;
using MonoObjects.Particle;
using ScriptableObjects;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Controllers
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float segmentMoveDuration;
        [SerializeField] private float initialMoveDuration;
        [SerializeField] private float backToIdleDuration;
        [SerializeField] private float killDuration;
        [SerializeField] private Vector3 drawAngle;
        [SerializeField] private Vector3 punchAngle;


        private bool _killing;
        private int _instanceID;
        private static readonly int Point = Animator.StringToHash("Point");
        private static readonly int Idle = Animator.StringToHash("Natural");

        private Vector3 _idlePosition;
        private Vector3 _idleRotation;
        private static readonly int Fist = Animator.StringToHash("Fist");

        private void Awake()
        {
            _instanceID = gameObject.GetInstanceID();
        }

        public void StartDrawing(Vector3 position)
        {
            DOTween.Kill(_instanceID);
            animator.SetTrigger(Point);
            MoveTo(position, initialMoveDuration);
            transform.DORotate(drawAngle + _idleRotation, initialMoveDuration).SetId(_instanceID);
        }

        public void DrawNext(Vector3 position)
        {
            transform.position = position;
        }
        
        public void StopDrawing()
        {
            if (_killing)
                return;
            DOTween.Kill(_instanceID);
            animator.SetTrigger(Idle);
            MoveTo(_idlePosition, backToIdleDuration);
            transform.DORotate(_idleRotation, backToIdleDuration).SetId(_instanceID);
        }

        public void Init(Vector3 idlePosition, Vector3 playerIdleForward)
        {
            animator.SetTrigger(Idle);
            MoveTo(idlePosition, segmentMoveDuration);
            transform.DORotate(Quaternion.LookRotation(playerIdleForward).eulerAngles, segmentMoveDuration).SetId(_instanceID);
            _idlePosition = idlePosition;
            _idleRotation = Quaternion.LookRotation(playerIdleForward).eulerAngles;
        }

        private void MoveTo(Vector3 idlePosition, float duration)
        {
            DOTween.Kill(_instanceID);
            transform.DOMove(idlePosition, duration).SetId(_instanceID);
        }

        public void Kill(List<EnemyAgent> agents, Vector3 worldPos)
        {
            if (agents.Count == 0)
                return;
            DOTween.Kill(_instanceID);
            _killing = true;
            
            animator.SetTrigger(agents.Count >= 2 ? Fist : Point);

            transform.DOMove(agents.Count >= 2 ? worldPos + Vector3.up *3f : agents[0].transform.position, killDuration).SetId(_instanceID).OnComplete(() =>
            {                
                _killing = false;

                foreach(var agent in agents)
                {
                    Debug.Log(agent.name);
                    ObjectPoolManager.Instance.GetFromPool<EnemyDeathParticle>().Activate(agent.transform.position);
                    agent.ReturnToPool();
                }
                
                animator.SetTrigger(Idle);
                MoveTo(_idlePosition, backToIdleDuration);
                transform.DORotate(_idleRotation, backToIdleDuration).SetId(_instanceID);
            });
            transform.DORotate((agents.Count == 1 ? drawAngle : punchAngle  ) + _idleRotation , killDuration).SetId(_instanceID);

        }
    }
}
