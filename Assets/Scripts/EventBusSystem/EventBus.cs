using System;
using JetBrains.Annotations;

namespace EventBusSystem
{
    public static class EventBus<T> where T : IEvent
    {
        public static event Action<T> OnEvent;

        public static void Raise(T evt) => OnEvent?.Invoke(evt);
        
        public static void Raise() => Raise(default);
    }
}