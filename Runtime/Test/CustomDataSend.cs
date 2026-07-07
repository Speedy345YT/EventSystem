using UnityEngine;

namespace EventBusSystem
{
    public class CustomDataSend : SendScript
    {
        [ContextMenu("Send Custom Data")]
        public override void SendSignal()
        {
            //Create new message container
            CustomDataContainer _message = new CustomDataContainer(message);

            //Create a new message container with the result of all of the changes from Subscribed Listeners
            CustomDataContainer newMessage = EventBus.Raise(channel, _message);

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
