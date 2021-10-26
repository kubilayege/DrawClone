using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Wait
    {
        public static WaitForFixedUpdate FixedUpdate { get; } = new WaitForFixedUpdate();
        public static WaitForEndOfFrame EndOfFrame { get; } = new WaitForEndOfFrame();
        public static WaitForSeconds HalfSecond { get; } = new WaitForSeconds(0.5f);
        public static WaitForSeconds OneSecond { get; } = new WaitForSeconds(1);

        private static Dictionary<float, WaitForSeconds> _waitForSecondsList;

        public static WaitForSeconds ForSeconds(float seconds)
        {
            if (_waitForSecondsList == null)
            {
                _waitForSecondsList = new Dictionary<float, WaitForSeconds>();
            }

            if (!_waitForSecondsList.ContainsKey(seconds))
            {
                _waitForSecondsList.Add(seconds,new WaitForSeconds(seconds));
            }
        
            return _waitForSecondsList[seconds];
        }

    }
}