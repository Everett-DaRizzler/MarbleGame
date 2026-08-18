using UnityEngine;

namespace MarbleGame
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private MarblePlayer target;
        [SerializeField] private float distance = 9.5f;
        [SerializeField] private float height = 4.8f;
        [SerializeField] private float lookAhead = 3.2f;
        [SerializeField] private float followSharpness = 10f;
        [SerializeField] private float rotationSharpness = 12f;
        // The generated first-playable course is intentionally wide and has no camera-obstruction layer.
        // Keep this disabled by default; enabling it later must use a dedicated environment mask.
        [SerializeField] private LayerMask collisionMask = 0;
        private readonly RaycastHit[] collisionHits = new RaycastHit[16];

        private void Start()
        {
            if (target == null) target = FindAnyObjectByType<MarblePlayer>();
            Snap();
        }

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 direction = Vector3.ProjectOnPlane(target.TravelDirection, Vector3.up);
            if (direction.sqrMagnitude < 0.01f) direction = Vector3.forward;
            direction.Normalize();
            Vector3 focus = target.transform.position + Vector3.up * 0.65f + direction * lookAhead;
            Vector3 desired = target.transform.position - direction * distance + Vector3.up * height;

            Vector3 fromTarget = desired - target.transform.position;
            if (collisionMask.value != 0)
            {
                Vector3 castOrigin = target.transform.position + Vector3.up * 0.4f;
                int hitCount = Physics.SphereCastNonAlloc(castOrigin, 0.35f, fromTarget.normalized, collisionHits,
                    fromTarget.magnitude, collisionMask, QueryTriggerInteraction.Ignore);
                float nearestDistance = float.PositiveInfinity;
                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit hit = collisionHits[i];
                    if (hit.collider == null || hit.rigidbody == target.Body || hit.collider.transform.IsChildOf(target.transform)) continue;
                    if (hit.distance < nearestDistance)
                    {
                        nearestDistance = hit.distance;
                        desired = hit.point - fromTarget.normalized * 0.35f;
                    }
                }
            }

            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            Quaternion desiredRotation = Quaternion.LookRotation(focus - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime));
        }

        private void Snap()
        {
            if (target == null) return;
            Vector3 direction = Vector3.ProjectOnPlane(target.TravelDirection, Vector3.up).normalized;
            if (direction.sqrMagnitude < 0.01f) direction = Vector3.forward;
            transform.position = target.transform.position - direction * distance + Vector3.up * height;
            transform.LookAt(target.transform.position + Vector3.up * 0.65f + direction * lookAhead);
        }
    }
}
