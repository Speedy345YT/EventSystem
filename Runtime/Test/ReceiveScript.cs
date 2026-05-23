using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem
{
    public class ReceiveScript : MonoBehaviour
    {
        #region Variables
        #if ODIN_INSPECTOR
        [DisableInPlayMode]
        #endif
        public int priority;
        #if ODIN_INSPECTOR
        [DisableInPlayMode]
        #endif
        #endregion
        public string channel = "genericSend";
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
