using System.Collections.Generic;
using DG.Tweening;
using Managers;
using MonoObjects.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonoObjects.Interactable
{
    
    public enum GateType
    {
        Additive = 0,
        Multiplier = 1,
        Decrement = 2
    }
    public abstract class Gate : MonoBehaviour
    {        
        private LevelSegment _levelSegment;
        [SerializeField] protected GameObject _gateParent;
        [SerializeField] protected TextMeshProUGUI infoText;
        [SerializeField] protected Transform canvas;
        [SerializeField] protected Transform _modelTransform;
        [SerializeField] protected CapsuleCollider _collider;
        [SerializeField] private Material _decrementalMaterial;
        [SerializeField] private Material _incrementerMaterial;
        [SerializeField] private MeshRenderer modelMesh;
        [SerializeField] private Image barImage;
        
        [Space(15)]
        [SerializeField] protected GateType gateType;
        
        [Header("Gate Style")]
        [SerializeField] private float textSize;
        [SerializeField] private float radius;
        [SerializeField] private float height;
        [SerializeField] private Color barColor;

        
        // [Header("Gate Lifetime Settings ")]
        // [SerializeField] protected float initializeTime;
        // [SerializeField] protected float lifeTime;
        // [SerializeField] protected float reSpawnIntervalTime;
        

        [Header("Gate Movement Settings")] 
        [SerializeField] private Transform[] destinations;
        [SerializeField] protected bool isMoving;
        [SerializeField] protected bool startAtFirstDestination;
        [SerializeField] protected bool doLoop;
        [SerializeField] protected float moveSpeed;
        private Vector3 _defaultPosition;
        private List<Vector3> _path;
        private float _moveDistance;
        

        [Header("Gate Animation Settings")] 
        [SerializeField] private Ease _ease;
        [SerializeField] private float duration;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float elasticity = 1f;
        [SerializeField] private Vector3 punch;
        private Vector3 _defaultTextScale;


        [Header("Agent Spawn Settings")]
        [SerializeField] protected int maxUseCount = 0;
        private int _currentUseCount;
        [SerializeField] protected int amount = 0;
        [SerializeField] protected float spawnDuration;
        [SerializeField] protected Ease spawnEase;
        [SerializeField] protected float minDistance;
        [SerializeField] protected float maxDistance;




        private void Awake()
        {
            CalculatePath();
            
            _defaultTextScale = infoText.transform.localScale;

            // if (initializeTime == 0) return;

            if (maxUseCount == 0)
            {
                barImage.gameObject.SetActive(false);
            }
            
            _gateParent.SetActive(false);
            _collider.enabled = false;
        }

        public virtual void OnValidate()
        {
            ApplyChanges();
            if(modelMesh != null)
                modelMesh.material = gateType == GateType.Decrement ? _decrementalMaterial : _incrementerMaterial;
        }
        

        private void StartPath()
        {
            if (!isMoving) return;


            transform.position = startAtFirstDestination ? destinations[0].position : _defaultPosition;
            if (doLoop)
            {
                _moveDistance += (_path[_path.Count-1] - transform.position).magnitude;
                _path.Add(transform.position);
            }
            Debug.Log($"Path {infoText.text}");
            transform.DOPath(_path.ToArray(),
                    _moveDistance / moveSpeed, pathType:PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1, doLoop? LoopType.Restart : LoopType.Yoyo)
                .SetId(transform.GetInstanceID());
        }

        private void CalculatePath()
        {
            _defaultPosition = transform.position;
            var startDestination = destinations[0].position;
            _path = new List<Vector3>();
            _moveDistance = 0f;
            foreach (var destination in destinations)
            {
                _moveDistance += (destination.position - startDestination).magnitude;
                startDestination = destination.position;
                _path.Add(startDestination);
            }
            
            
            transform.position = startAtFirstDestination ? destinations[0].position : _defaultPosition;
        }

        // private IEnumerator LifeTimeRoutine()
        // {
        //     yield return Wait.ForSeconds(initializeTime);
        //     
        //     while(true)
        //     {
        //         _gateParent.SetActive(true);
        //         _collider.enabled = true;
        //         StartPath();
        //         
        //         yield return Wait.ForSeconds(lifeTime);
        //         
        //         DOTween.Kill(transform.GetInstanceID());
        //         _gateParent.SetActive(false);
        //         _collider.enabled = false;
        //         
        //         yield return Wait.ForSeconds(reSpawnIntervalTime);
        //     }
        // }
        //
        
        private void ApplyChanges()
        {
            _modelTransform.localScale = new Vector3(radius, height, radius);
            _modelTransform.localPosition = Vector3.up * height / 2f;
            _collider.center = _modelTransform.localPosition;
            _collider.radius = radius/2f;
            _collider.height = height*3;

            canvas.localPosition = _modelTransform.localPosition + _modelTransform.up * (height + 0.01f);
            infoText.fontSize = textSize;

            if(maxUseCount != 0)
                barImage.fillAmount = 1f - ((float)_currentUseCount / maxUseCount);
            barImage.transform.localScale = Vector3.one * radius;
            barImage.color = barColor;
        }

        public virtual void Process(Agent.Agent playerAgent)
        {
            DOTween.Kill(gameObject.GetInstanceID());
            infoText.transform.DOPunchScale(punch, duration, vibrato, elasticity).SetEase(_ease)
                .SetId(gameObject.GetInstanceID()).OnKill(() =>
                {
                    infoText.transform.localScale = _defaultTextScale;
                }).OnComplete(() =>
                {
                    infoText.transform.localScale = _defaultTextScale;
                });

            _currentUseCount++;
            
            UIManager.Instance.AddScore(amount);

            if (maxUseCount == 0)
            {
                return;
            }
            
            barImage.fillAmount = 1f - ((float)_currentUseCount / maxUseCount);
            
            if (_currentUseCount != maxUseCount) return;
            
            DeActivate();
            _levelSegment.GateDeActive(this);
        }

        public void Activate(LevelSegment levelSegment)
        {
            _gateParent.SetActive(true);
            _collider.enabled = true;
            _currentUseCount = 0;
            _levelSegment = levelSegment;
            
            ApplyChanges();
            StartPath();
            // StartCoroutine(nameof(LifeTimeRoutine));
        }

        public void DeActivate()
        { 
            _gateParent.SetActive(false);
            _collider.enabled = false;
            // StopCoroutine(nameof(LifeTimeRoutine));
        }
    }
}