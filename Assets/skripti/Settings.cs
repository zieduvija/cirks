using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
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
        // Define a few fixed resolutions.
        customResolutions = new Resolution[3];
        customResolutions[0] = new Resolution { width = 1280, height = 720, refreshRate = 60 };
        customResolutions[1] = new Resolution { width = 1920, height = 1080, refreshRate = 60 }; // Default (1080p)
        customResolutions[2] = new Resolution { width = 2560, height = 1440, refreshRate = 60 };

        // Populate the resolution dropdown with our fixed options.
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
        // Set the volume slider to 50% on load.
        volumeSlider.value = 0.5f;
        // Ensure the slider's minimum is set to avoid Log10(0). If not, set it.
        if (volumeSlider.minValue <= 0)
        {
            volumeSlider.minValue = 0.01f;
        }
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // --- Fullscreen Setup ---
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        // --- Pitch Dropdown Setup ---
        // Define the pitch options.
        List<string> pitchOptions = new List<string> { "Chill Mode", "Normal Mode", "Energetic Mode" };
        pitchDropdown.ClearOptions();
        pitchDropdown.AddOptions(pitchOptions);
        pitchDropdown.onValueChanged.AddListener(SetPitch);

        // --- Load Saved Settings ---
        LoadSettings();

        // --- Save Button Setup ---
        saveButton.onClick.AddListener(SaveSettings);
    }

    /// <summary>
    /// Loads saved settings from PlayerPrefs, or applies default values.
    /// </summary>
    private void LoadSettings()
    {
        // Load resolution index; default to 1 (1920x1080)
        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 1);
        if (savedResolutionIndex < 0 || savedResolutionIndex >= customResolutions.Length)
            savedResolutionIndex = 1;
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        SetResolution(savedResolutionIndex);

        // Load volume; default to 0.5 (50%)
        float savedVolume = PlayerPrefs.GetFloat("volume", 0.5f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // Load fullscreen; default to current screen state
        bool savedFullscreen = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        SetFullscreen(savedFullscreen);

        // Load pitch index; default to 1 (Normal Mode)
        int savedPitchIndex = PlayerPrefs.GetInt("pitchIndex", 1);
        if(savedPitchIndex < 0 || savedPitchIndex >= pitchDropdown.options.Count)
            savedPitchIndex = 1;
        pitchDropdown.value = savedPitchIndex;
        pitchDropdown.RefreshShownValue();
        SetPitch(savedPitchIndex);
    }

    /// <summary>
    /// Adjusts the game volume based on the slider's value.
    /// Converts the slider's linear value (0.01 to 1) to decibels.
    /// Stores the volume setting in PlayerPrefs (to be saved later).
    /// </summary>
    public void SetVolume(float volume)
    {
        if (volume < 0.01f)
            volume = 0.01f;

        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("MasterVolume", dB);
        Debug.Log("Volume set to (dB): " + dB);

        // Store volume setting (actual write to disk occurs in SaveSettings())
        PlayerPrefs.SetFloat("volume", volume);
    }

    /// <summary>
    /// Sets the screen resolution based on the dropdown selection.
    /// Stores the resolution index in PlayerPrefs.
    /// </summary>
    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= customResolutions.Length)
            return;

        Resolution resolution = customResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("Resolution set to: " + resolution.width + " x " + resolution.height);

        // Store resolution setting.
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    /// <summary>
    /// Toggles fullscreen mode on or off.
    /// Stores the fullscreen setting in PlayerPrefs.
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen set to: " + isFullscreen);

        // Store fullscreen setting.
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    /// <summary>
    /// Adjusts the pitch using the AudioMixer's exposed parameter "Pitch".
    /// Stores the pitch setting in PlayerPrefs.
    /// </summary>
    public void SetPitch(int pitchIndex)
    {
        float pitchValue;
        switch (pitchIndex)
        {
            case 0:
                // Chill Mode: Lower pitch
                pitchValue = 0.8f;
                break;
            case 1:
                // Normal Mode: Default pitch
                pitchValue = 1.0f;
                break;
            case 2:
                // Energetic Mode: Higher pitch
                pitchValue = 1.2f;
                break;
            default:
                pitchValue = 1.0f;
                break;
        }
        audioMixer.SetFloat("Pitch", pitchValue);
        Debug.Log("Pitch set to (AudioMixer parameter): " + pitchValue);

        // Store pitch setting.
        PlayerPrefs.SetInt("pitchIndex", pitchIndex);
    }

    /// <summary>
    /// Saves all settings to disk.
    /// This function is called when the Save button is pressed.
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings saved!");
    }
}
