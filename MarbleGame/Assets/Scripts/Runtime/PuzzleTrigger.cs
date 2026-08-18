using UnityEngine;

namespace MarbleGame
{
    public sealed class PuzzleTrigger : MonoBehaviour
    {
        [SerializeField] private string failureMessage = "THE OBVIOUS ROUTE WAS A LIE.";
        [SerializeField] private string sectionLabel;
        [SerializeField] private bool disableAfterActivation;
        private bool activated;

        public void Configure(string message, string label = null)
        {
            failureMessage = message;
            sectionLabel = label;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (activated && disableAfterActivation) return;
            MarblePlayer player = other.GetComponentInParent<MarblePlayer>();
            if (player == null) return;
            activated = true;
            if (!string.IsNullOrEmpty(sectionLabel)) GameDirector.Instance?.SetSection(sectionLabel);
            GameDirector.Instance?.Fail(failureMessage);
        }
    }
}
