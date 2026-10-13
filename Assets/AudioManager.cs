using UnityEngine;
using UnityEngine.SceneManagement;

namespace Laliphat
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private const string BGM_VOL_KEY = "BGM_Volume";
        private const string SFX_VOL_KEY = "SFX_Volume";

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip itemCollectClip;
        [SerializeField] private AudioClip obstacleHitClip;

        [Header("BGM Clips")]
        [SerializeField] private AudioClip defaultBgmClip;

        private float bgmVolume = 0.8f;
        private float sfxVolume = 0.8f;

        public float BGMVolume => bgmVolume;
        public float SFXVolume => sfxVolume;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance == null)
            {
                var existing = Object.FindFirstObjectByType<AudioManager>();
                if (existing == null)
                {
                    var prefab = Resources.Load<GameObject>("AudioManager");
                    if (prefab != null)
                    {
                        Instantiate(prefab);
                    }
                    else
                    {
                        GameObject obj = new GameObject("AudioManager");
                        obj.AddComponent<AudioManager>();
                    }
                }
            }
        }

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

            // Load saved volume preferences
            bgmVolume = PlayerPrefs.GetFloat(BGM_VOL_KEY, 0.8f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 0.8f);
            bgmSource.volume = bgmVolume;
            sfxSource.volume = sfxVolume;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            if (defaultBgmClip != null && (bgmSource.clip == null || !bgmSource.isPlaying))
            {
                PlayBGM(defaultBgmClip);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Ensure background music keeps playing across all scenes (Q.10)
            if (bgmSource != null && !bgmSource.isPlaying && bgmSource.clip != null)
            {
                bgmSource.Play();
            }
        }

        public void PlayItemSFX()
        {
            if (sfxSource != null && itemCollectClip != null)
            {
                sfxSource.PlayOneShot(itemCollectClip, sfxVolume);
            }
        }

        public void PlayObstacleSFX()
        {
            if (sfxSource != null && obstacleHitClip != null)
            {
                sfxSource.PlayOneShot(obstacleHitClip, sfxVolume);
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (bgmSource != null && clip != null)
            {
                if (bgmSource.clip == clip && bgmSource.isPlaying) return;
                bgmSource.clip = clip;
                bgmSource.volume = bgmVolume;
                bgmSource.Play();
            }
        }

        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            if (bgmSource != null) bgmSource.volume = bgmVolume;
            PlayerPrefs.SetFloat(BGM_VOL_KEY, bgmVolume);
            PlayerPrefs.Save();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null) sfxSource.volume = sfxVolume;
            PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVolume);
            PlayerPrefs.Save();
        }
    }
}
