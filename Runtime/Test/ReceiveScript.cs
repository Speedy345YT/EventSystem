using UnityEngine;

namespace EventBusSystem
{
    public class ReceiveScript : MonoBehaviour
    {
        public int priority;
        public string channel = "genericSend";
        protected virtual void Start() => EventBus.Subscribe<string>(channel, ReceiveSignal, priority);
        protected virtual void OnDestroy() => EventBus.Unsubscribe<string>(channel, ReceiveSignal);
        /// <summary>
        /// Receives the signal from the channel
        /// </summary>
        /// <param name="message">The message received</param>
        public void ReceiveSignal(string message)
        {
            Debug.Log($"{message} Priority:{priority}");
        }
    }
}
