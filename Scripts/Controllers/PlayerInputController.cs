using System.Collections.Generic;
using Core;
using Managers;
using MonoObjects.Agent;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Controllers
{
    public class PlayerInputController : MonoSingleton<PlayerInputController>
    {
        [SerializeField] private EventController _inGameEventController;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private LayerMask _drawableMask;
        [SerializeField] private LayerMask _drawStartMask;
        [SerializeField] private LayerMask _enemyKillMask;

        private LevelManager _levelManager;
            
        private Vector2 mouseDownStart;
        private float ScreenWidth;
        private float ScreenHeight;

        private DrawController _currentDrawController;
        
        private void Start()
        {
            ScreenWidth = Screen.width;
            ScreenWidth = Screen.height;
            _levelManager = LevelManager.Instance;
            ActionManager.Instance.AddAction(ActionIDHolder.PrepareLevelID, RemoveEvents);
            ActionManager.Instance.AddAction(ActionIDHolder.OnSegmentCompleteID, ClearDrawing);
        }

        private void ClearDrawing()
        {
            if (_currentDrawController != null)
            {
                _currentDrawController.ClearDrawing();
                _currentDrawController = null;
            }
        }

        private void AddEvents()
        {
            _inGameEventController.dragStarted.AddListener(OnDragStart);
            _inGameEventController.dragged.AddListener(OnDragged);
            _inGameEventController.dragEnded.AddListener(OnDragEnd);
        }

        private void RemoveEvents()
        {
            _inGameEventController.dragStarted.RemoveListener(OnDragStart);
            _inGameEventController.dragged.RemoveListener(OnDragged);
            _inGameEventController.dragEnded.RemoveListener(OnDragEnd);
        }

        public void SetInputPanel(EventController inputPanel)
        {
            _inGameEventController = inputPanel;
            
            AddEvents();
        }

        private void OnDragStart(PointerEventData pointerEvent)
        {
            if (Helper.TryGetObjectOfType(pointerEvent.position, _drawStartMask,
                out _currentDrawController))
            {
                var worldPos = Helper.GetWorldPos(pointerEvent.position, _drawableMask);
                if(_levelManager.CurrentLevel.IsPointInNavMesh(worldPos) )
                {
                    _currentDrawController.OnDragStart(worldPos);
                    PlayerController.Instance.StartDrawing(worldPos);
                }
            }
            else
            {
                if (Helper.TryOverlapOfType(pointerEvent.position, _drawableMask, _enemyKillMask,
                    out List<EnemyAgent> agents))
                {
                    
                    var worldPos = Helper.GetWorldPos(pointerEvent.position, _drawableMask);
                    PlayerController.Instance.Kill(agents, worldPos);
                }
                _currentDrawController = null;
            }
        }
        
        private void OnDragged(PointerEventData pointerEvent)
        {
            if (_currentDrawController == null)
            {
                PlayerController.Instance.StopDrawing();
                return;
            }
            
            var worldPos = Helper.GetWorldPos(pointerEvent.position, _drawableMask);
            if (_levelManager.CurrentLevel.IsPointInNavMesh(worldPos) )
            {
                _currentDrawController.OnDrag(worldPos);
                PlayerController.Instance.DrawNext(worldPos);

            }
            else
            {
                
                PlayerController.Instance.StopDrawing();
                _currentDrawController.StopDrawing();
                _currentDrawController = null;
            }
        }
        
        private void OnDragEnd(PointerEventData pointerEvent)
        {
            if(_currentDrawController != null)
            {
                PlayerController.Instance.StopDrawing();
                _currentDrawController.OnRelease();
            }
            
            
        }
    }
}