using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// A class for buttons that will select what the player wants to build
/// </summary>
public class BuildButtonScript : MonoBehaviour
{
    private Button button;
    private GameViewScript view;
    public string type;
    /// <summary>
    /// Get the GameViewScript class and the button itself. The type variable is assigned in Unity. Finally, add the Select event to the buttons listener.
    /// </summary>
    void Start()
    {
        button = GetComponent<Button>();
        view = GameObject.Find("GameView").GetComponent<GameViewScript>();
        button.onClick.AddListener(Select);
    }
    /// <summary>
    /// Call GameViewScript's TileTypeSelected method with the building/decoration type that the player wants to build.
    /// </summary>
    private void Select()
    {
        view.TileTypeSelected(type);
    }
}
