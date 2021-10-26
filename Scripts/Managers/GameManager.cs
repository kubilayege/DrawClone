using Controllers;
using Core;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject cameraPrefab;

        private void Start()
        {
            if (PlayerController.Instance != null)
            {
                ActionManager.Instance.TriggerAction(ActionIDHolder.PrepareLevelID);
            }
        }
    }
}