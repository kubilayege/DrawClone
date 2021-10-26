using Core;
using ScriptableObjects;
using UnityEngine;
using MonoObjects.Core;

namespace Managers
{
    public class LevelManager : MonoSingleton<LevelManager> 
    {
        [SerializeField] private LevelPool levelPool;
        public int CurrentLevelNumber
        {
            get
            {
                if (PlayerPrefs.GetInt("Level") <= 0)
                    PlayerPrefs.SetInt("Level", 1);
                return PlayerPrefs.GetInt("Level");
            }
            private set => PlayerPrefs.SetInt("Level", value);
        }
        
        public int CurrentLevelIndex => (CurrentLevelNumber - 1) % levelPool.Length;

        private Level _currentLevel;

        public Level CurrentLevel => _currentLevel;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            ActionManager.Instance.AddAction(ActionIDHolder.PrepareLevelID, PrepareLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelPreparedID, LevelInit);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelStartedID, StartLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelFailedID, FailLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.OnSegmentCompleteID, NextSegment);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelCompleted, FinishLevel);
        }

        private void NextSegment()
        {
            _currentLevel.OnSegmentFinish();
        }

        private void PrepareLevel()
        {
            if (_currentLevel != null)
            {
                _currentLevel.DestroyLevel();
            }
            
            _currentLevel = Instantiate(levelPool.GetLevel(CurrentLevelIndex));

            ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelPreparedID);
        }

        private void LevelInit()
        {
            _currentLevel.Initialize(CurrentLevelNumber);
        }
        
        private void StartLevel()
        {
            _currentLevel.StartLevel();
            Debug.Log("Level Started");
            TinySauce.OnGameStarted(CurrentLevelNumber.ToString());
        }
        
        private void FailLevel()
        {
            _currentLevel.OnLevelFail();
            Debug.Log("Level Failed");
            TinySauce.OnGameFinished(false, 0f, CurrentLevelNumber.ToString());
        }
        
        private void FinishLevel()
        {
            CurrentLevelNumber += 1;
            Debug.Log("Level Succeed");
            TinySauce.OnGameFinished(true, UIManager.Instance.GetPlayerScore(), CurrentLevelNumber.ToString());
        }

        public Transform GetCurrentLevelParent()
        {
            return _currentLevel.transform;
        }
    }
}
