using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystem
{
    public static class EventBus
    {
        private static readonly Dictionary<string, List<PrioritizedHandler>> _handlers
            = new Dictionary<string, List<PrioritizedHandler>>();

        private static readonly List<string> _dirty = new List<string>();
        /// <summary>
        /// Allows you to subscribe and receive information when an event is Invoked
        /// </summary>
        /// <typeparam name="T">The type of information you hope to receive Example:(string, int, CustomDataContainers, etc)</typeparam>
        /// <param name="channel">The name of the channel you're subscribing to Example:("OnPlayerHit")</param>
        /// <param name="handler">The method that is triggered Example:(Item.OnPlayerHit)</param>
        /// <param name="priority">What order this will trigger in Example:(100)</param>
        public static void Subscribe<T>(string channel, Action<T> handler, int priority = 100)
        {
            if (!_handlers.TryGetValue(channel, out var list))
            {
                list = new List<PrioritizedHandler>();
                _handlers[channel] = list;
            }

            list.Add(new PrioritizedHandler(priority, handler, e => handler((T)e)));
            _dirty.Add(channel);
        }
        /// <summary>
        /// Unsubscribe from receiving information
        /// </summary>
        /// <typeparam name="T">Used for narrowing down the Method that is removed</typeparam>
        /// <param name="channel">Channel that is being unsubscribed from</param>
        /// <param name="handler">The Method that no longer will listen for event triggers</param>
        public static void Unsubscribe<T>(string channel, Action<T> handler)
        {
            if (!_handlers.TryGetValue(channel, out var list))
                return;

            list.RemoveAll(h => h.Matches(handler));
        }

        /// <summary>
        /// Fires all handlers on the channel in priority order.
        /// Returns the payload after all handlers have run so callers
        /// can read back any modifications.
        /// </summary>
        public static T Invoke<T>(string channel, T payload)
        {
            if (!_handlers.TryGetValue(channel, out var list) || list.Count == 0)
                return payload;

            if (_dirty.Contains(channel))
            {
                list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
                _dirty.Remove(channel);
            }

            var snapshot = new List<PrioritizedHandler>(list);

            foreach (var h in snapshot)
                h.Invoke(payload);

            return payload;
        }

        public static void Clear(string channel) => _handlers.Remove(channel);
        public static void ClearAll()
        {
            _handlers.Clear();
            _dirty.Clear();
        }
        private class PrioritizedHandler
        {
            public int Priority { get; }

            private Delegate _original;
            private Action<object> _action;

            public PrioritizedHandler(int priority, Delegate original, Action<object> action)
            {
                Priority = priority;
                _original = original;
                _action = action;
            }

            public bool Matches<T>(Action<T> handler)
                => _original.Target == handler.Target
                && _original.Method == handler.Method;

            public void Invoke(object payload) => _action(payload);
        }
    }
}