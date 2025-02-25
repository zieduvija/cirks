using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelSettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown pitchDropdown;
    public Button saveButton;
    public AudioMixer audioMixer;
    public GameObject settingsPanel; // Reference to the settings UI panel in Level 1

    private Resolution[] customResolutions;

    void Start()
    {
        customResolutions = new Resolution[3];
        customResolutions[0] = new Resolution { width = 1280, height = 720, refreshRate = 60 };
        customResolutions[1] = new Resolution { width = 1920, height = 1080, refreshRate = 60 };
        customResolutions[2] = new Resolution { width = 2560, height = 1440, refreshRate = 60 };

        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < customResolutions.Length; i++)
        {
            string option = customResolutions[i].width + " x " + customResolutions[i].height;
            resolutionOptions.Add(option);
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        volumeSlider.value = 0.5f;
        if (volumeSlider.minValue <= 0)
            volumeSlider.minValue = 0.01f;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        List<string> pitchOptions = new List<string> { "Chill Mode", "Normal Mode", "Energetic Mode" };
        pitchDropdown.ClearOptions();
        pitchDropdown.AddOptions(pitchOptions);
        pitchDropdown.onValueChanged.AddListener(SetPitch);

        LoadSettings();

        saveButton.onClick.AddListener(SaveSettings);
    }

    private void LoadSettings()
    {
        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 1);
        if (savedResolutionIndex < 0 || savedResolutionIndex >= customResolutions.Length)
            savedResolutionIndex = 1;
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        SetResolution(savedResolutionIndex);

        float savedVolume = PlayerPrefs.GetFloat("volume", 0.5f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        bool savedFullscreen = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        SetFullscreen(savedFullscreen);

        int savedPitchIndex = PlayerPrefs.GetInt("pitchIndex", 1);
        if (savedPitchIndex < 0 || savedPitchIndex >= pitchDropdown.options.Count)
            savedPitchIndex = 1;
        pitchDropdown.value = savedPitchIndex;
        pitchDropdown.RefreshShownValue();
        SetPitch(savedPitchIndex);
    }

    public void SetVolume(float volume)
    {
        if (volume < 0.01f)
            volume = 0.01f;
        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("MasterVolume", dB);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= customResolutions.Length)
            return;
        Resolution resolution = customResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetPitch(int pitchIndex)
    {
        float pitchValue;
        switch (pitchIndex)
        {
            case 0:
                pitchValue = 0.8f;
                break;
            case 1:
                pitchValue = 1.0f;
                break;
            case 2:
                pitchValue = 1.2f;
                break;
            default:
                pitchValue = 1.0f;
                break;
        }
        audioMixer.SetFloat("Pitch", pitchValue);
        PlayerPrefs.SetInt("pitchIndex", pitchIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        settingsPanel.SetActive(false);
    }
}
