using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Managers
{
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        [SerializeField] private MonoPooledData[] pooledObjects;
        
        private Dictionary<Type, Queue<MonoPooled>> _pools;


        private List<MonoPooled> _instances;

        private void Start()
        {
            _instances = new List<MonoPooled>();
            ActionManager.Instance.AddAction(ActionIDHolder.PrepareLevelID, ReturnInstances);
            ActionManager.Instance.AddAction(ActionIDHolder.OnSegmentCompleteID, ReturnInstances);
        }

        private void ReturnInstances()
        {
            var instancesCount = _instances.Count;
            for (int i = 0; i < instancesCount; i++)
            {
                _instances[0].ReturnToPool();
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();
            
            if (pooledObjects == null)
                return;
            
            foreach (var pooledObject in pooledObjects)
            {
                if(pooledObject.prefab != null)
                {
                    pooledObject.SetName(pooledObject.prefab.name);
                }
                else
                {
                    pooledObject.SetName("NaN");
                }
            }    
        }
        
        private void Awake()
        {
            _pools = new Dictionary<Type, Queue<MonoPooled>>();

            foreach (var pooledObject in pooledObjects)
            {
                Pool(pooledObject);
            }
        }

        private void Pool(MonoPooledData pooledObject)
        {
            if (pooledObject.count == 0)
            {
                return;
            }

            var type = pooledObject.prefab.GetType();
            GameObject typesParent = new GameObject($"{type.Name} Parent");
            
            typesParent.transform.parent = transform; 
            if (!_pools.ContainsKey(type))
            {
                _pools.Add(type, new Queue<MonoPooled>());
            }

            for (int i = 0; i < pooledObject.count; i++)
            {
                _pools[type].Enqueue(Instantiate(pooledObject.prefab, typesParent.transform).Init());
            }
        }
        
        public void ReturnToPool(MonoPooled pooledObject)
        {
            _pools[pooledObject.GetType()].Enqueue(pooledObject);
            _instances.Remove(pooledObject);
        }

        public T GetFromPool<T>() where T : MonoPooled
        {
            var monoPooled = _pools[typeof(T)].Dequeue();
            _instances.Add(monoPooled);
            return (T) monoPooled;
        }
    }

    [Serializable]
    public class MonoPooledData
    {
        [HideInInspector]
        public string _name = "";
        public MonoPooled prefab;
        public int count;

        public void SetName(string name)
        {
            _name = name;
        }
    }
}