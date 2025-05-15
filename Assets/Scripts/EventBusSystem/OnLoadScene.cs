using CommonDataTypes;

namespace EventBusSystem
{
    public readonly struct OnLoadScene : IEvent
    {
        public readonly SceneName EnumValue;
        public string Name => EnumValue.ToString();

        public OnLoadScene(SceneName enumValue)
        {
            EnumValue = enumValue;
        }
    }
}