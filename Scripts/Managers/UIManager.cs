using System;
using Controllers;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private GameObject TapToPlayPanel;
        [SerializeField] private GameObject GamePanel;
        [SerializeField] private GameObject LevelCompletePanel;
        [SerializeField] private GameObject LevelFailedPanel;
    

        [SerializeField] private TextMeshProUGUI totalScore;
        [SerializeField] private TextMeshProUGUI earnedScore;
        [SerializeField] private TextMeshProUGUI[] levelInfoTexts;
        [SerializeField] private TweenData panelAnimationData;

        [SerializeField] private EventController inGameInputController;
        

        private int currentLevelScore;
        private int currentPlayerScore;
        private int PlayerTotalScore {
            get
            {
                if(!PlayerPrefs.HasKey("Score"))
                    PlayerPrefs.SetInt("Score", 0);
                return PlayerPrefs.GetInt("Score");
            }
            set => PlayerPrefs.SetInt("Score", value);
        }
        
        private void Awake()
        {
            ActionManager.Instance.AddAction(ActionIDHolder.PrepareLevelID, OnPrepareLevel);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelPreparedID, OnLevelPrepared);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelStartedID, OnLevelStarted);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelCompleted, OnLevelComplete);
            ActionManager.Instance.AddAction(ActionIDHolder.OnLevelFailedID, OnLevelFailed);
        }

        private void OnLevelPrepared()
        {
            foreach (var levelInfoText in levelInfoTexts)
            {
                levelInfoText.text = $"LEVEL {LevelManager.Instance.CurrentLevelNumber}";
            }
        }

        private void OnLevelFailed()
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(true);
            
            DoPanelTransition(LevelFailedPanel.transform);
        }


        private void OnLevelStarted()
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(true);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(false);

            // DoPanelTransition(GamePanel.transform);

            PlayerInputController.Instance.SetInputPanel(inGameInputController);
        }
        
        private void OnLevelComplete()
        {
            TapToPlayPanel.SetActive(false);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(true);
            LevelFailedPanel.SetActive(false);
            
            
            PlayerTotalScore = currentLevelScore + currentPlayerScore;
            
            DoPanelTransition(LevelCompletePanel.transform);
        }
        
        private void OnPrepareLevel()
        {
            currentPlayerScore = PlayerTotalScore;
            currentLevelScore = 0;
            AddScore(0);
            
            TapToPlayPanel.SetActive(true);
            GamePanel.SetActive(false);
            LevelCompletePanel.SetActive(false);
            LevelFailedPanel.SetActive(false);

            // DoPanelTransition(TapToPlayPanel.transform);
        }

        public void AddScore(int addAmount)
        {
            currentLevelScore += addAmount;
            
            totalScore.text = $"{currentPlayerScore + currentLevelScore}";
            earnedScore.text =  $"{currentLevelScore}";
        }
        
        public void TapToPlay()
        {
            ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelStartedID);
        }
        
        public void Continue()
        {
            ActionManager.Instance.TriggerAction(ActionIDHolder.PrepareLevelID);
        }
        
        public void PassLevel()
        {
            ActionManager.Instance.TriggerAction(ActionIDHolder.OnSegmentCompleteID);
        }
        
        public void FailLevel()
        {
            ActionManager.Instance.TriggerAction(ActionIDHolder.OnLevelFailedID);
        }

        private void DoPanelTransition(Transform panel)
        {
            DOTween.Kill("PanelTween");
            panel.gameObject.SetActive(true);
            panel.localScale = Vector3.zero;
            panel.DOScale(Vector3.one, panelAnimationData.duration).SetEase(panelAnimationData.ease).OnKill(() =>
            {
                panel.localScale = Vector3.one;
            }).SetId("PanelTween");
        }

        public int GetPlayerScore()
        {
            return currentLevelScore;
        }
    }
}