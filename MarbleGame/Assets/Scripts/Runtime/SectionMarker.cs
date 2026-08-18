using UnityEngine;

namespace MarbleGame
{
    public sealed class SectionMarker : MonoBehaviour
    {
        [SerializeField] private string label = "SECTOR";
        public void Configure(string value) => label = value;
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<MarblePlayer>() != null) GameDirector.Instance?.SetSection(label);
        }
    }
}
