using System.Threading.Tasks;
using UnityEngine;

namespace EventSystem
{
    public class CustomDataReceiveAsync : ReceiveScript
    {
        protected override void Start() => EventBus.Subscribe<CustomDataContainer>(channel, ReceiveSignal, priority);
        protected override void OnDestroy() => EventBus.Unsubscribe<CustomDataContainer>(channel, ReceiveSignal);
        /// <summary>
        /// Receives and adds the priority to the end (used to show off priority ordering).
        /// </summary>
        /// <param name="message"></param>
        public async Task ReceiveSignal(CustomDataContainer message)
        {
            Debug.Log($"[CustomDataReceiveAsync] Hit! priority {priority}");

            await Task.Delay(100);
            string result = $"Priority {priority}: Waited 100ms";
            await Task.Delay(100);
            result += $", Waited 200ms";
            await Task.Delay(100);
            result += $", Waited 300ms";

            message.data += $"\n({result})";
        }
    }
}
