using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
/// <summary>
/// A Class which will start the game and asks the player to give the park a name.
/// </summary>
public class StartView : MonoBehaviour
{
    public InputField nameInput;
    public Text loading;

    /// <summary>
    /// Start the game and save the typed in park's name
    /// </summary>
    public void GameStart()
    {
        if (nameInput.text.Length != 0 && Regex.Matches(nameInput.text, @"[a-zA-Z0-9]").Count>0)
        {
            ParkName.Name = nameInput.text;
            loading.gameObject.SetActive(true);
            SceneManager.LoadScene("SampleScene");
        }
    }

}
