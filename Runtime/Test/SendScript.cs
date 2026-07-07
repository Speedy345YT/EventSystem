using UnityEngine;

namespace EventBusSystem
{
    public class SendScript : MonoBehaviour
    {
        public string channel = "genericSend";
        public string message = "Hello World";
        public virtual void SendSignal()
        {
            EventBus.Raise(channel, message);
        }
        [ContextMenu("Send Signal")]
        public void SendMessage()
        {
            SendSignal();
        }
    }
}
