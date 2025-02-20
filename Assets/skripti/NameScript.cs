using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameScript : MonoBehaviour
{
    // Holds the reference to the TextMeshPro component on the child "NameField".
    private TextMeshPro tmp;

    void Awake()
    {
        // Find the child GameObject "NameField" and get its TextMeshPro component.
        tmp = transform.Find("NameField").gameObject.GetComponent<TextMeshPro>();
        
        if (tmp == null)
        {
            Debug.LogError("TextMeshPro component not found on NameField!");
        }
    }

    // Sets the player's name and assigns a random color.
    public void SetPlayerName(string name)
    {
        if (tmp != null)
        {
            tmp.text = name;
            tmp.color = new Color32(
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                255
            );
        }
    }

    // Added getter method to retrieve the player's name.
    public string GetPlayerName()
    {
        return tmp != null ? tmp.text : "";
    }
}
