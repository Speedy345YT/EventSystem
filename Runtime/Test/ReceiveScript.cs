using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem
{
    public class ReceiveScript : MonoBehaviour
    {
        [DisableInPlayMode]public int priority;
        [DisableInPlayMode]public string channel = "genericSend";
        private void Start() => EventBus.Subscribe<string>(channel, ReceiveSignal, priority);
        private void OnDestroy() => EventBus.Unsubscribe<string>(channel, ReceiveSignal);
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
