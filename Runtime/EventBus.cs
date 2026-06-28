using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace EventSystem
{
    public static class EventBus
    {
        private static readonly Dictionary<(string, Type), List<PrioritizedHandler>> _handlers
            = new Dictionary<(string, Type), List<PrioritizedHandler>>();

        private static readonly List<(string, Type)> _dirty = new List<(string, Type)>();
        /// <summary>
        /// Allows you to subscribe and receive information when an event is Invoked
        /// </summary>
        /// <typeparam name="T">The type of information you hope to receive Example:(string, int, CustomDataContainers, etc)</typeparam>
        /// <param name="channel">The name of the channel you're subscribing to Example:("OnPlayerHit")</param>
        /// <param name="handler">The method that is triggered Example:(Item.OnPlayerHit)</param>
        /// <param name="priority">What order this will trigger in Example:(100)</param>
        public static void Subscribe<T>(string channel, Func<T, Task> handler, int priority = 0)
        {
            GetOrCreate<T>(channel).Add(new PrioritizedHandler(
                priority,
                handler,
                payload => handler((T)payload)
            ));
        }
        public static void Subscribe<T>(string channel, Action<T> handler, int priority = 0)
        {
            GetOrCreate<T>(channel).Add(new PrioritizedHandler(
                priority,
                handler,
                payload => { handler((T)payload); return Task.CompletedTask; }
            ));
        }
        public static void Subscribe(string channel, Action handler, int priority = 0)
        {
            GetOrCreate<NoPayload>(channel).Add(new PrioritizedHandler(
                priority,
                handler,
                _ => { handler(); return Task.CompletedTask; }
            ));
        }

        /// <summary>
        /// Subscribe with no payload. Handler is awaited before the next handler fires.
        /// </summary>
        public static void Subscribe(string channel, Func<Task> handler, int priority = 0)
        {
            GetOrCreate<NoPayload>(channel).Add(new PrioritizedHandler(
                priority,
                handler,
                _ => handler()
            ));
        }
        /// <summary>
        /// Unsubscribe from receiving information
        /// </summary>
        /// <typeparam name="T">Used for narrowing down the Method that is removed</typeparam>
        /// <param name="channel">Channel that is being unsubscribed from</param>
        /// <param name="handler">The Method that no longer will listen for event triggers</param>
        public static void Unsubscribe<T>(string channel, Action<T> handler) => Remove<T>(channel, handler);
        public static void Unsubscribe<T>(string channel, Func<T, Task> handler) => Remove<T>(channel, handler);
        public static void Unsubscribe(string channel, Action handler) => Remove<NoPayload>(channel, handler);
        public static void Unsubscribe(string channel, Func<Task> handler) => Remove<NoPayload>(channel, handler);

        /// <summary>
        /// Fires all handlers on the channel in priority order.
        /// Returns the payload after all handlers have run so callers can read back any modifications.
        /// </summary>
        public static T Raise<T>(string channel, T payload)
        {
            foreach (var h in Snapshot<T>(channel))
                h.Invoke(payload);

            return payload;
        }
        public static void Raise(string channel)
        {
            foreach (var h in Snapshot<NoPayload>(channel))
                h.Invoke(null);
        }
        public static async Task<T> RaiseAsync<T>(string channel, T payload)
        {
            foreach (var h in Snapshot<T>(channel))
            {
                await h.InvokeAsync(payload).ConfigureAwait(false);
            }
            return payload;
        }
        public static async Task RaiseAsync(string channel)
        {
            foreach (var h in Snapshot<NoPayload>(channel))
                await h.InvokeAsync(null).ConfigureAwait(false);
        }

        public static void ClearAll()
        {
            _handlers.Clear();
            _dirty.Clear();
        }
        private static List<PrioritizedHandler> GetOrCreate<T>(string channel)
        {
            var key = (channel, typeof(T));
            if (!_handlers.TryGetValue(key, out var list))
            {
                list = new List<PrioritizedHandler>();
                _handlers[key] = list;
            }
            _dirty.Add(key);
            return list;
        }

        private static List<PrioritizedHandler> Snapshot<T>(string channel)
        {
            var key = (channel, typeof(T));
            if (!_handlers.TryGetValue(key, out var list) || list.Count == 0)
                return new List<PrioritizedHandler>();

            if (_dirty.Contains(key))
            {
                list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
                _dirty.Remove(key);
            }
            Debug.Log($"[EventBus] Snapshot found {list.Count} handlers for '{channel}' {typeof(T).Name}");
            return new List<PrioritizedHandler>(list);
        }

        private static void Remove<T>(string channel, Delegate handler)
        {
            if (_handlers.TryGetValue((channel, typeof(T)), out var list))
                list.RemoveAll(h => h.Matches(handler));
        }
        public static void Clear(string channel) => _handlers.Remove((channel, typeof(void)));

        private class PrioritizedHandler
        {
            public int Priority { get; }

            private readonly Delegate _original;
            private readonly Func<object, Task> _action;

            public PrioritizedHandler(int priority, Delegate original, Func<object, Task> action)
            {
                Priority = priority;
                _original = original;
                _action = action;
            }

            public bool Matches(Delegate handler)
                => _original.Target == handler.Target
                && _original.Method == handler.Method;
            public void Invoke(object payload) {
                _action(payload);
            }
            public Task InvokeAsync(object payload)
            {
                return _action(payload);
            }
        }
        private class NoPayload { }
    }
}

