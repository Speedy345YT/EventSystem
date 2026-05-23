using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem
{
    public class CustomDataReceive : ReceiveScript
    {
        private void Start() => EventBus.Subscribe<CustomDataContainer>(channel, ReceiveSignal, priority);
        private void OnDestroy() => EventBus.Unsubscribe<CustomDataContainer>(channel, ReceiveSignal);
        /// <summary>
        /// Receives and adds the priority to the end (used to show off priority ordering).
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveSignal(CustomDataContainer message)
        {
            message.data = $"{message.data} + {priority}";
        }
    }
}
