using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D[] cursors;

    void Start()
    {
        DefaultCursor();
    }
    public void DefaultCursor()
    {
        Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.ForceSoftware);
    }
    public void OnButton()
    {
        Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.ForceSoftware);
    }
    public void ClickedButton()
    {
        Cursor.SetCursor(cursors[2], Vector2.zero, CursorMode.ForceSoftware);
    }
    public void OnObject()
    {
        Cursor.SetCursor(cursors[3], Vector2.zero, CursorMode.ForceSoftware);
    }
}
