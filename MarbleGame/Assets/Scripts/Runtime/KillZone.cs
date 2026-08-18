using UnityEngine;

namespace MarbleGame
{
    public sealed class KillZone : MonoBehaviour
    {
        [SerializeField] private string message = "THE MACHINE REJECTED THAT ROUTE.";
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<MarblePlayer>() != null) GameDirector.Instance?.Fail(message);
        }
    }
}
