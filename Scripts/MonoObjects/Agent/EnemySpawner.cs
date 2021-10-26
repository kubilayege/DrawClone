using System.Collections;
using Managers;
using UnityEngine;
using Utils;

namespace MonoObjects.Agent
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        
        [Header("Spawn Settings")] 
        [SerializeField] private float minInterval;
        [SerializeField] private float maxInterval;
        
        [SerializeField] private int minRate;
        [SerializeField] private int maxRate;
        [SerializeField] private int maxSpawnCount;
        [SerializeField] private float spawnMargin = 1f;
        
        

        public void StartSpawning()
        {
            StartCoroutine(nameof(SpawnRoutine), maxSpawnCount == 0);
        }

        private IEnumerator SpawnRoutine(bool infiniteSpawn)
        {
            while (infiniteSpawn || maxSpawnCount > 0)
            {
                yield return Wait.ForSeconds(Random.Range(minInterval, maxInterval));
                for (int i = 0; i < Mathf.Min(minRate++, maxRate); i++)
                {
                    var offsetPosition = transform.position + new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f)) * spawnMargin; 
                    // var destination = FindTargetPoint(offsetPosition);
                    var destination = LevelManager.Instance.CurrentLevel.GetClosestPlayerBase(offsetPosition);
                    var enemyAgent = ObjectPoolManager.Instance.GetFromPool<EnemyAgent>();
                    
                    enemyAgent.AssignPath(offsetPosition, destination);
                }
            }
        }

        private Vector3 FindTargetPoint(Vector3 origin)
        {
            var ray = new Ray(origin, transform.forward);
            
            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, targetLayer)) 
                return Vector3.positiveInfinity;
            
            
            return hit.point;

        }

        public void StopSpawning()
        {
            StopCoroutine(nameof(SpawnRoutine));
        }
    }
}