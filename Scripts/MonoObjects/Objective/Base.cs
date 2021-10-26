using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using MonoObjects.Core;
using TMPro;
using UnityEngine;
using Utils;

namespace MonoObjects.Objective
{
    public class Base : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI infoText;
        [SerializeField] private Transform modelParent;
        
        [SerializeField] private Transform canvasParent;
        [SerializeField] private Transform canvasTargetParent;
        [SerializeField] private BoxCollider boxCollider;

        [Header("Base Damage Animation")]
        [SerializeField] private Vector3 punchVector;
        [SerializeField] private float punchDuration;
        [Range(0,10)]
        [SerializeField] private int vibrato;
        [Range(0f,1f)]
        [SerializeField] private float elasticity;

        [Header("Base Settings")]
        [SerializeField] protected int amount;
        [SerializeField] private float textSize;
        [SerializeField] private float width;
        [SerializeField] private float height;

        protected LevelSegment.BaseDestructionDelegate BaseDestructionDelegate;
        
        public delegate void OnBaseDestructionDelegate();

        protected List<OnBaseDestructionDelegate> OnBaseDestructionDelegates;
        
        private void OnValidate()
        {
            if (boxCollider == null || modelParent == null || canvasParent == null || canvasTargetParent == null ||
                infoText == null) return;
           
            infoText.text = amount.ToString();
            infoText.fontSize = textSize;
            var modelParentLocalScale = new Vector3(width, height, width);
            modelParent.localScale = modelParentLocalScale;
            boxCollider.center = Vector3.zero.WithY(height/2f);
            boxCollider.size = modelParentLocalScale.WithY(height);
            canvasParent.position = canvasTargetParent.position;
        }

        private void Awake()
        {
            modelParent.localScale = Vector3.right + Vector3.forward;
            infoText.gameObject.SetActive(false);
            modelParent.gameObject.SetActive(false);
            OnBaseDestructionDelegates = new List<OnBaseDestructionDelegate>();
        }

        protected bool Damage()
        {
            amount--;
            
            infoText.text = amount.ToString();

            DOTween.Kill(modelParent.GetInstanceID(),true);
            modelParent.DOPunchScale(punchVector, punchDuration, vibrato, elasticity).SetId(modelParent.GetInstanceID())
                .OnComplete(() =>
                {
                    modelParent.transform.localScale = new Vector3(width, height, width);
                });
            
            return amount <= 0;
        }

        public void Init(LevelSegment.BaseDestructionDelegate baseDestructionDelegate)
        {
            modelParent.gameObject.SetActive(true);
            var modelParentLocalScale = new Vector3(width, height, width);
            modelParent.DOScale(modelParentLocalScale, 0.6f).OnComplete((() => infoText.gameObject.SetActive(true)));
            BaseDestructionDelegate = baseDestructionDelegate;
        }

        public void OnDestruction(OnBaseDestructionDelegate onBaseDestructionDelegate)
        {
            OnBaseDestructionDelegates.Add(onBaseDestructionDelegate);
        }
    }
}