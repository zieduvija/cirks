using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown resolutionDropdown;   // Resolution selection dropdown
    public Slider volumeSlider;               // Volume control slider
    public Toggle fullscreenToggle;           // Fullscreen toggle
    public TMP_Dropdown pitchDropdown;        // Pitch control dropdown
    public Button saveButton;                 // Save button

    [Header("Audio")]
    public AudioMixer audioMixer;             // AudioMixer with exposed parameters

    // An array to hold our custom resolution options.
    private Resolution[] customResolutions;

    void Start()
    {

        // --- Custom Resolution Setup ---
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

        // --- Volume Setup ---
        volumeSlider.value = 0.5f;
        if (volumeSlider.minValue <= 0)
        {
            volumeSlider.minValue = 0.01f;
        }
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // --- Fullscreen Setup ---
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        // --- Pitch Dropdown Setup ---
        List<string> pitchOptions = new List<string> { "Chill Mode", "Normal Mode", "Energetic Mode" };
        pitchDropdown.ClearOptions();
        pitchDropdown.AddOptions(pitchOptions);
        pitchDropdown.onValueChanged.AddListener(SetPitch);

        // --- Load Saved Settings ---
        LoadSettings();

        // --- Save Button Setup ---
        saveButton.onClick.AddListener(SaveSettings);
    }

    public void LoadSettings()
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
        Debug.Log("Volume set to (dB): " + dB);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= customResolutions.Length)
            return;
        Resolution resolution = customResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("Resolution set to: " + resolution.width + " x " + resolution.height);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen set to: " + isFullscreen);
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
        Debug.Log("Pitch set to: " + pitchValue);
        PlayerPrefs.SetInt("pitchIndex", pitchIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings saved!");
        // Determine the previous scene from which settings were loaded.
        // This should be set before loading the settings scene.
        string previousScene = PlayerPrefs.GetString("previousScene", "MainMenue");
        SceneManager.LoadScene(previousScene);
    }
}
