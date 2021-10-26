using System.Collections;
using System.Collections.Generic;
using Managers;
using MonoObjects.Agent;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class DrawController : MonoBehaviour
    {

        [SerializeField] private PlayerData _playerData;
        [SerializeField] private LineRenderer lineRenderer;
        public PlayerData PlayerData => _playerData;
        private Vector3[] _currentPoints;
        [SerializeField]private List<Vector3> _agentPathPoints;

        [Range(0f,2f)]
        [SerializeField] private float minimumDistance;

        [Range(2f,10f)]
        [SerializeField] private float maximumDistance;

        [Range(0f,25f)]
        [SerializeField] private float maximumAngle;

        private void Awake()
        {
            lineRenderer.alignment = LineAlignment.TransformZ;
        }

        public void OnDragStart(Vector3 getWorldPos)
        {
            lineRenderer.positionCount = 2;

            _agentPathPoints = new List<Vector3> {getWorldPos};
            
            getWorldPos = lineRenderer.transform.InverseTransformPoint(getWorldPos);
            
            lineRenderer.SetPosition(0,getWorldPos);
            lineRenderer.SetPosition(1,getWorldPos);
            
            if(_playerData.useInitialFireRate || _playerData.useAutoFire)
                StopCoroutine(nameof(SpawnRoutine));
        }


        public void OnDrag(Vector3 getWorldPos)
        {
            AddPointToLineRenderer(getWorldPos.AddY(0.01f));
            
            var distance = Vector3.Distance(getWorldPos, _agentPathPoints[_agentPathPoints.Count-1]);
            if(distance < minimumDistance || distance > maximumDistance)
            {
                return;
            }
            
            AddPoint(getWorldPos.AddY(0.01f));
        }

        private void AddPoint(Vector3 getWorldPos)
        {
            var secondPos = _agentPathPoints[_agentPathPoints.Count - 1];

            var halfPos = new Vector3((secondPos.x + getWorldPos.x) / 2f,
                getWorldPos.y,(secondPos.z + getWorldPos.z) / 2f
            );
            
            _agentPathPoints.Add(halfPos);
            _agentPathPoints.Add(getWorldPos);
        }

        private void AddPointToLineRenderer(Vector3 getWorldPos)
        {
            _currentPoints = new Vector3[lineRenderer.positionCount];

            lineRenderer.GetPositions(_currentPoints);
            
            lineRenderer.positionCount = _currentPoints.Length + 1;
            
            getWorldPos = lineRenderer.transform.InverseTransformPoint(getWorldPos);
            
            lineRenderer.SetPositions(_currentPoints.Add(getWorldPos));
        }

        public void OnRelease()
        {
            if(_playerData.useInitialFireRate || _playerData.useAutoFire)
                StartCoroutine(nameof(SpawnRoutine));
            else
                ObjectPoolManager.Instance.GetFromPool<PlayerAgent>().FollowPath(_agentPathPoints.ToArray(), Vector3.zero);

        }

        private IEnumerator SpawnRoutine()
        {
            if(_playerData.useAutoFire)
            {
                while (_playerData.useAutoFire)
                {
                    if(_playerData.useInitialFireRate)
                        yield return Wait.ForSeconds(_playerData.initialFireRate);
                    ObjectPoolManager.Instance.GetFromPool<PlayerAgent>()
                        .FollowPath(_agentPathPoints.ToArray(), Vector3.zero);
                    yield return Wait.ForSeconds(_playerData.reloadTime);
                }
            }
            else
            {
                yield return Wait.ForSeconds(_playerData.initialFireRate);
                ObjectPoolManager.Instance.GetFromPool<PlayerAgent>()
                    .FollowPath(_agentPathPoints.ToArray(), Vector3.zero);
            }

            
        }
        
        public void StopDrawing()
        {
            OnRelease();
        }

        public void ClearDrawing()
        {
            _agentPathPoints = new List<Vector3>();
        }
    }
}