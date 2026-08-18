using UnityEngine;

namespace MarbleGame
{
    public sealed class OutOfBoundsReset : MonoBehaviour
    {
        [SerializeField] private float minimumHeight = -30f;
        [SerializeField] private float maximumRadius = 64f;
        [SerializeField] private Vector3 machineCenter = new Vector3(0f, 5f, 25f);
        private MarblePlayer player;

        private void Awake()
        {
            player = FindAnyObjectByType<MarblePlayer>();
        }

        private void Update()
        {
            if (player == null || GameDirector.Instance == null || GameDirector.Instance.IsEnding) return;
            Vector3 offset = player.transform.position - machineCenter;
            if (player.transform.position.y < minimumHeight || offset.sqrMagnitude > maximumRadius * maximumRadius)
                GameDirector.Instance.Fail("THE MACHINE LET GO. LEARN THE LINE.");
        }
    }
}
