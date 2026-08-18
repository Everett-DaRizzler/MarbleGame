using UnityEngine;

namespace MarbleGame
{
    public sealed class RotatingArm : MonoBehaviour, ILevelResettable
    {
        [SerializeField] private float degreesPerSecond = 80f;
        private void Update() => transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.Self);
        public void ResetMechanism() => transform.localRotation = Quaternion.identity;
    }
}
