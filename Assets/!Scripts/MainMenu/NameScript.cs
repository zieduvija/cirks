using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameScript : MonoBehaviour
{
    private TextMeshPro tmp;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        tmp = transform.Find("NameField").gameObject.GetComponent<TextMeshPro>();
        if (tmp == null)
        {
            Debug.LogError("TextMeshPro component not found on NameField!");
        }
    }

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

    public Sprite GetPlayerSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();
        return (sr != null) ? sr.sprite : null;
    }


    public string GetPlayerName()
    {
        return tmp != null ? tmp.text : "";
    }
}
