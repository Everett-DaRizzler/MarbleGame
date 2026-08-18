using UnityEngine;

namespace MarbleGame
{
    public sealed class MachineAmbience : MonoBehaviour
    {
        [SerializeField] private float volume = 0.025f;

        private void Awake()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 0.25f;
            source.loop = true;
            source.volume = volume;
            source.clip = CreateAmbience();
            source.Play();
        }

        private static AudioClip CreateAmbience()
        {
            const int sampleRate = 44100;
            const float duration = 4f;
            int samples = Mathf.RoundToInt(sampleRate * duration);
            AudioClip clip = AudioClip.Create("machine-ambience", samples, 1, sampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float swell = 0.5f + 0.5f * Mathf.Sin(t * Mathf.PI * 0.5f);
                data[i] = (Mathf.Sin(t * 2f * Mathf.PI * 54f) * 0.45f + Mathf.Sin(t * 2f * Mathf.PI * 81f) * 0.2f) * swell * 0.16f;
            }
            clip.SetData(data, 0);
            return clip;
        }
    }
}
