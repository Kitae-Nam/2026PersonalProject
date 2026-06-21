using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _01_Script.UI.Setting
{
    public class SoundSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        void Start()
        {
            masterSlider.value = PlayerPrefs.GetFloat("Master", 0.8f);
            bgmSlider.value    = PlayerPrefs.GetFloat("BGM", 0.8f);
            sfxSlider.value    = PlayerPrefs.GetFloat("SFX", 0.8f);

            masterSlider.onValueChanged.AddListener(SetMaster);
            bgmSlider.onValueChanged.AddListener(SetBGM);
            sfxSlider.onValueChanged.AddListener(SetSFX);

            SetMaster(masterSlider.value);
            SetBGM(bgmSlider.value);
            SetSFX(sfxSlider.value);
        }

        // 슬라이더 0~1 → 데시벨 변환 (로그 스케일)
        private float ToDb(float v) => Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;

        public void SetMaster(float v)
        {
            mixer.SetFloat("Master", ToDb(v));
            PlayerPrefs.SetFloat("Master", v);
        }
        public void SetBGM(float v)
        {
            mixer.SetFloat("BGM", ToDb(v));
            PlayerPrefs.SetFloat("BGM", v);
        }
        public void SetSFX(float v)
        {
            mixer.SetFloat("SFX", ToDb(v));
            PlayerPrefs.SetFloat("SFX", v);
        }
    }
}