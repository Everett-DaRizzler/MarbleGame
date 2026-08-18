using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MarbleGame
{
    public sealed class GameDirector : MonoBehaviour
    {
        public static GameDirector Instance { get; private set; }

        [SerializeField] private MarblePlayer player;
        [SerializeField] private CanvasGroup resultOverlay;
        [SerializeField] private TMPro.TMP_Text resultText;
        [SerializeField] private TMPro.TMP_Text sectionText;
        [SerializeField] private TMPro.TMP_Text telemetryText;

        private bool ending;
        private int attempts;
        private Coroutine restartRoutine;
        private ILevelResettable[] resettableComponents = Array.Empty<ILevelResettable>();
        private float nextTelemetryUpdate;

        public bool IsEnding => ending;
        public int Attempts => attempts;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            if (player == null) player = FindAnyObjectByType<MarblePlayer>();
            CacheResettableComponents();
            SetOverlay(false);
            SetSection("THE GLASS RUN  /  INTAKE");
            UpdateTelemetry();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) RestartLevel();
            if (Time.unscaledTime >= nextTelemetryUpdate)
            {
                nextTelemetryUpdate = Time.unscaledTime + 0.1f;
                UpdateTelemetry();
            }
        }

        public void Fail(string message)
        {
            if (ending) return;
            ending = true;
            attempts++;
            player?.SetInputEnabled(false);
            SetResult(message + "\n\nRESETTING  •  R");
            if (restartRoutine != null) StopCoroutine(restartRoutine);
            restartRoutine = StartCoroutine(FastRestartRoutine());
        }

        private IEnumerator FastRestartRoutine()
        {
            yield return new WaitForSecondsRealtime(0.32f);
            RestartLevel();
        }

        public void Complete()
        {
            if (ending) return;
            ending = true;
            player?.SetInputEnabled(false);
            SetResult("THE GLASS RUN  /  COMPLETE\n\nYou learned the machine.\nR  RUN IT AGAIN");
        }

        public void RestartLevel()
        {
            if (restartRoutine != null)
            {
                StopCoroutine(restartRoutine);
                restartRoutine = null;
            }
            foreach (ILevelResettable resettable in resettableComponents) resettable.ResetMechanism();
            player ??= FindAnyObjectByType<MarblePlayer>();
            player?.ResetToSpawn();
            ending = false;
            SetOverlay(false);
            UpdateTelemetry();
        }

        private void CacheResettableComponents()
        {
            var components = FindObjectsByType<MonoBehaviour>();
            var cached = new System.Collections.Generic.List<ILevelResettable>();
            foreach (MonoBehaviour component in components)
                if (component is ILevelResettable resettable) cached.Add(resettable);
            resettableComponents = cached.ToArray();
        }

        public void SetSection(string label)
        {
            if (sectionText != null) sectionText.text = label;
        }

        private void SetResult(string message)
        {
            if (resultText != null) resultText.text = message;
            SetOverlay(true);
        }

        private void SetOverlay(bool visible)
        {
            if (resultOverlay == null) return;
            resultOverlay.alpha = visible ? 1f : 0f;
            resultOverlay.blocksRaycasts = visible;
            resultOverlay.interactable = visible;
        }

        private void UpdateTelemetry()
        {
            if (telemetryText == null || player == null) return;
            telemetryText.SetText("SPEED  {0:0.0} m/s\nATTEMPTS  {1:00}", player.Speed, attempts);
        }
    }

    public interface ILevelResettable
    {
        void ResetMechanism();
    }
}
