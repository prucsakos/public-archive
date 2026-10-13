using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
/// <summary>
/// Model of Tegridy's RollerCoaster Game.
/// </summary>
public class GameModel
{
    private TileObject _selectedTileObject;
    public TileObject SelectedTileObject { get { return _selectedTileObject; } set { _selectedTileObject = value; } }

    public BuildingTileObject selectedBuilding;
    private BuildingTileObject helperTile;

    private Visitor _selectedVisitor;

    private GameTile[,] _tileGrid;
    private int _size;

    private Money _money;
    public Money MoneyObject { get { return _money; } }
    public int Money { get { return _money.Value; } }

    private bool _open;
    public bool Open { get { return _open; } }

    public event EventHandler TileGridChanged;
    public event EventHandler VisitorChanged;
    public event EventHandler<Coords> BuildingDestroyed;

    private List<TileObject> _tileObjects;
    public List<TileObject> TileObjects { get { return _tileObjects; } }

    private List<BuildingTileObject> _buildingTileObjects;
    public List<BuildingTileObject> BuildingTileObjects { get { return _buildingTileObjects; } }

    private List<Visitor> _visitors;
    public List<Visitor> Visitors { get { return _visitors; } }

    private int _buildingNumber;
    public int BuildingNumber { get { return _buildingNumber; } }

    private int _visitorNumber;
    public int VisitorNumber { get { return _visitorNumber; } set { _visitorNumber = value; } }

    int _ticket;
    public int Ticket { get { return _ticket; } set { _ticket = value; } }

    private System.Random _random;

