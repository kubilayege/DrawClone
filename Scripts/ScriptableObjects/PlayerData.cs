using UnityEngine;

namespace ScriptableObjects
{
    
    [CreateAssetMenu(menuName = "Player/Data")]
    public class PlayerData : ScriptableObject
    {
        public bool useInitialFireRate;
        public bool useAutoFire;
        [Space(10)]
        public float reloadTime;
        public float initialFireRate;
    }
}