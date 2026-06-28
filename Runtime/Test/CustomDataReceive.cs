using UnityEngine;

namespace EventSystem
{
    public class CustomDataReceive : ReceiveScript
    {
        protected override void Start() => EventBus.Subscribe<CustomDataContainer>(channel, ReceiveSignal, priority);
        protected override void OnDestroy() => EventBus.Unsubscribe<CustomDataContainer>(channel, ReceiveSignal);
        /// <summary>
        /// Receives and adds the priority to the end (used to show off priority ordering).
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveSignal(CustomDataContainer message)
        {
            Debug.Log($"[CustomDataReceive] Hit! priority {priority}");

            message.data = $"{message.data} + {priority}";
        }
    }
}
