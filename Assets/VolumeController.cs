using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Laliphat
{
    public class VolumeController : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Text Displays")]
        [SerializeField] private TextMeshProUGUI volumeText;

        private float bgm = 0.8f;
        private float sfx = 0.8f;

        private void Start()
        {
            if (AudioManager.Instance != null)
            {
                bgm = AudioManager.Instance.BGMVolume;
                sfx = AudioManager.Instance.SFXVolume;
            }
            else
            {
                bgm = PlayerPrefs.GetFloat("BGM_Volume", 0.8f);
                sfx = PlayerPrefs.GetFloat("SFX_Volume", 0.8f);
            }

            if (volumeText == null)
            {
                // Auto-detect the volume text in OptionsPanel
                var tmps = GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var t in tmps)
                {
                    if (t.text.Contains("BGM Volume") || t.text.Contains("Volume"))
                    {
                        volumeText = t;
                        break;
                    }
                }
            }

            if (bgmSlider != null)
            {
                bgmSlider.value = bgm;
                bgmSlider.onValueChanged.AddListener(SetBGM);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = sfx;
                sfxSlider.onValueChanged.AddListener(SetSFX);
            }

            UpdateDisplay();
        }

        private void Update()
        {
            // Keyboard controls for convenient volume adjustment in Options
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeBGM(-0.1f);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeBGM(0.1f);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChangeSFX(-0.1f);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ChangeSFX(0.1f);
            }
        }

        public void SetBGM(float val)
        {
            bgm = Mathf.Clamp01(val);
            if (AudioManager.Instance != null) AudioManager.Instance.SetBGMVolume(bgm);
            UpdateDisplay();
        }

        public void SetSFX(float val)
        {
            sfx = Mathf.Clamp01(val);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(sfx);
                AudioManager.Instance.PlayItemSFX(); // Play audio sample when adjusting SFX!
            }
            UpdateDisplay();
        }

        public void ChangeBGM(float delta)
        {
            SetBGM(bgm + delta);
            if (bgmSlider != null) bgmSlider.value = bgm;
        }

        public void ChangeSFX(float delta)
        {
            SetSFX(sfx + delta);
            if (sfxSlider != null) sfxSlider.value = sfx;
        }

        private void UpdateDisplay()
        {
            if (volumeText != null)
            {
                int bgmBars = Mathf.RoundToInt(bgm * 20f);
                string bgmBarStr = new string('=', bgmBars).PadRight(20, ' ');

                int sfxBars = Mathf.RoundToInt(sfx * 20f);
                string sfxBarStr = new string('=', sfxBars).PadRight(20, ' ');

                volumeText.text = $"BGM Volume [{bgmBarStr}] {Mathf.RoundToInt(bgm * 100)}%\n\n    SFX Volume [{sfxBarStr}] {Mathf.RoundToInt(sfx * 100)}%";
            }
        }
    }
}
