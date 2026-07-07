using System.Threading.Tasks;
using UnityEngine;

namespace EventBusSystem
{
    public class CustomDataSendAsync : SendScript
    {
        [ContextMenu("Send Async Data")]
        public override async void SendSignal()
        {
            //Create new message container
            CustomDataContainer _message = new CustomDataContainer(message);

            //Create a new message container with the result of all of the changes from Subscribed Listeners
            CustomDataContainer newMessage = await EventBus.RaiseAsync(channel, _message);

            //Read from new message container
            Debug.Log($"Final Message:{newMessage.data}");
        }
    }
}
