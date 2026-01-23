using CommonDataTypes;

namespace EventBusSystem
{
    public readonly struct OnSceneLoaded : IEvent
    {
        public readonly SceneName EnumValue;
        public string SceneName => EnumValue.ToString();

        public OnSceneLoaded(SceneName enumValue)
        {
            EnumValue = enumValue;
        }
    }
}