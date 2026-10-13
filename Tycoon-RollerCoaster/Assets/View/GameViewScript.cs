using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;
/// <summary>
/// View class is responsible for the game's display and it handles most of the buttons that can be pressed.
/// </summary>
public class GameViewScript : MonoBehaviour
{
    //Start és Update nem kell, majd a Controller vezérli

    //controllertől kapja majd
    private int _boardSize;
    private GameModel _gameModel;

    //Unity-ben odahúzzuk neki a Tilemap-et
    public Tilemap tileMap;
    public Tilemap highlight;
    public Tilemap visitorMap;
    public Tilemap buildingMap;

    //Tilemap-hez a textúrák (Tile-ok), Unityben adjok meg őket
    public TileBase dirtTile;
    public TileBase rollercoasterTile;
    public TileBase merryGoRoundTile;
    public TileBase ferrisWheelTile;
    public TileBase buffetTile;
    public TileBase candyTile;
    public TileBase restaurantTile;
    public TileBase drinksTile;
    public TileBase toiletTile;
    public TileBase treeTile;
    public TileBase bushTile;
    public TileBase grassTile;
    public TileBase pathTile;
    public TileBase illegalTile;
    public TileBase greenTransparentTile_1;
    public TileBase greenTransparentTile_3;
    public TileBase redTransparentTile_1;
    public TileBase redTransparentTile_3;

    public TileBase visitorTile;
    public TileBase visitorTile2;
    public TileBase visitorTile3;
    public TileBase visitorTile4;
    public TileBase visitorTile5;
    public TileBase visitorTile6;

    public TileBase constructionTile;
    public TileBase closedTile;
    public TileBase balloonTile;

    private bool pathSelected;
    private BuildingType selectedBuilding;
    private DecorationType selectedDecoration;
    public bool PathSelected { get { return pathSelected; } }
    public BuildingType SelectedBuilding { get { return selectedBuilding; } }
    public DecorationType SelectedDecoration { get { return selectedDecoration; } }

    //A nézet fő nyomógombjai és a menük
    public Button buildButton;
    public Button decorationButton;
    public Button destructionButton;
    public Button openParkButton;
    public Button ticketButton;
    public Button changeActiveButton;
    public Button gChangeActiveButton;
    public GameObject buildingMenu;
    public GameObject decorationMenu;
    public GameObject badPlacePopUp;
    public GameObject badPricePopUp;
    public GameObject badPercentagePopUp;
    public GameObject informationPanel;
    public GameObject gInformationPanel;
    public GameObject visitorInfoPanel;
    public GameObject nameOfPark;
    public GameObject ticketPanel;
    public GameObject avaragePanel;
    public GameObject toolTipPanel;
    public InputField tileNameInput;
    public InputField tilePriceInput;
    public InputField gTileNameInput;
    public InputField gTilePriceInput;
    public InputField gTileMinimumInput;
    public InputField ticketPriceInput;
    public TextMeshProUGUI moneyStatus;
    public TextMeshProUGUI visitorStatus;
    public TextMeshProUGUI nameOfParkText;
    public TextMeshProUGUI changeActiveButtontext;
    public TextMeshProUGUI gChangeActiveButtontext;
    public TextMeshProUGUI visitorHappines;
    public TextMeshProUGUI visitorHunger;
    public TextMeshProUGUI visitorThirst;
    public TextMeshProUGUI visitorWC;
    public TextMeshProUGUI visitorMoney;
    public TextMeshProUGUI avaregeHappinesText;
    public TextMeshProUGUI gameSpeedText;

    public event EventHandler ChangeToBuildMode;
    public event EventHandler ChangeToDestructionMode;
    public event EventHandler MenuOpened;
    public event EventHandler MenuClosed;
    public event EventHandler InfoOpened;
    public event EventHandler InfoClosed;
    public event EventHandler BuildingSelected;
    public event EventHandler DecorationSelected;
    public event EventHandler PathSelectedEvent;
    public event EventHandler SaveInformation;
    public event EventHandler GSaveInformation;
    public event EventHandler OpenPark;
    public event EventHandler ChangeActive;
    public event EventHandler SlowerTime;
    public event EventHandler FasterTime;

