using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem
{
    public class SendScript : MonoBehaviour
    {
        public string channel = "genericSend";
        #if ODIN_INSPECTOR
        [Button("Send Signal")]
        #endif
        public virtual void SendSignal(string message)
        {
            EventBus.Invoke(channel, message);
        }
    }
}
