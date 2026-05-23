using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem
{
    public class SendScript : MonoBehaviour
    {
        public string channel = "genericSend";
        [Button("Send Signal")]
        public virtual void SendSignal(string message)
        {
            EventBus.Invoke(channel, message);
        }
    }
}
