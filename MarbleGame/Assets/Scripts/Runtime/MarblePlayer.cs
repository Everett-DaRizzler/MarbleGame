using UnityEngine;
using UnityEngine.InputSystem;

namespace MarbleGame
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public sealed class MarblePlayer : MonoBehaviour
    {
        [Header("Physical steering")]
        [SerializeField] private float lateralSteeringForce = 18f;
        [SerializeField] private float airSteeringMultiplier = 0.22f;
        [SerializeField] private float rollingTorque = 16f;
        [SerializeField] private float groundProbeDistance = 1.2f;
        [SerializeField] private Transform cameraTransform;

        private Rigidbody body;
        private SphereCollider sphere;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Vector3 travelDirection = Vector3.forward;
        private bool inputEnabled = true;

        public Rigidbody Body => body;
        public Vector3 TravelDirection => travelDirection;
        public bool IsGrounded { get; private set; }
        public float Speed => body == null ? 0f : body.linearVelocity.magnitude;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            sphere = GetComponent<SphereCollider>();
            body.mass = 1.1f;
            body.linearDamping = 0.04f;
            body.angularDamping = 0.08f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.useGravity = true;
            body.WakeUp();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            if (body == null) return;

            Vector3 velocity = body.linearVelocity;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            if (horizontalVelocity.sqrMagnitude > 0.18f)
                travelDirection = horizontalVelocity.normalized;
            else
            {
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
                if (forward.sqrMagnitude > 0.01f) travelDirection = forward.normalized;
            }

            IsGrounded = Physics.SphereCast(transform.position, sphere.radius * 0.88f, Vector3.down,
                out _, groundProbeDistance, ~0, QueryTriggerInteraction.Ignore);
            if (!inputEnabled) return;

            float steer = ReadSteerInput();
            if (Mathf.Abs(steer) < 0.01f) return;

            // No force is applied along travelDirection. Gravity and the track own travel.
            Vector3 lateral = Vector3.Cross(Vector3.up, travelDirection).normalized;
            float control = IsGrounded ? 1f : airSteeringMultiplier;
            body.AddForce(lateral * (steer * lateralSteeringForce * control), ForceMode.Acceleration);
            body.AddTorque(Vector3.Cross(lateral, Vector3.up) * (steer * rollingTorque * control), ForceMode.Acceleration);
        }

        private float ReadSteerInput()
        {
            float value = 0f;
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.leftArrowKey.isPressed) value -= 1f;
                if (keyboard.rightArrowKey.isPressed) value += 1f;
            }

            return Mathf.Clamp(value, -1f, 1f);
        }

        public void SetSpawn(Transform marker)
        {
            if (marker == null) return;
            spawnPosition = marker.position;
            spawnRotation = marker.rotation;
            travelDirection = Vector3.ProjectOnPlane(marker.forward, Vector3.up).normalized;
        }

        public void ResetToSpawn()
        {
            inputEnabled = true;
            body.isKinematic = false;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            travelDirection = Vector3.ProjectOnPlane(spawnRotation * Vector3.forward, Vector3.up).normalized;
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            if (!enabled && body != null)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
        }
    }
}
