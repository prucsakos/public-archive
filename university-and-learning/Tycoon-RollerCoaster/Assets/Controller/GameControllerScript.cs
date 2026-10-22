using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameControllerScript : MonoBehaviour
{
    //Unity-ben adjuk meg
    public int boardSize;
    public GameViewScript gameView;
    public GameObject cameraObject;
    public float cameraSpeed;
    public float buildUpdateSec; //hány másodpercenként frissüljön az építése az épületnek
    public float moneyUpdateSec; //hány másodpercenként frissítsük a pénzt
    public float visitorUpdateSec; //hány másodpercenként léptessük a látogatókat
    public float buildingUpdateSec; //hány másodpercenként "lépjenek" az épületek (látogatók kezelése, stb)
    public float visitorSpawnSec; //hány másodpercenként spawnoljanak a látogatók
    public float initialVisitorDelaySec; //hány másodpercig ne legyenek látogatók a park kinyitása után
    public float minSpeed; // minimum time speed
    public float maxSpeed; // maximum time speed

    //Start-ban hozzuk lézre
    private GameModel _gameModel;
    private Camera _camera;

    private float clickCooldown;
    private float keyCooldown;
    private AudioSource backgroundMusic;

    private bool menuUp;
    private bool informationUp;
    private bool isOpened;
    //Timer
    private float time;
    private float moneyTime;
    private float visitorTime;
    private float spawnTime;
    private float spawnDelayTime;

    private float _timerModifier;

    public enum Mode
    {
        Idle, Building, Destruction
    }
    public Mode gamemode;

    /// <summary>
    /// Starts the game with initializing variables, connecting event handlers
    /// </summary>
    void Start()
    {
        if (boardSize <= 0)
            return;

        _timerModifier = 1;

        _camera = cameraObject.GetComponent<Camera>();
        backgroundMusic = _camera.GetComponent<AudioSource>();

        _gameModel = new GameModel(boardSize);
        _gameModel.SetupGrid();
        _gameModel.MoneyObject.MoneyChanged += new EventHandler(OnMoneyChanged);
        _gameModel.VisitorChanged += new EventHandler(OnChangeVisitorNumber);
        _gameModel.BuildingDestroyed += new EventHandler<Coords>(OnBuildingDestroyed);


        gamemode = Mode.Idle;

        gameView.SetupGrid(_gameModel, boardSize, gamemode);
        gameView.ChangeToBuildMode += new EventHandler(OnChangeToBuild);
        gameView.ChangeToDestructionMode += new EventHandler(OnChangeToDestruction);
        gameView.MenuOpened += new EventHandler(OnMenuOpened);
        gameView.MenuClosed += new EventHandler(OnMenuClosed);
        gameView.BuildingSelected += new EventHandler(OnBuildingSelectedInView);
        gameView.DecorationSelected += new EventHandler(OnDecorationSelectedInView);
        gameView.PathSelectedEvent += new EventHandler(OnPathSelectedInView);
        gameView.SaveInformation += new EventHandler(OnSaveInformation);
        gameView.GSaveInformation += new EventHandler(OnGSaveInformation);
        gameView.InfoOpened += new EventHandler(OnInformationOpened);
        gameView.InfoClosed += new EventHandler(OnInformationClosed);
        gameView.OpenPark += new EventHandler(OnOpenPark);
        gameView.ChangeActive += new EventHandler(OnChangeActive);
        gameView.SlowerTime += new EventHandler(OnSlowerTime);
        gameView.FasterTime += new EventHandler(OnFasterTime);
        gameView.ticketButton.onClick.AddListener(OnTicketSelected);
        gameView.OnSpeedChanged(_timerModifier);

        _gameModel.TileGridChanged += new EventHandler(OnTileGridChanged);

        clickCooldown = 1;
        keyCooldown = 1;

        gameView.setParkName(ParkName.Name);
        gameView.avaragePanel.SetActive(false);

		menuUp = false;
        informationUp = false;
        isOpened = false;
        time = 0;
        moneyTime = 0;
        visitorTime = 0;
        spawnTime = 0;
        spawnDelayTime = 0;
        OnMoneyChanged(this, EventArgs.Empty);
        gameView.setVisitors(0);
    }

    /// <summary>
    /// Updates the game, handles key inputs and clicks, calls methods based on elapsed time
    /// </summary>
    void Update()
    {
        if ((Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))&& !informationUp)
        {
            _camera.transform.localPosition += new Vector3(0, 1, 0) * cameraSpeed;
        }
        if ((Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)) && !informationUp)
        {
            _camera.transform.localPosition += new Vector3(0, -1, 0) * cameraSpeed;
        }
        if ((Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow)) && !informationUp)
        {
            _camera.transform.localPosition += new Vector3(-1, 0, 0) * cameraSpeed;
        }
        if ((Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)) && !informationUp)
        {
            _camera.transform.localPosition += new Vector3(1, 0, 0) * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (gamemode == Mode.Building)
            {
                Vector3 hoverPos = _camera.ScreenToWorldPoint(Input.mousePosition);

                if (hoverPos.x >= 0 && hoverPos.y >= 0 && hoverPos.x < boardSize && hoverPos.y < boardSize)
                {
                    int tileX = Mathf.FloorToInt(hoverPos.x);
                    int tileY = Mathf.FloorToInt(hoverPos.y);

                    TileObject tileObject = _gameModel.SelectedTileObject;
                    if(tileObject == null)
                        gameView.UpdateHighlightGrid(tileX, tileY, new Coords(0,0), false);
                    else
                    {
                        int StartX = TileObject.CalculateStartFromMiddle(tileX, tileObject.Size.x);
                        int StartY = TileObject.CalculateStartFromMiddle(tileY, tileObject.Size.y);

                        gameView.UpdateHighlightGrid(tileX, tileY, tileObject.Size, _gameModel.CanBuild(StartX, StartY, false));
                    }
                }
            }
        }

        // itt ne változzon az idő, kell delay a billentyűknek, hogy egyszer legyenek csak lenyomva
        if (keyCooldown >= 0.07 && !informationUp)
        {
            keyCooldown = 0;

            if (Input.GetKey("e"))
            {
                OnChangeToIdle(true);
            }
            if (Input.GetKey("t"))
            {
                gameView.BuildingMenuSelected();
            }
            if (Input.GetKey("z"))
            {
                gameView.DecorationgMenuSelected();
            }
            if (Input.GetKey("r"))
            {
                gameView.DestructionSelected();
            }
            if (Input.GetKey("n"))
            {
                backgroundMusic.volume -= 0.1f;
            }
            if (Input.GetKey("m"))
            {
                backgroundMusic.volume += 0.1f;
            }
            if (Input.GetKey("q"))
            {
                if(_gameModel.SelectedTileObject is PathTileObject)
                    OnChangeToIdle(true);
                else
                    gameView.TileTypeSelected("Path");
            }
        }
        else
        {
            keyCooldown += Time.deltaTime;

            if (keyCooldown > 10)
            {
                keyCooldown = 5;
            }
        }

        // click cooldown szintén ne változzon gyorsításkor
        if (Input.GetMouseButton(0) && clickCooldown >= 0.1 && !EventSystem.current.IsPointerOverGameObject())
        {
            clickCooldown = 0;

            Vector3 clickPos = _camera.ScreenToWorldPoint(Input.mousePosition);

            if (clickPos.x >= 0 && clickPos.y >= 0 && clickPos.x < boardSize && clickPos.y < boardSize)
            {
                int tileX = Mathf.FloorToInt(clickPos.x);
                int tileY = Mathf.FloorToInt(clickPos.y);

                switch (gamemode)
                {
                    case Mode.Idle:
                        if(!menuUp)
                        {
                            TileObject selected = _gameModel.SelectTile(tileX, tileY);
                            if(selected != null)
                            {
                                if (selected is BuildingTileObject)
                                {
                                    //call method in view to show info panel with TileObject
                                    gameView.showInformation((BuildingTileObject)selected);
                                    _gameModel.selectedBuilding = (BuildingTileObject)selected;
                                }
                                else if(selected is PathTileObject)
                                {
                                    GameTile gameTile = _gameModel.getGameTile(tileX, tileY);
                                    if(gameTile.VisitorCount != 0)
                                    {
                                        gameView.visitorClicked(gameTile.Visitors[UnityEngine.Random.Range(0,gameTile.VisitorCount)]);
                                    }
                                }
                            }
                        }
                        break;
                    case Mode.Building:
                        if (!_gameModel.Build(tileX, tileY))
                        {
                            if (!(_gameModel.SelectedTileObject is PathTileObject))
                                gameView.badLocation();
                        }
                        else
                        {
                            gameView.playBuildSound();
                        }
                        if (!(_gameModel.SelectedTileObject is PathTileObject))
                            OnChangeToIdle();
                        else
                            gameView.UpdateTileGrid(gamemode);
                        break;
                    case Mode.Destruction:
                        if(_gameModel.Destroy(tileX, tileY))
                        {
                            gameView.playDestroySound();
                        }
                        break;
                }
            }
        }
        else
        {
            clickCooldown += Time.deltaTime;

            if(clickCooldown > 10)
            {
                clickCooldown = 5;
            }
        }

        time += (Time.deltaTime*_timerModifier);

        if (time > buildUpdateSec)
        {
            _gameModel.secPassed(time);
            gameView.UpdateBuildingGrid();
            time = 0;
        }
        if (isOpened)
        {
            moneyTime += (Time.deltaTime * _timerModifier);
            visitorTime += (Time.deltaTime * _timerModifier);
            spawnTime += (Time.deltaTime * _timerModifier);
            if(spawnDelayTime < initialVisitorDelaySec)
                spawnDelayTime += (Time.deltaTime * _timerModifier);

            if (moneyTime > moneyUpdateSec)
            {
                moneyTime = 0;
                _gameModel.MoneyObject.Update();
            }
            if (visitorTime > visitorUpdateSec)
            {
                visitorTime = 0;
                _gameModel.UpdateVisitors();
                gameView.UpdateVisitorGrid();
            }
            if (spawnDelayTime >= initialVisitorDelaySec && spawnTime > visitorSpawnSec)
            {
                spawnTime = 0;
                _gameModel.spawnVisitors();
                gameView.UpdateVisitorGrid();
            }

        }
        if (_gameModel.VisitorNumber > 0)
        {
            gameView.avaregeHappinesText.text = "Átlag elégedettség: " + _gameModel.avarageHappiness().ToString();
        }
    }

    /// <summary>
    /// Event handling, updates view
    /// </summary>
    void OnTileGridChanged(object sender, EventArgs e)
    {
        gameView.UpdateTileGrid(gamemode);
    }
    /// <summary>
    /// Event handling, changes gamemode to Idle
    /// </summary>
    void OnChangeToIdle(bool closeEverything = false)
    {
        gamemode = Mode.Idle;
        _gameModel.SelectedTileObject = null;
        gameView.UpdateTileGrid(gamemode);
        if(closeEverything)
            gameView.CloseEverything();
        gameView.changeDestructionButtonText("Pusztítás");
    }
    /// <summary>
    /// Event handling changes gamemode to Build
    /// </summary>
    void OnChangeToBuild(object sender, EventArgs e)
    {
        gamemode = Mode.Building;
        gameView.UpdateTileGrid(gamemode);
        gameView.changeDestructionButtonText("Pusztítás");
    }
    /// <summary>
    /// Event handling, changes gamemode to Destruction or from Destruction
    /// </summary>
    void OnChangeToDestruction(object sender, EventArgs e)
    {
        if (gamemode == Mode.Destruction || gamemode == Mode.Building)
        {
            gamemode = Mode.Idle;
            gameView.changeDestructionButtonText("Pusztítás");
        }
        else
        {
            gamemode = Mode.Destruction;
            gameView.changeDestructionButtonText("Alap állapot");
        }
        _gameModel.SelectedTileObject = null;
        gameView.UpdateTileGrid(gamemode);
    }
    /// <summary>
    /// Event handling, a menu has been opened
    /// </summary>
    void OnMenuOpened(object sender, EventArgs e)
    {
        menuUp = true;

        OnChangeToIdle();
        if (gamemode == Mode.Destruction)
        {
            gameView.changeDestructionButtonText("Pusztítás");
        }
    }
    /// <summary>
    /// Event handling, a menu has been closed
    /// </summary>
    void OnMenuClosed(object sender, EventArgs e)
    {
        menuUp = false;
    }
    /// <summary>
    /// Event handling, balance changed
    /// </summary>
    void OnMoneyChanged(object sender, EventArgs e)
    {
        gameView.setMoney(_gameModel.Money);
    }
    /// <summary>
    /// Event handling, user wants to build a building
    /// </summary>
    void OnBuildingSelectedInView(object sender, EventArgs e)
    {
        _gameModel.BuildingSelected(gameView.SelectedBuilding);
    }
    /// <summary>
    /// Event handling, user wants to build a decoration
    /// </summary>
    void OnDecorationSelectedInView(object sender, EventArgs e)
    {
        _gameModel.DecorationSelected(gameView.SelectedDecoration);
    }
    /// <summary>
    /// Event handling, user wants to build a path
    /// </summary>
    void OnPathSelectedInView(object sender, EventArgs e)
    {
        _gameModel.PathSelected();
    }
    /// <summary>
    /// Event handling, information menu of a building has been opened
    /// </summary>
    void OnInformationOpened(object sender, EventArgs e)
    {
        informationUp = true;
    }
    /// <summary>
    /// Event handling, information menu of a building has been closed
    /// </summary>
    void OnInformationClosed(object sender, EventArgs e)
    {
        informationUp = false;
    }
    /// <summary>
    /// Event handling, name or ticket price change from information menu saved
    /// Deprecated
    /// </summary>
    void OnSaveInformation(object sender, EventArgs e)
    {
        try
        {
            if (int.Parse(gameView.tilePriceInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite) < 1)
            {
                throw new Exception("asd");
            }
            _gameModel.selectedBuilding.Ticket = int.Parse(gameView.tilePriceInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            _gameModel.selectedBuilding.Name = gameView.tileNameInput.text;
        }
        catch(Exception ex)
        {
            gameView.badPrice();
        }
    }
    /// <summary>
    /// Event handling, name, ticket price or min visitor % change from information menu saved
    /// </summary>
    void OnGSaveInformation(object sender, EventArgs e)
    {
        try
        {
            if(int.Parse(gameView.gTileMinimumInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite)>100 || int.Parse(gameView.gTileMinimumInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite)<1)
            {
                throw new DivideByZeroException("bad % given");
            }
            if (int.Parse(gameView.gTilePriceInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite) <1)
            {
                throw new Exception();
            }
            _gameModel.selectedBuilding.Ticket = int.Parse(gameView.gTilePriceInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            _gameModel.selectedBuilding.Name = gameView.gTileNameInput.text;
            int helper = int.Parse(gameView.gTileMinimumInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            _gameModel.selectedBuilding.Minimum = Mathf.Round((helper/100.0f)* _gameModel.selectedBuilding.Maximum);
            if (_gameModel.selectedBuilding.Minimum == 0) { _gameModel.selectedBuilding.Minimum = 1; }
        }
        catch (DivideByZeroException c)
        {
            gameView.badPercentage();
        }
        catch (Exception ex)
        {
            gameView.badPrice();
        }
    }
    /// <summary>
    /// Event handling, park opened
    /// </summary>
    void OnOpenPark(object sender, EventArgs e)
    {
        isOpened = true;
        gameView.avaragePanel.SetActive(true);
    }
    /// <summary>
    /// Event handling, open/close building
    /// </summary>
    void OnChangeActive(object sender, EventArgs e)
    {
        if(_gameModel.selectedBuilding.IsActive)
        {
            _gameModel.selectedBuilding.Close();
        }
        else
        {
            _gameModel.selectedBuilding.Open();
        }
    }
    /// <summary>
    /// Event handling, visitor number changed
    /// </summary>
    void OnChangeVisitorNumber(object sender, EventArgs e)
    {
        gameView.setVisitors(_gameModel.VisitorNumber);
        gameView.UpdateVisitorGrid();
    }
    /// <summary>
    /// Event handling, building destroyed
    /// </summary>
    void OnBuildingDestroyed(object sender, Coords coords)
    {
        gameView.buildingMap.SetTile(new Vector3Int(coords.x, coords.y, 0), null);
    }
    /// <summary>
    /// Event handling, time slowed
    /// </summary>
    void OnSlowerTime(object sender, EventArgs e)
    {
        _timerModifier -= 0.5f;
        if (_timerModifier <= minSpeed)
            _timerModifier = minSpeed;

        gameView.OnSpeedChanged(_timerModifier);
    }
    /// <summary>
    /// Event handling, time sped up
    /// </summary>
    void OnFasterTime(object sender, EventArgs e)
    {
        _timerModifier += 0.5f;
        if (_timerModifier > maxSpeed)
            _timerModifier = maxSpeed;

        gameView.OnSpeedChanged(_timerModifier);
    }
    /// <summary>
    /// Event handling, new ticket price set
    /// </summary>
    void OnTicketSelected()
    {
        if(gameView.ticketPanel.activeSelf)
        {
            gameView.ticketPanel.SetActive(false);
            informationUp = false;
            try
            {
                if (int.Parse(gameView.ticketPriceInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite)<1)
                {
                    throw new Exception();
                }
                _gameModel.Ticket = int.Parse(gameView.ticketPriceInput.text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            }
            catch (Exception c)
            {
                gameView.badPrice();
            }
        }
        else
        {
            gameView.ticketPanel.SetActive(true);
            informationUp = true;
            gameView.ticketPriceInput.text = _gameModel.Ticket.ToString();
        }
    }
}
