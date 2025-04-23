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

    public readonly struct OnLoadScene : IEvent
    {
        readonly Scene _scene;
        public string SceneName => _scene.ToString();

        public OnLoadScene(Scene scene)
        {
            _scene = scene;
        }
    }
}
