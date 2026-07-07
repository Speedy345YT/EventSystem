using UnityEngine;

namespace EventBusSystem
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
