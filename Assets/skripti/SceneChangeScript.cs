using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{
    public FadeScript fadeScript;

    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, ""));
    }
    
    // Call this method from your Settings button
    public void OpenSettings()
    {
        StartCoroutine(Delay("settings", -1, ""));
    }

    public IEnumerator Delay(string command, int character, string name)
    {
        if (string.Equals(command, "quit", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeIn(0.1f);
            PlayerPrefs.DeleteAll();

            #if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Application.Quit();
            }
            #else
                Application.Quit();
            #endif
        }
        else if (string.Equals(command, "play", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeIn(0.1f);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else if (string.Equals(command, "settings", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeIn(0.1f);
            // Loads the scene named "Settings" (ensure it is added to Build Settings)
            SceneManager.LoadScene("Settings", LoadSceneMode.Single);
        }
    }
}
