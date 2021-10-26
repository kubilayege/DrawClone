using UnityEngine;

namespace Core
{
    public abstract class MonoPooled : MonoBehaviour
    {
        public abstract MonoPooled Init();
        
        public abstract void ReturnToPool();
    }
}