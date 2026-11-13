using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace EventBusSystem
{
    public static class EventBus
    {
        private static readonly Dictionary<object, List<PrioritizedHandler>> _handlers
            = new Dictionary<object, List<PrioritizedHandler>>();

        /// <summary>
        /// Allows you to subscribe and receive information when an event is Invoked
        /// </summary>
        /// <typeparam name="T">The type of information you hope to receive Example:(string, int, CustomDataContainers, etc)</typeparam>
        /// <param name="channel">The name of the channel you're subscribing to Example:("OnPlayerHit")</param>
        /// <param name="handler">The method that is triggered Example:(Item.OnPlayerHit)</param>
        /// <param name="priority">What order this will trigger in Example:(100)</param>
        public static void Subscribe<T>(object channel, Func<T, Task> handler, int priority = 0)
        {
            GetOrCreate<T>(channel).Add(new PrioritizedHandler(
                priority,
                payload => handler((T)payload)
            ));
        }

        public static void Subscribe<T>(object channel, Action<T> handler, int priority = 0)
        {
            GetOrCreate<T>(channel).Add(new PrioritizedHandler(
                priority,
                payload =>
                {
                    handler((T)payload);
                    return Task.CompletedTask;
                }
            ));
        }

        public static void Subscribe(object channel, Action handler, int priority = 0)
        {
            GetOrCreate<NoPayload>(channel).Add(new PrioritizedHandler(
                priority,
                _ =>
                {
                    handler();
                    return Task.CompletedTask;
                }
            ));
        }

        /// <summary>
        /// Subscribe with no payload. Handler is awaited before the next handler fires.
        /// </summary>
        public static void Subscribe(object channel, Func<Task> handler, int priority = 0)
        {
            GetOrCreate<NoPayload>(channel).Add(new PrioritizedHandler(
                priority,
                _ => handler()
            ));
        }
        /// <summary>
        /// Unsubscribe from receiving information
        /// </summary>
        /// <typeparam name="T">Used for narrowing down the Method that is removed</typeparam>
        /// <param name="channel">Channel that is being unsubscribed from</param>
        /// <param name="handler">The Method that no longer will listen for event triggers</param>
        public static void Unsubscribe<T>(object channel, Action<T> handler) => Remove<T>(channel, handler);
        public static void Unsubscribe<T>(object channel, Func<T, Task> handler) => Remove<T>(channel, handler);
        public static void Unsubscribe(object channel, Action handler) => Remove<NoPayload>(channel, handler);
        public static void Unsubscribe(object channel, Func<Task> handler) => Remove<NoPayload>(channel, handler);

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
                await h.InvokeAsync(payload).ConfigureAwait(false);

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
        }
        private static List<PrioritizedHandler> GetOrCreate<T>(object channel)
        {
            var key = (channel, typeof(T));
            if (!_handlers.TryGetValue(key, out var list))
            {
                list = new List<PrioritizedHandler>();
                _handlers[key] = list;
            }
            return list;
        }

        private static List<PrioritizedHandler> Snapshot<T>(object channel)
        {
            var key = (channel, typeof(T));
            if (!_handlers.TryGetValue(key, out var list) || list.Count == 0)
                return new List<PrioritizedHandler>();

            list.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            Debug.Log($"[EventBus] Snapshot found {list.Count} handlers for '{channel}' {typeof(T).Name}");
            return new List<PrioritizedHandler>(list);
        }

        private static void Remove<T>(object channel, Delegate handler)
        {
            if (_handlers.TryGetValue((channel, typeof(T)), out var list))
                list.RemoveAll(h => h.Matches(handler));
        }

        public static void Clear(object channel) => _handlers.Remove((channel, typeof(void)));

        private class PrioritizedHandler
        {
            public int Priority { get; }

            private readonly Func<object, Task> _action;

            public PrioritizedHandler(int priority, Func<object, Task> action)
            {
                Priority = priority;
                _action = action;
            }

            public bool Matches(Delegate handler)
                => _action.Target == handler.Target
                && _action.Method == handler.Method;
            public object Invoke(object payload) {
                return _action(payload);
            }
            public Task InvokeAsync(object payload)
            {
                return _action(payload);
            }
        }
        private class NoPayload { }
    }
}
