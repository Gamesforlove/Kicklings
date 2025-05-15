using CommonDataTypes;
using UnityEngine.UI;

namespace EventBusSystem
{
    #region Gameplay

    public struct GoalEvent : IEvent
    {
        public readonly FieldSideData ScoringSideData;
        public readonly FieldSideData ScoredSideData;

        public GoalEvent(FieldSideData scoringSideData, FieldSideData scoredSideData)
        {
            ScoringSideData = scoringSideData;
            ScoredSideData = scoredSideData;
        }
    }

    #endregion
}
