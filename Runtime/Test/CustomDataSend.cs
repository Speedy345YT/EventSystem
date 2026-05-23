using Sirenix.OdinInspector;
using UnityEngine;

namespace EventSystem
{
    public class CustomDataSend : SendScript
    {
        public override void SendSignal(string text)
        {
            //Create new message container
            CustomDataContainer message = new CustomDataContainer(text);

            //Create a new message container with the result of all of the changes from Subscribed Listeners
            CustomDataContainer newMessage = EventBus.Invoke(channel, message);

            //Read from new message container
            Debug.Log($"Final Message:{newMessage.data}");
        }
    }
    public class CustomDataContainer
    {
        public string data;
        public CustomDataContainer(string data)
        {
            this.data = data;
        }
    }
}
