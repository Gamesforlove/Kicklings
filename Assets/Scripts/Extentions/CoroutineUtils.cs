using System;
using UnityEngine;

public static class CoroutineUtils
{
    public class WaitForEvent : CustomYieldInstruction
    {
        private bool eventRaised = false;

        public WaitForEvent(Action<Action> subscribe, Action<Action> unsubscribe)
        {
            void Handler()
            {
                eventRaised = true;
                unsubscribe(Handler);
            }

            subscribe(Handler);
        }

        public override bool keepWaiting => !eventRaised;
    }
    public class WaitForEvent<T> : CustomYieldInstruction
    {
        private bool eventRaised = false;
        public T Value { get; private set; }

        public WaitForEvent(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe)
        {
            void Handler(T value)
            {
                Value = value;
                eventRaised = true;
                unsubscribe(Handler);
            }
            subscribe(Handler);
        }
        public override bool keepWaiting => !eventRaised;
    }
}