    //A játék audió elemei
    private AudioSource audioSource;
    public AudioClip buttonPressed;
    public AudioClip buildSound;
    public AudioClip destroySound;
    public AudioClip openSound;
    /// <summary>
    /// Setup the board at the start of the game and assign the values that are given from the controller class.
    /// </summary>
    /// <param name="gameModel">The model class.</param>
    /// <param name="boardSize">The size of the board</param>
    /// <param name="mode">Game mode. Can be: Idle,Building,Destruction. At start, its Idle.</param>
    public void SetupGrid(GameModel gameModel, int boardSize, GameControllerScript.Mode mode)
    {
        _boardSize = boardSize;

        _gameModel = gameModel;

        audioSource = GetComponent<AudioSource>();

        pathSelected = false;
        selectedBuilding = BuildingType.Illegal;
        selectedDecoration = DecorationType.Illegal;

        tileMap.ClearAllTiles();

        UpdateTileGrid(mode);
    }
    /// <summary>
    /// Update the display of the board using the model.
    /// </summary>
    /// <param name="mode">Game mode. Can be: Idle,Building,Destruction.</param>
    public void UpdateTileGrid(GameControllerScript.Mode mode)
    {
        if (_boardSize <= 0 || _gameModel == null)
            return;

        pathSelected = false;
        selectedBuilding = BuildingType.Illegal;
        selectedDecoration = DecorationType.Illegal;
        highlight.ClearAllTiles();

        for (int i = 0; i < _boardSize; i++)
        {
            for (int j = 0; j < _boardSize; j++)
            {
                GameTile gameTile = _gameModel.getGameTile(i, j);
                TileObject tileObject = _gameModel.getTileObject(i, j);

                Vector3Int index = new Vector3Int(i, j, 0);


                if (tileObject == null)
                {
                    tileMap.SetTile(index, dirtTile);
                }
                else if (tileObject.TileType == TileType.Path && gameTile.ShowTexture)
                {
                    tileMap.SetTile(index, pathTile);
                }
                else if (tileObject.TileType == TileType.Building && gameTile.ShowTexture)
                {
                    switch(((BuildingTileObject)tileObject).BuildingType)
                    {
                        case BuildingType.Rollercoaster:
                            tileMap.SetTile(index, rollercoasterTile);
                            break;
                        case BuildingType.MerryGoRound:
                            tileMap.SetTile(index, merryGoRoundTile);
                            break;
                        case BuildingType.FerrisWheel:
                            tileMap.SetTile(index, ferrisWheelTile);
                            break;
                        case BuildingType.Buffet:
                            tileMap.SetTile(index, buffetTile);
                            break;
                        case BuildingType.Candy:
                            tileMap.SetTile(index, candyTile);
                            break;
                        case BuildingType.Restaurant:
                            tileMap.SetTile(index, restaurantTile);
                            break;
                        case BuildingType.Drinks:
                            tileMap.SetTile(index, drinksTile);
                            break;
                        case BuildingType.Toilet:
                            tileMap.SetTile(index, toiletTile);
                            break;
                    }
                }
                else if (tileObject.TileType == TileType.Decoration && gameTile.ShowTexture)
                {
                    switch (((DecorationTileObject)tileObject).DecorationType)
                    {
                        case DecorationType.Grass:
                            tileMap.SetTile(index, grassTile);
                            break;
                        case DecorationType.Tree:
                            tileMap.SetTile(index, treeTile);
                            break;
                        case DecorationType.Bush:
                            tileMap.SetTile(index, bushTile);
                            break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Display a semi-transparent square at the cursor's position when the player wants to build. It is red if the player can't build. Otherwise, it is green. The highlight is as big as the building/decoration that we want to build.
    /// </summary>
    /// <param name="x">First cordinate of the tile</param>
    /// <param name="y">Second cordinate of the tile</param>
    /// <param name="size">Size of the element that the player wants to build.</param>
    /// <param name="canBuild">Boolean that is true if the player can build at the given place.</param>
    public void UpdateHighlightGrid(int x, int y, Coords size, bool canBuild)
    {
        highlight.ClearAllTiles();

        if(size.x == 3 && size.y == 3)
        {
            if(canBuild)
            {
                highlight.SetTile(new Vector3Int(x, y, 0), greenTransparentTile_3);
            }
            else
            {
                highlight.SetTile(new Vector3Int(x, y, 0), redTransparentTile_3);
            }
        }
        else
        {
            if (canBuild)
            {
                highlight.SetTile(new Vector3Int(x, y, 0), greenTransparentTile_1);
            }
            else
            {
                highlight.SetTile(new Vector3Int(x, y, 0), redTransparentTile_1);
            }
        }
    }
    /// <summary>
    /// Display the visitors using the model. Can display 1-6 visitors.
    /// </summary>
    public void UpdateVisitorGrid()
    {
        //ez lehet laggos és majd ki kell cserélni jobbra
        for(int i = 0; i < _boardSize; i++)
        {
            for (int j = 0; j < _boardSize; j++)
            {
                int count = _gameModel.getVisitorCount(i, j);
                if(count > 0)
                {
                    switch (count)
                    {
                        case 1:
                            visitorMap.SetTile(new Vector3Int(i, j, 0), visitorTile);
                            break;
                        case 2:
                            visitorMap.SetTile(new Vector3Int(i, j, 0), visitorTile2);
                            break;
                        case 3:
                            visitorMap.SetTile(new Vector3Int(i, j, 0), visitorTile3);
                            break;
                        case 4:
                            visitorMap.SetTile(new Vector3Int(i, j, 0), visitorTile4);
                            break;
                        case 5:
                            visitorMap.SetTile(new Vector3Int(i, j, 0), visitorTile5);
                            break;
                        case 6:
                        default:
                            visitorMap.SetTile(new Vector3Int(i, j, 0), visitorTile6);
                            break;
                    }
                    
                }
                else
                {
                    visitorMap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }
        }
    }
    /// <summary>
    /// Display the status of the buildings wheter they are in building status, closed or the round is still going.
    /// </summary>
    public void UpdateBuildingGrid()
    {
        foreach(BuildingTileObject buildingTileObject in _gameModel.BuildingTileObjects)
        {
            Coords pos = buildingTileObject.MiddlePos;

            if (buildingTileObject.CState == State.Building)
            {
                buildingMap.SetTile(new Vector3Int(pos.x, pos.y, 0), constructionTile);
            }
            else if (buildingTileObject.Provides == Provides.Fun)
            {
                if (!buildingTileObject.IsActive)
                {
                    buildingMap.SetTile(new Vector3Int(pos.x, pos.y, 0), closedTile);
                }
                else if(buildingTileObject.CState == State.Going)
                {
                    buildingMap.SetTile(new Vector3Int(pos.x, pos.y, 0), balloonTile);
                }
                else
                {
                    buildingMap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
                }
            }
            else
            {
                if(!buildingTileObject.IsActive)
                {
                    buildingMap.SetTile(new Vector3Int(pos.x, pos.y, 0), closedTile);
                }
                else
                {
                    buildingMap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
                }
            }
        }
    }
    /// <summary>
    /// Called after the building button is pressed. Open the building menu and close everything else or close the menu if it was opened.
    /// </summary>
    public void BuildingMenuSelected()
    {
        audioSource.PlayOneShot(buttonPressed, 1.0f);
        if (buildingMenu.activeSelf)
        {
            CloseEverything();
        }
        else
        {
            CloseEverything();

            buildingMenu.SetActive(true);
            OnMenuOpened();
        }
    }
    /// <summary>
    /// Called after the decoration button is pressed. Open the decoration menu and close everything else or close the menu if it was opened.
    /// </summary>
    public void DecorationgMenuSelected()
    {
        audioSource.PlayOneShot(buttonPressed, 1.0f);
        if (decorationMenu.activeSelf)
        {
            CloseEverything();
        }
        else
        {
            CloseEverything();

            decorationMenu.SetActive(true);
            OnMenuOpened();
        }
    }
    /// <summary>
    /// Called after the destruction button is pressed. Change the gamemode to destruction.
    /// </summary>
    public void DestructionSelected()
    {
        audioSource.PlayOneShot(buttonPressed, 1.0f);
        pathSelected = false;
        selectedBuilding = BuildingType.Illegal;
        selectedDecoration = DecorationType.Illegal;
        buildingMenu.SetActive(false);
        decorationMenu.SetActive(false);
        CloseEverything();

        OnChangeToDestructionMode();
    }
    /// <summary>
    /// Called after the park opening button is pressed. Hide the button and send a signal to the controller to open the park so the visitors can come.
    /// </summary>
    public void OpenParkSelected()
    {
        openParkButton.gameObject.SetActive(false);
        audioSource.PlayOneShot(openSound, 1.0f);
        OnOpenPark();
    }
    /// <summary>
    /// Called after the player selected what he/she wants to build. The game is changed to build mode and store what type of Building the player wants to build.
    /// </summary>
    public void TileTypeSelected(string type)
    {
        //ha kiválasztottuk mit akarunk építeni, akkor ez hívódik meg, ahol a type az épület/növény
        audioSource.PlayOneShot(buttonPressed, 1.0f);
        CloseEverything();
        OnChangeToBuildMode();
        switch(type)
        {
            case "RollerCoaster":
                selectedBuilding = BuildingType.Rollercoaster;
                OnBuildingSelected();
                break;
            case "MarryGoRound":
                selectedBuilding = BuildingType.MerryGoRound;
                OnBuildingSelected();
                break;
            case "FerrisWheel":
                selectedBuilding = BuildingType.FerrisWheel;
                OnBuildingSelected();
                break;
            case "Buffet":
                selectedBuilding = BuildingType.Buffet;
                OnBuildingSelected();
                break;
            case "Candy":
                selectedBuilding = BuildingType.Candy;
                OnBuildingSelected();
                break;
            case "Restaurant":
                selectedBuilding = BuildingType.Restaurant;
                OnBuildingSelected();
                break;
            case "Drinks":
                selectedBuilding = BuildingType.Drinks;
                OnBuildingSelected();
                break;
            case "Toilet":
                selectedBuilding = BuildingType.Toilet;
                OnBuildingSelected();
                break;
            case "Path":
                pathSelected = true;
                OnPathSelected();
                break;
            case "Tree":
                selectedDecoration = DecorationType.Tree;
                OnDecorationSelected();
                break;
            case "Bush":
                selectedDecoration = DecorationType.Bush;
                OnDecorationSelected();
                break;
            case "Grass":
                selectedDecoration = DecorationType.Grass;
                OnDecorationSelected();
                break;
        }
    }
    /// <summary>
    /// Display the informations of the given building. Give default values if they have not been set.
    /// </summary>
    /// <param name="selected">The selected building</param>
    public void showInformation(BuildingTileObject selected)
    {
        buildingMenu.SetActive(false);
        decorationMenu.SetActive(false);
        OnMenuOpened();
        OnInfoOpened();
        if (selected.Provides == Provides.Fun)
        {
            gInformationPanel.SetActive(true);
            if (selected.CState == State.Building) { gChangeActiveButton.enabled = false; gChangeActiveButtontext.text = "Épül"; gChangeActiveButtontext.color = Color.gray; }
            else
            {
                gChangeActiveButton.enabled = true;
                gChangeActiveButtontext.color = Color.black;
                if (selected.IsActive)
                {
                    gChangeActiveButtontext.text = "Leállás";
                }
                else
                {
                    gChangeActiveButtontext.text = "Kinyitás";
                }
            }
            if (selected.Name != null)
            {
                gTileNameInput.text = selected.Name;
            }
            else
            {
                switch (selected.BuildingType)
                {
                    case BuildingType.Rollercoaster:
                        gTileNameInput.text = "Hullámvasút";
                        break;
                    case BuildingType.MerryGoRound:
                        gTileNameInput.text = "Körhinta";
                        break;
                    case BuildingType.FerrisWheel:
                        gTileNameInput.text = "Óriáskerék";
                        break;
                    default:
                        break;
                }
            }
            gTilePriceInput.text = selected.Ticket.ToString();
            gTileMinimumInput.text = (System.Math.Round(selected.Minimum / selected.Maximum * 100)).ToString();
        }
        else
        {
            informationPanel.SetActive(true);
            if (selected.CState == State.Building) { changeActiveButton.enabled = false; changeActiveButtontext.text = "Épül"; changeActiveButtontext.color = Color.gray; }
            else
            {
                changeActiveButton.enabled = true;
                changeActiveButtontext.color = Color.black;
                if (selected.IsActive)
                {
                    changeActiveButtontext.text = "Leállás";
                }
                else
                {
                    changeActiveButtontext.text = "Kinyitás";
                }
            }
            if (selected.Name != null)
            {
                tileNameInput.text = selected.Name;
            }
            else
            {
                switch (selected.BuildingType)
                {
                    case BuildingType.Buffet:
                        tileNameInput.text = "Büfé";
                        break;
                    case BuildingType.Candy:
                        tileNameInput.text = "Édességbolt";
                        break;
                    case BuildingType.Restaurant:
                        tileNameInput.text = "Étterem";
                        break;
                    case BuildingType.Drinks:
                        tileNameInput.text = "Ital Bolt";
                        break;
                    case BuildingType.Toilet:
                        tileNameInput.text = "WC";
                        break;
                    default:
                        break;
                }
            }
            tilePriceInput.text = selected.Ticket.ToString();
        }
    }
    /// <summary>
    /// Change the destruction buttons text.
    /// </summary>
    /// <param name="x">The new text of the destruction button</param>
    public void changeDestructionButtonText(string x)
    {
        TextMeshProUGUI helper = destructionButton.GetComponentInChildren<TextMeshProUGUI>();
        helper.text = x;

    }
    /// <summary>
    /// Display a warning with a message that the player wanted to build at a wrong location or he/she doesn't have enough money.
    /// </summary>
    public void badLocation()
    {
        CloseEverything();
        badPlacePopUp.SetActive(true);
        OnMenuOpened();
    }
    /// <summary>
    /// Display a warning with a message that the player wrote a bad number as price.
    /// </summary>
    public void badPrice()
    {
        CloseEverything();
        badPricePopUp.SetActive(true);
        OnMenuOpened();
    }
    /// <summary>
    /// Display a warning with a message that the player wrote a bad number.
    /// </summary>
    public void badPercentage()
    {
        CloseEverything();
        badPercentagePopUp.SetActive(true);
        OnMenuOpened();
    }
    /// <summary>
     /// Close the bad price pop up.
     /// </summary>
    public void closeBadPrice()
    {
        badPricePopUp.SetActive(false);
        badPercentagePopUp.SetActive(false);
        CloseEverything();
        OnMenuClosed();
    }
    /// <summary>
    /// Close the bad place pop up.
    /// </summary>
    public void closePopUp()
    {
        badPlacePopUp.SetActive(false);
        CloseEverything();
        OnMenuClosed();
    }
    /// <summary>
    /// Close the information panel.
    /// </summary>
    public void closeInformation()
    {
        informationPanel.SetActive(false);
        gInformationPanel.SetActive(false);
        OnMenuClosed();
        OnInfoClosed();
    }
    /// <summary>
    /// Close every opened pop up or panel.
    /// </summary>
    public void CloseEverything()
    {
        highlight.ClearAllTiles();
        pathSelected = false;
        selectedBuilding = BuildingType.Illegal;
        selectedDecoration = DecorationType.Illegal;
        buildingMenu.SetActive(false);
        decorationMenu.SetActive(false);
        informationPanel.SetActive(false);
        badPlacePopUp.SetActive(false);
        badPricePopUp.SetActive(false);
        badPercentagePopUp.SetActive(false);
        gInformationPanel.SetActive(false);
        visitorInfoPanel.SetActive(false);
        toolTipPanel.SetActive(false);
        OnMenuClosed();
        OnInfoClosed();
    }
    /// <summary>
    /// Paly build sound.
    /// </summary>
    public void playBuildSound()
    {
        audioSource.PlayOneShot(buildSound, 1.0f);
    }
    /// <summary>
    /// Paly destroy sound.
    /// </summary>
    public void playDestroySound()
    {
        audioSource.PlayOneShot(destroySound, 1.0f);
    }
    /// <summary>
    /// Change the text of money status on screen.
    /// </summary>
    /// <param name="amount">The amount of money that the player has and will be set in the status.</param>
    public void setMoney(int amount)
    {
        moneyStatus.text = "Pénz: " + amount;
    }
    /// <summary>
    /// Change the text of visitor count on screen.
    /// </summary>
    /// <param name="amount">The amount of visitors in the park and will be set on the screen.</param>
    public void setVisitors(int amout)
    {
        visitorStatus.text = "Látogatók: " + amout;
    }
    /// <summary>
    /// Set the name of the park. It is not visible if the name is null.
    /// </summary>
    /// <param name="name">The name of the park</param>
    public void setParkName(string name)
    {
        if (name == null)
        {
            nameOfPark.SetActive(false);
        }
        else
        {
            nameOfParkText.text = name;
        }
    }
    /// <summary>
    /// Call the OnSaveInformation method to save the changed data.
    /// </summary>
    public void saveInformation()
    {
        CloseEverything();
        OnSaveInformation();
    }
    /// <summary>
    /// Call the OnGSaveInformation method to save the changed data.
    /// </summary>
    public void gSaveInformation()
    {
        CloseEverything();
        OnGSaveInformation();
    }
    /// <summary>
    /// Call the OnChangeActive() method to change the building'status.
    /// </summary>
    public void changeActive()
    {
        OnChangeActive();
        CloseEverything();
    }
    /// <summary>
    /// Display the given visitor's information.
    /// </summary>
    /// <param name="v">The selected visitor</param>
    public void visitorClicked(Visitor v)
    {
        visitorInfoPanel.SetActive(true);
        OnInfoOpened();
        OnMenuOpened();
        visitorHappines.text = "Boldogság: " + v.Happiness;
        visitorHunger.text = "Éhség: " + v.Hunger;
        visitorThirst.text = "Szomjúság: " + v.Thirst;
        visitorWC.text = "WC: " + v.Toilet;
        visitorMoney.text = "Pénz: " + v.Money;
    }
    /// <summary>
    /// Raise an event to the controller to change to Build mode.
    /// </summary>
    public void OnChangeToBuildMode()
    {
        if (ChangeToBuildMode != null)
            ChangeToBuildMode(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller to change to destruction mode.
    /// </summary>
    public void OnChangeToDestructionMode()
    {
        if (ChangeToDestructionMode != null)
            ChangeToDestructionMode(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that a menu was opened.
    /// </summary>
    public void OnMenuOpened()
    {
        if (MenuOpened != null)
            MenuOpened(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that a menu was closed.
    /// </summary>
    public void OnMenuClosed()
    {
        if (MenuClosed != null)
            MenuClosed(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that an info panel was opened.
    /// </summary>
    public void OnInfoOpened()
    {
        if (InfoOpened != null)
            InfoOpened(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that an info panel was closed.
    /// </summary>
    public void OnInfoClosed()
    {
        if (InfoClosed != null)
            InfoClosed(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that a building is selected.
    /// </summary>
    public void OnBuildingSelected()
    {
        if (BuildingSelected != null)
            BuildingSelected(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that a decoration is selected.
    /// </summary>
    public void OnDecorationSelected()
    {
        if (DecorationSelected != null)
            DecorationSelected(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that path is selected.
    /// </summary>
    public void OnPathSelected()
    {
        if (PathSelectedEvent != null)
            PathSelectedEvent(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that the player wants to save the selected building's information.
    /// </summary>
    public void OnSaveInformation()
    {
        if (SaveInformation != null)
            SaveInformation(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that the player wants to save the selected game's information.
    /// </summary>
    public void OnGSaveInformation()
    {
        if (GSaveInformation != null)
            GSaveInformation(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that the player wants to open the park.
    /// </summary>
    public void OnOpenPark()
    {
        if (OpenPark != null)
            OpenPark(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller that the building's status is changed.
    /// </summary>
    public void OnChangeActive()
    {
        if (ChangeActive != null)
            ChangeActive(this, EventArgs.Empty);
    }
    /// <summary>
     /// Raise an event to the controller to slow down time.
     /// </summary>
    public void OnSlowerTime()
    {
        if (SlowerTime != null)
            SlowerTime(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raise an event to the controller to make the game faster.
    /// </summary>
    public void OnFasterTime()
    {
        if (FasterTime != null)
            FasterTime(this, EventArgs.Empty);
    }
    /// <summary>
    /// Change the text where the current speed is displayed.
    /// </summary>
    /// <param name="n">The current speed of the game.</param>
    public void OnSpeedChanged(float n)
    {
        gameSpeedText.text = "Játék sebessége: " + Math.Round(n,1);
    }
    /// <summary>
    /// Display or close the panel where the player can see the key bindings.
    /// </summary>
    public void OnToolTipsPressed()
    {
        if(toolTipPanel.activeSelf)
        {
            CloseEverything();
        }
        else
        {
            CloseEverything();
            toolTipPanel.SetActive(true);
        }
    }
}
