using UnityEngine;

namespace MarbleGame
{
    public sealed class PressureGate : MonoBehaviour, ILevelResettable
    {
        [SerializeField] private Transform gate;
        [SerializeField] private Vector3 openOffset = new Vector3(0f, 2.3f, 0f);
        private Vector3 closed;
        private int occupants;
        public void Configure(Transform targetGate) { gate = targetGate; closed = gate != null ? gate.localPosition : Vector3.zero; }
        private void Start() { if (gate != null) closed = gate.localPosition; }
        private void OnTriggerEnter(Collider other) { if (other.GetComponentInParent<MarblePlayer>() != null) occupants++; }
        private void OnTriggerExit(Collider other) { if (other.GetComponentInParent<MarblePlayer>() != null) occupants = Mathf.Max(0, occupants - 1); }
        private void Update()
        {
            if (gate == null) return;
            Vector3 target = closed + (occupants > 0 ? openOffset : Vector3.zero);
            gate.localPosition = Vector3.MoveTowards(gate.localPosition, target, Time.deltaTime * 3.5f);
        }

        public void ResetMechanism()
        {
            occupants = 0;
            if (gate != null) gate.localPosition = closed;
        }
    }
}
