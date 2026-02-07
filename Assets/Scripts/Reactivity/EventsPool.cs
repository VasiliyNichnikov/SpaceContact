#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Reactivity
{
    internal class EventsPool : IEventsPool
    {
        private readonly Queue<EventWrapper> _events = new Queue<EventWrapper>();

        public EventWrapper Get()
        {
            if (_events.Count == 0)
            {
                var eventWrapper = new EventWrapper(this);
                
                return eventWrapper;
            }

            var existingEvent = _events.Dequeue();
            
            return existingEvent;
        }

        public void Remove(EventWrapper eventWrapper)
        {
            if (_events.Contains(eventWrapper))
            {
                Debug.LogError("EventsPool.Remove: eventWrapper is already contains in queue.");
                return;
            }

            _events.Enqueue(eventWrapper);
        }
    }
}