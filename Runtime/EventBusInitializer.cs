using UnityEngine;

namespace EventSystem
{
    public class EventBusInitializer : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("[EventBus] ClearAll");
            EventBus.ClearAll();
        }
    }
}
