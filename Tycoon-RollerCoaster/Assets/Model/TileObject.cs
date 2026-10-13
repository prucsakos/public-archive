using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Type of a Tile.
/// </summary>
public enum TileType
{
    Empty, Path, Building, Decoration, Illegal
}
/// <summary>
/// Type of a BuildingTileObject.
/// </summary>
public enum BuildingType
{
    Rollercoaster, MerryGoRound, FerrisWheel, Buffet, Candy, Restaurant, Drinks, Toilet, Illegal, Test
}
/// <summary>
/// Type of what a building provides.
/// </summary>
public enum Provides
{
    Fun, Food, Drink, Toilet
}
/// <summary>
/// Type of a DecorationTileObject.
/// </summary>
public enum DecorationType
{
    Grass, Tree, Bush, Illegal
}
/// <summary>
/// State of a BuildingTileObject.
/// </summary>
public enum State
{
    Building, Waiting, Going
}
/// <summary>
/// TileObject can be put onto the GameTile grid.
/// </summary>
public class TileObject
{
    protected TileType _tileType;

    protected Coords _middlePos; // középső blokk 
    protected Coords _startPos; // bal alsó blokk
    protected Coords _size;    // méret
    protected int _price;         // ár
    protected int _upkeep;       // fentartási költség

    public int Price { get { return _price; } set { _price = value; } }
    public int Upkeep { get { return _upkeep; } set { _upkeep = value; } }
    public TileType TileType { get { return _tileType; }}

    public Coords MiddlePos { get { return _middlePos; } }
    public Coords Size { get { return _size; } }
    public Coords StartPos { get { return _startPos; } }

    ///<summary>
    /// Given a TileObject's middle cordinate and size, this function calculates
    /// where the begining shall be, relatively to it's middle cord.
    ///</summary>
    public static int CalculateStartFromMiddle(int cord, int size) 
    {
        if (size % 2 == 0)
        {
            return (2 * cord - size + 2) / 2;
        }
        else
        {
            return (2 * cord - size + 1) / 2;
        }
    }
    /// <summary>
    /// Constructor of TileObject.
    /// </summary>
    protected TileObject()
    {
        _size = new Coords();
        _middlePos = new Coords();
        _startPos = new Coords();
    }
    /// <summary>
    /// Set the middle coordinates of the TileObject.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    public void SetMiddlePos(int x, int y)
    {
        
        _middlePos.x = x;
        _middlePos.y = y;
    }
    /// <summary>
    /// Returns true, if the given coords are the midle of the TileObject.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    /// <returns></returns>
    public bool IsMiddle(int x, int y)
    {
        if (_middlePos.x == x && _middlePos.y == y) return true;
        return false;
    }
    /// <summary>
    /// Set the starting position ( left down corner ) of the TileObject.
    /// </summary>
    /// <param name="x">X Cords</param>
    /// <param name="y">Y Cords</param>
    public void SetStartPos(int x, int y)
    {
        _startPos.x = x;
        _startPos.y = y;
    }
    
}
/// <summary>
/// Special TileObject. Works as a Building.
/// </summary>
public class BuildingTileObject : TileObject
{
    private string _name;
    public string Name { get { return _name; } set { _name = value; } }

    private Provides _provides;
    public Provides Provides { get { return _provides; } }

    private BuildingType _buildingType;
    public BuildingType BuildingType { get { return _buildingType; } }

    private List<Visitor> _que;
    public int QueueCount { get { return _que.Count; } }
    private List<Visitor> _inside;
    public int InsideCount { get { return _inside.Count; } }
    private float _minimum;
    public float Minimum { get { return _minimum; } set { _minimum = value; } }
    private float _maximum;
    public float Maximum { get { return _maximum; } set { _maximum = value; } }
    private State _state;
    public State CState { get { return _state; } set { _state = value; } }
    private float _buildTime;
    public float BuildTime { get { return _buildTime; } set { _buildTime = value; } }
    private float _runTime;
    public float RunTime { get { return _runTime; }}
    private float _runningFor;
    public float RunningFor { get { return _runningFor; } set { _runningFor = value; } }

    private bool _isActive;
    public bool IsActive { get { return _isActive; } set { _isActive = value; } }

    private int _ticket;
    public int Ticket { get { return _ticket; } set { _ticket = value; } }

    private int _roundCost;
    public int RoundCost { get { return _roundCost; } }

    private Money _moneyFromModel;

