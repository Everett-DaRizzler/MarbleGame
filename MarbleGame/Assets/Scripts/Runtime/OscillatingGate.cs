using UnityEngine;

namespace MarbleGame
{
    public sealed class OscillatingGate : MonoBehaviour, ILevelResettable
    {
        [SerializeField] private Vector3 travel = new Vector3(0f, 0f, 2f);
        [SerializeField] private float speed = 1.2f;
        [SerializeField] private float phase;
        private Vector3 start;
        public void Configure(Vector3 travelAmount, float movementSpeed, float movementPhase)
        {
            travel = travelAmount;
            speed = movementSpeed;
            phase = movementPhase;
        }
        private void Start() => start = transform.localPosition;
        private void Update() => transform.localPosition = start + travel * (0.5f + 0.5f * Mathf.Sin(Time.time * speed + phase));
        public void ResetMechanism() => transform.localPosition = start;
    }
}
