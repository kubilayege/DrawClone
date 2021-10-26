using Controllers;
using Core;
using Managers;
using MonoObjects.Objective;
using UnityEngine;
using UnityEngine.AI;

namespace MonoObjects.Core
{
    public class Level : MonoBehaviour 
    {
        [SerializeField] private ParticleSystem[] _confetti;

        [SerializeField] private NavMeshSurface _navMeshSurface;
        [SerializeField] private LevelSegment[] levelSegments;


        private int _currentSegmentIndex;

        public void DestroyLevel()
        {
            Destroy(gameObject);
        }

        public void Initialize(int levelNumber)
        {
            PlayerCameraController.Instance.SetPosition(levelSegments[_currentSegmentIndex].transform);
            PlayerController.Instance.Init(levelSegments[_currentSegmentIndex].PlayerIdlePosition, levelSegments[_currentSegmentIndex].PlayerIdleForward);
        }

        public void StartLevel()
        {
            levelSegments[_currentSegmentIndex].ActivateSegment();
        }

        public void OnSegmentFinish()
        {
            foreach (var confetti in _confetti)
            {
                confetti.Play();
            }
            
            levelSegments[_currentSegmentIndex].StopSegment();
            
            
            if (_currentSegmentIndex == levelSegments.Length - 1)
            {
                ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelCompleted);
            }
            else
            {
                
                _currentSegmentIndex++;
                levelSegments[_currentSegmentIndex].ActivateSegment();
                PlayerCameraController.Instance.Move(levelSegments[_currentSegmentIndex].transform);
                PlayerController.Instance.Init(levelSegments[_currentSegmentIndex].PlayerIdlePosition, levelSegments[_currentSegmentIndex].PlayerIdleForward);
            }
        }

        public void OnLevelFail()
        {
            levelSegments[_currentSegmentIndex].StopSegment();
        }

        public Base GetClosestEnemyBase(Vector3 from)
        {
            return levelSegments[_currentSegmentIndex].GetClosestEnemyBasePosition(from);
        }

        public Base GetClosestPlayerBase(Vector3 from)
        {
            return levelSegments[_currentSegmentIndex].GetClosestPlayerBasePosition(from);
        }
        public Vector3 GetNextLevelPosition()
        {
            return levelSegments[_currentSegmentIndex].transform.position;
        }
        
        public bool IsPointInNavMesh(Vector3 point)
        {
            return _navMeshSurface.navMeshData.sourceBounds.Contains(point);
        }
    }
}