    public event EventHandler VisitorsKicked;
    /// <summary>
    /// Constructor of the BuildingTileObject.
    /// </summary>
    /// <param name="buildingType">Type of the building.</param>
    /// <param name="money">Money object of the Model.</param>
    public BuildingTileObject(BuildingType buildingType, Money money)
    {
        _size.x = 3;
        _size.y = 3;

        _buildingType = buildingType;

        _que = new List<Visitor>();
        _inside = new List<Visitor>();

        _state = State.Building;
        _buildTime = -10;
        _isActive = false;

        _moneyFromModel = money;

        switch (_buildingType)
        {
            case BuildingType.Test:
                _tileType = TileType.Building;
                _price = 50;
                _upkeep = 10;
                _provides = Provides.Fun;
                Minimum = 1;
                Maximum = 20;
                _runTime = 10;
                _ticket = 10;
                break;
            case BuildingType.Rollercoaster:
                _tileType = TileType.Building;
                _price = 1000;
                _upkeep = 60;
                _provides = Provides.Fun;
                Minimum = 5;
                Maximum = 40;
                _runTime = 30;
                _ticket = 20;
                _roundCost = 50;
                break;
            case BuildingType.MerryGoRound:
                _tileType = TileType.Building;
                _price = 200;
                _upkeep = 20;
                _provides = Provides.Fun;
                Minimum = 2;
                Maximum = 6;
                _runTime = 10;
                _ticket = 10;
                _roundCost = 10;
                break;
            case BuildingType.FerrisWheel:
                _tileType = TileType.Building;
                _price = 1000;
                _upkeep = 100;
                _provides = Provides.Fun;
                Minimum = 5;
                Maximum = 40;
                _runTime = 5;
                _ticket = 30;
                _roundCost = 100;
                break;
            case BuildingType.Buffet:
                _tileType = TileType.Building;
                _price = 200;
                _upkeep = 20;
                _provides = Provides.Food;
                Minimum = 2;
                Maximum = 5;
                _ticket = 10;
                _roundCost = 10;
                break;
            case BuildingType.Candy:
                _tileType = TileType.Building;
                _price = 200;
                _upkeep = 20;
                _provides = Provides.Food;
                Minimum = 2;
                Maximum = 5;
                _ticket = 10;
                _roundCost = 10;
                break;
            case BuildingType.Restaurant:
                _tileType = TileType.Building;
                _price = 700;
                _upkeep = 70;
                _provides = Provides.Drink;
                Minimum = 5;
                Maximum = 30;
                _ticket = 20;
                _roundCost = 20;
                break;
            case BuildingType.Drinks:
                _tileType = TileType.Building;
                _price = 200;
                _upkeep = 20;
                _provides = Provides.Drink;
                Minimum = 1;
                Maximum = 2;
                _ticket = 10;
                _roundCost = 10;
                break;
            case BuildingType.Toilet:
                _tileType = TileType.Building;
                _price = 100;
                _upkeep = 5;
                _provides = Provides.Toilet;
                _size.x = 1;
                _size.y = 1;
                Minimum = 1;
                Maximum = 1;
                _ticket = 10;
                _roundCost = 0;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Satisfies the given visitor.
    /// </summary>
    /// <param name="visitor">Visitor Type</param>
    public void Provide(Visitor visitor)
    {
        switch(_provides)
        {
            case Provides.Food:
                visitor.Hunger += 30;
                _que.Remove(visitor);
                visitor.Kicked();
                OnVisitorsKicked();
                _moneyFromModel.PayMoney(_roundCost);
                break;

            case Provides.Drink:
                visitor.Thirst += 30;
                _que.Remove(visitor);
                visitor.Kicked();
                OnVisitorsKicked();
                _moneyFromModel.PayMoney(_roundCost);
                break;

            case Provides.Toilet:
                visitor.Toilet += 30;
                _que.Remove(visitor);
                visitor.Kicked();
                OnVisitorsKicked();
                _moneyFromModel.PayMoney(_roundCost);
                break;

            default:
                break;
        }
    }
    /// <summary>
    /// Satisfies the visitors inside, then kicks them out.
    /// </summary>
    /// <returns></returns>
    public bool ProvideFirst()
    {
        if (_que.Count <= 0)
            return false;

        List<Visitor> toBeKicked = new List<Visitor>();

        bool provided = false;
        int i = 0;
        while(i < _que.Count && !provided)
        {
            Visitor visitor = _que[i];
            if(visitor.CanPay(_ticket) && visitor.Happiness > 0)
            {
                _moneyFromModel.ReciveMoney(visitor.Pay(_ticket));
                Provide(visitor);
                provided = true;
            }
            else
            {
                toBeKicked.Add(visitor);
            }

            i++;
        }

        foreach(Visitor visitor in toBeKicked)
        {
            _que.Remove(visitor);
            _inside.Remove(visitor);
            visitor.Kicked();
        }
        OnVisitorsKicked();

        return provided;
    }
    /// <summary>
    /// Adds the given visitor inside the queue
    /// </summary>
    /// <param name="visitor">Visitor</param>
    /// <returns></returns>
    public bool EnterQue(Visitor visitor)
    {
        if (!_isActive || _state == State.Building )
            return false;

        _que.Add(visitor);
        visitor.EnterQue();

        return true;
    }
    /// <summary>
    /// Starts the round of the building.
    /// </summary>
    /// <returns></returns>
    public bool NextRound()
    {
        if ((_provides != Provides.Fun) || (_state != State.Waiting) || !_isActive)
            return false;

        int insideCount = _inside.Count;
        for(int j = 0; j < insideCount; j++)
        {
            Visitor visitor = _inside[0];
            _inside.RemoveAt(0);
            visitor.Kicked();
        }
        OnVisitorsKicked();

        List<Visitor> possibleVisitors = new List<Visitor>();
        List<Visitor> toBeKicked = new List<Visitor>();
        int count = 0;
        int i = 0;
        while (i < _que.Count && count < _maximum)
        {
            Visitor visitor = _que[i];

            bool canPay = visitor.CanPay(_ticket);

            if(canPay && visitor.Happiness >= 0)
            {
                possibleVisitors.Add(visitor);
                count++;
            }
            else
            {
                toBeKicked.Add(visitor);
            }

            i++;
        }

        foreach(Visitor visitor in toBeKicked)
        {
            _que.Remove(visitor);
            _inside.Remove(visitor);
            visitor.Kicked();
        }
        OnVisitorsKicked();

        if (count < _minimum)
        {
            return false;
        }
        else
        {
            int money = 0;
            foreach(Visitor visitor in possibleVisitors)
            {
                money += visitor.Pay(_ticket);
                _que.Remove(visitor);
                _inside.Add(visitor);
                visitor.Happiness += 10;
                visitor.Inside();
            }
            _moneyFromModel.ReciveMoney(money);

            _state = State.Going;

            _runningFor = 0;

            _moneyFromModel.PayMoney(_roundCost);

            return true;
        }

    }
    /// <summary>
    /// Stops the round.
    /// </summary>
    public void EndOfRound()
    {
        if ((_provides != Provides.Fun) || (_state != State.Going))
            return;

        int count = _inside.Count;
        for(int i = 0; i < count; i++)
        {
            Visitor visitor = _inside[0];
            _inside.RemoveAt(0);
            visitor.Kicked();
        }
        OnVisitorsKicked();

        _state = State.Waiting;
    }
    /// <summary>
    /// Close the building, making it unavailable for visitors.
    /// </summary>
    public void Close()
    {
        _isActive = false;
        int queCount = _que.Count;
        for(int i = 0; i < queCount; i++)
        {
            Visitor visitor = _que[0];
            _que.RemoveAt(0);
            visitor.Happiness -= 10;
            visitor.Kicked();
        }
        OnVisitorsKicked();
    }
    /// <summary>
    /// Opens the building, making it available for visitors.
    /// </summary>
    public void Open()
    {
        _isActive = true;
        _state = State.Waiting;
    }
    /// <summary>
    /// This event gets triggered when a visitor is kicked.
    /// </summary>
    public void OnVisitorsKicked()
    {
        if (VisitorsKicked != null)
            VisitorsKicked(this, EventArgs.Empty);
    }
}
/// <summary>
/// Special TileObject. Works as a Decoration.
/// </summary>
public class DecorationTileObject : TileObject
{
    private DecorationType _decorationType;
    public DecorationType DecorationType { get { return _decorationType; } }
    /// <summary>
    /// Constructor of the DecorationTileObject.
    /// </summary>
    /// <param name="decorationType">Type of the DecorationTileObject</param>
    public DecorationTileObject(DecorationType decorationType)
    {
        _size.x = 1;
        Size.y = 1;
        _price = 20;

        _decorationType = decorationType;

        switch (DecorationType)
        {
            case DecorationType.Grass:
                _tileType = TileType.Decoration;
                _price = 5;
                _upkeep = 0;
                break;
            case DecorationType.Tree:
                _tileType = TileType.Decoration;
                _price = 20;
                _upkeep = 0;
                break;
            case DecorationType.Bush:
                _tileType = TileType.Decoration;
                _price = 10;
                _upkeep = 0;
                break;
            default:
                break;
        }
    }
}
/// <summary>
/// Special TileObject. Works as a path.
/// </summary>
public class PathTileObject : TileObject
{

    public PathTileObject()
    {
        _price = 10;
        _upkeep = 0;
        _size.x = 1;
        _size.y = 1;
        _price = 10;

        _tileType = TileType.Path;
    }
}