    /// <summary>
    /// Constructor of the GameModel.
    /// </summary>
    /// <param name="boardSize">Size of the board</param>
    public GameModel(int boardSize)
    {
        _random = new System.Random();

        _size = boardSize;
        _tileGrid = new GameTile[_size, _size];
        _tileObjects = new List<TileObject>();
        _buildingTileObjects = new List<BuildingTileObject>();
        _visitors = new List<Visitor>();
        _money = new Money(_tileObjects, 1000);
        _buildingNumber = 0;
        _visitorNumber = 0;
        _ticket = 10;
    }
    /// <summary>
    /// Returns the TileType of a given position on the board.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public TileType getTileType(int x, int y)
    {
        return _tileGrid[x, y].TileType;
    }
    /// <summary>
    /// Returns the TileObject of a given position on the board.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public TileObject getTileObject(int x, int y)
    {
        return _tileGrid[x, y].TileObject;
    }
    /// <summary>
    /// Returns the GameTile of a given position on the board.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public GameTile getGameTile(int x, int y)
    {
        return _tileGrid[x, y];
    }
    /// <summary>
    /// Returns the number of the visitors currently on the board.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public int getVisitorCount(int x, int y)
    {
        if (x < 0 || x >= _size || y < 0 || y >= _size)
            return -1;

        int count = 0;
        foreach(Visitor visitor in _tileGrid[x, y].Visitors)
        {
            if (!visitor.InQue && !visitor.InFun)
                count++;
        }
        return count;
    }
    /// <summary>
    /// Setups the grid. The size is given in the GameModel's contructor.
    /// </summary>
    public void SetupGrid()
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _tileGrid[i, j] = new GameTile(i,j,null);
            }
        }

        //ide spawnolnak a látogatók
        _tileGrid[0, 0] = new GameTile(0,0,new PathTileObject());
        OnTileGridChanged();
    }

    // Choose already existing building
    /// <summary>
    /// Returns a TileObject on the board if exists, else returns null.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public TileObject SelectTile(int x, int y)
    {
        if (_tileGrid[x, y].TileType == TileType.Building || _tileGrid[x, y].TileType == TileType.Building || _tileGrid[x, y].TileType == TileType.Path)
            return _tileGrid[x, y].TileObject;
        else
            return null;
    }

    // Kijelölik az adott építmény-típust.
    /// <summary>
    /// Prepares a BuildingTileObject for building.
    /// </summary>
    /// <param name="bt">Building type</param>
    public void BuildingSelected(BuildingType bt)
    {
        _selectedTileObject = new BuildingTileObject(bt, _money);
    }
    /// <summary>
    /// Prepares a DecorationTileObject for building.
    /// </summary>
    /// <param name="dt">Decoration type</param>
    public void DecorationSelected(DecorationType dt)
    {
        _selectedTileObject = new DecorationTileObject(dt);
    }
    /// <summary>
    /// Prepares a PathTileObject for building.
    /// </summary>
    public void PathSelected()
    {
        _selectedTileObject = new PathTileObject();
    }
    /// <summary>
    /// Builds the selected TileObject to the given position.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public bool Build(int x, int y)
    {
        int StartX = TileObject.CalculateStartFromMiddle(x, SelectedTileObject.Size.x);
        int StartY = TileObject.CalculateStartFromMiddle(y, SelectedTileObject.Size.y);
        if (CanBuild(StartX, StartY))
        {
            _money.PayMoney(SelectedTileObject.Price);
            if (SelectedTileObject is BuildingTileObject)
            {
                _buildingNumber++;
                _buildingTileObjects.Add((BuildingTileObject)SelectedTileObject);
                ((BuildingTileObject)SelectedTileObject).VisitorsKicked += new EventHandler(OnVisitorKicked);
            }
            SelectedTileObject.SetMiddlePos(x, y);
            SelectedTileObject.SetStartPos(StartX, StartY);
            _tileObjects.Add(_selectedTileObject);

            for (int i = StartX; i <= StartX + SelectedTileObject.Size.x - 1; i++)
            {
                for (int j = StartY; j <= StartY + SelectedTileObject.Size.y - 1; j++)
                {
                    _tileGrid[i, j] = new GameTile(i, j, _selectedTileObject);
                }
            }
            if (_selectedTileObject is PathTileObject)
                PathSelected();
            else
            {
                _selectedTileObject = null;
            }   
            return true;
        }
        return false;
    }
    /// <summary>
    /// Destroys a TileObject on the board if possible.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public bool Destroy(int x, int y)
    {
        //can't delete 0,0 path
        if (x == 0 && y == 0) return false;
        if (_tileGrid[x, y].Visitors.Count > 0) return false;  // ha van visitor a mezőn akkor false

        TileObject to = _tileGrid[x, y].TileObject;
        if (to == null) return false;
        if (to.TileType == TileType.Building)
        {
            BuildingTileObject bto = (BuildingTileObject)(to);
            if (bto.IsActive || (bto.InsideCount + bto.QueueCount) > 0) return false;
        }
        

        _tileObjects.Remove(to);
        if (to is BuildingTileObject)
        {
            _buildingTileObjects.Remove((BuildingTileObject)to);
            _buildingNumber--;
            OnBuildingDestroyed(new Coords(to.MiddlePos.x, to.MiddlePos.y));
        }
        _money.ReciveMoney((int)(to.Price * 0.3f));

        for (int i = to.StartPos.x; i <= to.StartPos.x + to.Size.x - 1; i++)
        {
            for (int j = to.StartPos.y; j <= to.StartPos.y + to.Size.y - 1; j++)
            {
                _tileGrid[i, j] = new GameTile(i, j, null);
            }
        }

        OnTileGridChanged();
        return true;
    }
    /// <summary>
    /// Checks if building is possible on the given position.
    /// </summary>
    /// <param name="StartX">X Cords</param>
    /// <param name="StartY">Y Cords</param>
    /// <param name="CheckMoney"></param>
    /// <returns></returns>
    public bool CanBuild(int StartX, int StartY, bool CheckMoney = true)
    {
        if (_selectedTileObject == null) return false;
        if (CheckMoney)
        {
            
            if (Money < SelectedTileObject.Price) return false;
        }
        Coords size = _selectedTileObject.Size;

        // Ellenőrzi, pályán belül és nincs építve oda eddig 
        if (StartX < 0 || StartX + size.x - 1 >= _size || StartY < 0 || StartY + size.y - 1 >= _size) return false;
        for (int i = StartX; i <= StartX + size.x - 1; i++)
        {
            for (int j = StartY; j <= StartY + size.y - 1; j++)
            {
                if (_tileGrid[i, j].TileObject != null) return false;
            }
        }

        // Ellenőrzi, hogy van-e út az épület mellett
        if (_selectedTileObject is DecorationTileObject) // dekorációnál nem nézzük
            return true;

        bool nextToPath = false;
        int leftCornerX = (StartX - 1);
        int bottomCornerY = (StartY - 1);
        int rightCornerX = (StartX + size.x);
        int upperCornerY = (StartY + size.y);
        for (int i = leftCornerX; i <= rightCornerX; i++)
        {
            for (int j = bottomCornerY; j <= upperCornerY; j++)
            {
                //If: Ne indexeljen ki a pályáról, csak a széleket ellenőrizze,  sakokat ignorálja.
                if (    ( i>=0 && i < _size && j >=0 && j<_size )
                        &&  ( i == leftCornerX || i == rightCornerX || j == bottomCornerY || j == upperCornerY )
                        && !(( i == leftCornerX && j == bottomCornerY ) || ( i == leftCornerX && j == upperCornerY ) || ( i == rightCornerX && j == upperCornerY ) || (i == rightCornerX && j == bottomCornerY))
                        && _tileGrid[i, j].TileObject != null)
                {
                    nextToPath = nextToPath || _tileGrid[i, j].TileObject.TileType == TileType.Path;
                }
            }
        }
        return nextToPath;
    }
    /// <summary>
    /// Checks if you have enough money for building.
    /// </summary>
    /// <returns></returns>
    public bool HasMoney()
    {
        if (SelectedTileObject == null) return false;
        if (Money >= SelectedTileObject.Price) return true;
        return false;
    }
    /// <summary>
    /// Seconds passed. The GameControl uses this method.
    /// </summary>
    /// <param name="time"></param>
    public void secPassed(float time)
    {
        foreach (TileObject to in _tileObjects)
        {
            if(to is BuildingTileObject)
            {
                helperTile = (BuildingTileObject)to;
                if (helperTile.CState==State.Building)
                {
                    helperTile.BuildTime += time;
                    if(helperTile.BuildTime >= 0)
                    {
                        helperTile.CState = State.Waiting;
                        helperTile.IsActive = true;
                    }
                }
                else
                {
                    if(helperTile.Provides == Provides.Fun)
                    {
                        if (helperTile.CState == State.Waiting && helperTile.IsActive)
                        {
                            helperTile.NextRound();
                        }
                        else
                        {
                            helperTile.RunningFor += time;
                            if(helperTile.RunningFor >= helperTile.RunTime)
                            {
                                helperTile.EndOfRound();
                            }
                        }
                    }
                    else
                    {
                        if(helperTile.IsActive)
                            helperTile.ProvideFirst();
                    }
                }
            }
        }
    }
    /// <summary>
    /// Spawn a visitor at the entrance.
    /// </summary>
    public void spawnVisitors()
    {
        if (_visitorNumber < _buildingNumber * 10)
        {
            if (_tileGrid[0, 0].TileType == TileType.Path)
            {
                Visitor visitor = new Visitor(0, 0, _tileObjects, _buildingTileObjects, _tileGrid, _size, _random);

                if(visitor.CanPay(_ticket))
                {
                    _money.ReciveMoney(visitor.Pay(_ticket));
                    _visitors.Add(visitor);
                    _tileGrid[0, 0].Visitors.Add(visitor);
                    _visitorNumber++;
                    OnVisitorChanged();
                }   
            }
        }
    }
    /// <summary>
    /// Updates the status of the visitors. 
    /// </summary>
    public void UpdateVisitors()
    {
        List<Visitor> toBeRemoved = new List<Visitor>();

        foreach (Visitor visitor in _visitors)
        {
            if (visitor.WentHome)
            {
                toBeRemoved.Add(visitor);
            }
            else if (visitor.GoingHome)
            {
                if (visitor.Step())
                {
                    GameTile oldTile = getGameTile(visitor.lastX, visitor.lastY);
                    oldTile.Visitors.Remove(visitor);
                    GameTile newTile = getGameTile(visitor.X, visitor.Y);
                    newTile.Visitors.Add(visitor);
                }
            }
            else
            {
                if (visitor.InQue)
                {
                    int decorationCount = visitor.getNearbyDecorations().Count;

                    if (decorationCount == 0)
                        visitor.Happiness -= 3;
                    else
                        visitor.Happiness -= 2;
                }
                else if (visitor.InFun)
                {
                    //increase happiness
                    visitor.Happiness += 10;
                }
                else
                {
                    int decorationCount = visitor.getNearbyDecorations().Count;

                    if (decorationCount == 0)
                        visitor.Happiness -= 2;
                    else
                        visitor.Happiness -= 1;

                    if (visitor.Step())
                    {
                        GameTile oldTile = getGameTile(visitor.lastX, visitor.lastY);
                        oldTile.Visitors.Remove(visitor);
                        GameTile newTile = getGameTile(visitor.X, visitor.Y);
                        newTile.Visitors.Add(visitor);
                    }
                }

                //decrease hunger
                visitor.Hunger--;
                //decrease thirst
                visitor.Thirst--;
                //decreate toilet
                visitor.Toilet--;


                int rand = _random.Next(0, 20); // 0 -> refresh anyway, 1-9 refresh if needed, 10-19 no refresh
                if (rand == 0 || ( rand >= 1 && rand <= 9 && (visitor.Hunger <= 30 || visitor.Thirst <= 30 || visitor.Toilet <= 30)))
                {
                    visitor.Happiness--;
                    visitor.RefreshDestinations();
                }
                else if(visitor.Happiness <= 30)
                {
                    visitor.RefreshDestinations();
                }

                if (visitor.Happiness <= 0)
                {
                    visitor.GoHome();
                }

            }
        }

        foreach (Visitor v in toBeRemoved)
        {
            _tileGrid[v.X, v.Y].Visitors.Remove(v);
            _visitors.Remove(v);
            _visitorNumber--;
            OnVisitorChanged();
        }
    }
    /// <summary>
    /// Calculates the average happines of the visitors on the board.
    /// </summary>
    /// <returns></returns>
    public double avarageHappiness()
    {
        double sum=0;
        int n=0;
        foreach (Visitor v in _visitors)
        {
            sum += v.Happiness;
            n++;
        }
        return Math.Round(sum / n);
    }
    /// <summary>
    /// This event gets triggered when a visitor gets kicked out of a building.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnVisitorKicked(object sender, EventArgs e)
    {
        OnVisitorChanged();
    }
    /// <summary>
    /// This event gets triggered when the status changes of a visitor.
    /// </summary>
    public void OnVisitorChanged()
    {
        if (VisitorChanged != null)
            VisitorChanged(this, EventArgs.Empty);
    }
    /// <summary>
    /// The event gets triggered when the board changes.
    /// </summary>
    public void OnTileGridChanged()
    {
        if (TileGridChanged != null)
            TileGridChanged(this, EventArgs.Empty);
    }
    /// <summary>
    /// This event gets triggered when a building is destroyed
    /// </summary>
    /// <param name="coords">Coordinates of the destroyed building.</param>
    public void OnBuildingDestroyed(Coords coords)
    {
        if (BuildingDestroyed != null)
            BuildingDestroyed(this, coords);
    }
}
