using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
  
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private GameObject settingUI;

    void Awake()
    {
        // 確保只有一個 VolumeSetting 存在
        VolumeSetting[] existingSettings = FindObjectsOfType<VolumeSetting>();
        if (existingSettings.Length > 1)
        {
            Destroy(this.gameObject); // 如果已經有別的，就刪掉自己
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    

    void Start()
    {

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 50f); // 預設 50%
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 50f);

        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        SetMusicVolume();
        SetSFXVolume();

        // 監聽事件（建議補上）
        musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
      
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            settingUI.SetActive(!settingUI.activeSelf); // 切換開關
        }
    }

    public void SetMusicVolume()
    {
        float sliderValue = musicSlider.value; // 0 ~ 100
        float volume = sliderValue / 100f;     // 0.0 ~ 1.0
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 40f; // 對數轉換為 -80 ~ 0 dB

        mainMixer.SetFloat("MusicVolume", dB);
        musicVolumeText.text = Mathf.RoundToInt(sliderValue).ToString(); // 顯示 0 ~ 100

        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        float sliderValue = sfxSlider.value;
        float volume = sliderValue / 100f;
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 40f;

        mainMixer.SetFloat("SFXVolume", dB);
        sfxVolumeText.text = Mathf.RoundToInt(sliderValue).ToString();

        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }
}