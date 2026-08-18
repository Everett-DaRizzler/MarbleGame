using UnityEngine;

namespace MarbleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class MarbleAudio : MonoBehaviour
    {
        private AudioSource rollingSource;
        private AudioSource impactSource;
        private Rigidbody body;
        private AudioClip impactClip;
        private float lastImpact;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            rollingSource = gameObject.AddComponent<AudioSource>();
            rollingSource.spatialBlend = 1f;
            rollingSource.loop = true;
            rollingSource.volume = 0.03f;
            rollingSource.clip = CreateTone("marble-roll", 0.7f, 92f, 0.12f);
            rollingSource.Play();

            impactSource = gameObject.AddComponent<AudioSource>();
            impactSource.spatialBlend = 1f;
            impactSource.volume = 0.12f;
            impactClip = CreateTone("marble-impact", 0.08f, 480f, 0.24f);
        }

        private void Update()
        {
            if (body == null) return;
            float speed = body.linearVelocity.magnitude;
            rollingSource.pitch = Mathf.Lerp(0.75f, 1.35f, Mathf.InverseLerp(0f, 14f, speed));
            rollingSource.volume = Mathf.Lerp(0.015f, 0.07f, Mathf.InverseLerp(0.5f, 12f, speed));
        }

        private void OnCollisionEnter(Collision collision)
        {
            float impact = collision.relativeVelocity.magnitude;
            if (impact < 1.4f || Time.time - lastImpact < 0.1f) return;
            lastImpact = Time.time;
            impactSource.pitch = Mathf.Clamp(0.78f + impact * 0.025f, 0.78f, 1.3f);
            impactSource.PlayOneShot(impactClip, Mathf.Clamp01(impact / 18f));
        }

        private static AudioClip CreateTone(string name, float duration, float frequency, float harmonic)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float envelope = 1f - i / (float)samples;
                float t = i / (float)sampleRate;
                data[i] = (Mathf.Sin(2f * Mathf.PI * frequency * t) + harmonic * Mathf.Sin(2f * Mathf.PI * frequency * 1.7f * t)) * envelope * 0.2f;
            }
            clip.SetData(data, 0);
            return clip;
        }
    }
}
