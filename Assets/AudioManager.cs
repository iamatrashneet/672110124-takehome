using UnityEngine;

namespace Laliphat
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip itemCollectClip;
        [SerializeField] private AudioClip obstacleHitClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }

        public void PlayItemSFX()
        {
            if (sfxSource != null && itemCollectClip != null)
            {
                sfxSource.PlayOneShot(itemCollectClip);
            }
        }

        public void PlayObstacleSFX()
        {
            if (sfxSource != null && obstacleHitClip != null)
            {
                sfxSource.PlayOneShot(obstacleHitClip);
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (bgmSource != null && clip != null)
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            }
        }

        public void SetBGMVolume(float volume)
        {
            if (bgmSource != null) bgmSource.volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            if (sfxSource != null) sfxSource.volume = volume;
        }
    }
}
