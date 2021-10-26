using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{

    [AddComponentMenu("Managers/Action Manager")]
    public class ActionManager : MonoSingleton<ActionManager>
    {
        #region Properties

        private Dictionary<int, ListenerObject> _actionListeners = new Dictionary<int, ListenerObject>();

        public delegate void ActionDelegate();
        
        public delegate void ActionDelegateWithParam(int param);

        private static int _lastTriggerIndex = -1;

        #endregion

        #region Functions
        //Trigger an action 
        public void TriggerAction(int action)
        {
            if (_actionListeners.ContainsKey(action))
            {
                _actionListeners[action].ProcessDelegates();
            }
        }

        // Add new action to listener
        public void AddAction(int action, ActionDelegate newAction)
        {
            if (_actionListeners.ContainsKey(action))
            {
                _actionListeners[action].AddListener(newAction);
            }
            else
            {
                _actionListeners[action] = new ListenerObject();
                _actionListeners[action].AddListener(newAction);
            }
        }

        //Remove one action from list
        public void RemoveListener(int triggerName, ActionDelegate processToRemove)
        {
            if (_actionListeners.TryGetValue(triggerName, out ListenerObject temp))
            {
                temp.RemoveListener(processToRemove);
            }
        }

        //Get new trigger index for actions
        public static int GetTriggerIndex()
        {
            return ++_lastTriggerIndex;
        }

        public class ListenerObject
        {
            ActionDelegate _thisActionDelegate;

            public void ProcessDelegates()
            {
                _thisActionDelegate?.Invoke();
            }
            public void AddListener(ActionDelegate param)
            {
                _thisActionDelegate += param;
            }
            
            public void RemoveListener(ActionDelegate param)
            {
                _thisActionDelegate -= param;
            }
        }

        // Delete all actions
        public void ClearListeners() { _actionListeners.Clear(); }

        #endregion
    }
}
