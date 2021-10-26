using Managers;

namespace Core
{
    public static class ActionIDHolder
    {
        // Level failed event ID
        private static int _onLevelFailedID = ActionManager.GetTriggerIndex();
        public static int OnLevelFailedID { get { return _onLevelFailedID; } }

        // Level finish event ID
        private static int _onLevelCompleted = ActionManager.GetTriggerIndex();
        public static int OnLevelCompleted { get { return _onLevelCompleted; } }
        
        // Level completed event ID
        private static int _onSegmentCompleteID = ActionManager.GetTriggerIndex();
        public static int OnSegmentCompleteID { get { return _onSegmentCompleteID; } }

        // Level completed event ID
        private static int _onSectionObjectiveFinish = ActionManager.GetTriggerIndex();
        public static int OnSectionObjectiveFinish { get { return _onSectionObjectiveFinish; } }
        
        // Level is prepared event ID
        private static int _onLevelPreparedID = ActionManager.GetTriggerIndex();
        public static int OnLevelPreparedID { get { return _onLevelPreparedID; } }
    
        // Level completed event ID
        private static int _onLevelStartedID = ActionManager.GetTriggerIndex();
        public static int OnLevelStartedID { get { return _onLevelStartedID; } }
        
        // Level prepare event ID
        private static int _prepareLevelID = ActionManager.GetTriggerIndex();
        public static int PrepareLevelID { get { return _prepareLevelID; } }

        // Coin Pickup
        private static int _coinPickup = ActionManager.GetTriggerIndex();
        public static int CoinPickup { get { return _coinPickup; } }

    }
}
