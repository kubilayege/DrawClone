using Core;
using DG.Tweening;
using UnityEngine;

namespace Controllers
{
    public class PlayerCameraController : MonoSingleton<PlayerCameraController>
    {
        [SerializeField] private float moveDuration;
        [SerializeField] private float rotationDuration;
        [Range(0f,1f)]
        [SerializeField] private float rotationThreshold;
        
        
        public void Move(Transform nextSegment)
        {
            DOTween.Kill(transform.GetInstanceID());
            transform.DOMove(nextSegment.position, moveDuration).SetId(transform.GetInstanceID());
            transform.DORotate(nextSegment.rotation.eulerAngles, rotationDuration)
                .SetDelay(moveDuration * rotationThreshold)
                .SetId(transform.GetInstanceID());
        }
        
        public void SetPosition(Transform nextSegment)
        {
            transform.position = nextSegment.position;
            transform.rotation = nextSegment.rotation;
        }
    }
}