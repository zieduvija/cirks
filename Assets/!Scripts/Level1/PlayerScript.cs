using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject spawnPoint;
    int[] otherPlayers;
    int index;

    private const string textFileName = "playerNames";

    // Start is called before the first frame update
    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex], spawnPoint.transform.position, Quaternion.identity);
        mainCharacter.GetComponent<NameScript>().SetPlayerName(PlayerPrefs.GetString("PlayerName"));

        otherPlayers = new int[PlayerPrefs.GetInt("PlayerCount")];
        string[] nameArray = ReadLinesFromFile(textFileName);

        for (int i = 0; i < otherPlayers.Length - 1; i++)
        {
            spawnPoint.transform.position += new Vector3(0.2f, 0, 0.08f);
            // Use playerPrefabs (not PlayerPrefabs) and adjust the range to be proper.
            index = Random.Range(0, playerPrefabs.Length);
            GameObject character = Instantiate(playerPrefabs[index], spawnPoint.transform.position, Quaternion.identity);
            // Use Random.Range (not Random.range) and adjust the range for array indexing.
            character.GetComponent<NameScript>().SetPlayerName(nameArray[Random.Range(0, nameArray.Length-1)]);
        }
    }

    string[] ReadLinesFromFile(string fName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fName);
        if (textAsset != null)
            return textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        else
        {
            Debug.LogError("File not found: " + fName);
            return new string[0];
        }
    }
}
