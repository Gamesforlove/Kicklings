using CommonDataTypes;

namespace EventBusSystem
{
    public struct OutEvent : IEvent
    {
        public readonly FieldSideData FieldSideData;

        public OutEvent(FieldSideData data)
        {
            FieldSideData = data;
        }
    }
}