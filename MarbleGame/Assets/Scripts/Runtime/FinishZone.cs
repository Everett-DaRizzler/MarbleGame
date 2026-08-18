using UnityEngine;

namespace MarbleGame
{
    public sealed class FinishZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<MarblePlayer>() != null) GameDirector.Instance?.Complete();
        }
    }
}
