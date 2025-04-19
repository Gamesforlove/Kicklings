using CommonDataTypes;

namespace EventBusSystem
{
    public interface IEvent { }

    #region Gameplay

    public struct GoalEvent : IEvent
    {
        public readonly FieldSideData FieldSideData;

        public GoalEvent(FieldSideData data)
        {
            FieldSideData = data;
        }
    }
    
    public struct OutEvent : IEvent
    {
        public readonly FieldSideData FieldSideData;

        public OutEvent(FieldSideData data)
        {
            FieldSideData = data;
        }
    }
    
    public struct PlayerActionPerformed : IEvent {}
    
    public struct PlayerActionCanceled : IEvent {}
    
    public struct PlayerJumped : IEvent {}

    #endregion

    #region UI

    public struct CreateMatch : IEvent
    {
        public readonly GameModeData GameModeData;

        public CreateMatch(GameModeData gameModeData)
        {
            GameModeData = gameModeData;
        }
    }
    
    #endregion
}
