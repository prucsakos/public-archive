using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visitor
{
    private int _x;
    private int _y;
    public int X { get { return _x; } }
    public int Y { get { return _y; } }

    private int _lastX;
    private int _lastY;
    public int lastX { get { return _lastX; } }
    public int lastY { get { return _lastY; } }

    private BlackListOfTiles _pastPathDecisions;  // Kereszteződésnél elmenti a megjárt irányokat. 

    private bool _lastTileWasPath;
    public bool LastTileWasPath { get { return _lastTileWasPath; } }

    private bool _atDest; //Célállomásnál van-e jelenleg.
    public bool AtDest { get { return _atDest; } set { _atDest = value; } }

    private bool _inQue; //sorban áll, gyorsabban fogy a happiness
    public bool InQue { get { return _inQue; } set { _inQue = value; } }

    private bool _inFun; //fun helyen van (hullámvasút stb.)
    public bool InFun { get { return _inFun; } set { _inFun = value; } }

    private bool _goingHome; //megy a park bejárata felé?
    public bool GoingHome { get { return _goingHome; } set { _goingHome = value; } }

    private bool _wentHome; //hazament már, ilyenkor töröljük majd
    public bool WentHome { get { return _wentHome; } set { _wentHome = value; } }


    private int _money; //mennyi pénze van
    public int Money { get { return _money; } set { _money = value; } }

    private int _moneyLimit; //mennyit fizetnének belépésért / dolgokért
    public int MoneyLimit { get { return _moneyLimit; } }

    private int _happiness; //csökken, lassabban, ha van növény, nő, ha inFun (fun helyen van)
    public int Happiness { get { return _happiness; } set { _happiness = value; } }

    private int _hunger; //ha magas akkor az jó
    public int Hunger { get { return _hunger; } set { _hunger = value; } }

    private int _thirst; //ha magas akkor az jó
    public int Thirst { get { return _thirst; } set { _thirst = value; } }

    private int _toilet; //ha magas akkor az jó
    public int Toilet { get { return _toilet; } set { _toilet = value; } }


    private List<BuildingTileObject> _destinations;
    
    private List<TileObject> _tileObjectsFromModel;
    private List<BuildingTileObject> _buildingTileObjectsFromModel;
    private GameTile[,] _tileGridFromModel;
    private int _mapSize;

    private System.Random _random;

    /// <summary>
    /// Variables initialized, destinations is filled with buildings
    /// </summary>
    /// <param name="x">X coordinate of visitor</param>
    /// <param name="y">Y coordinate of visitor</param>
    /// <param name="tileObjects">List of objects on the map</param>
    /// <param name="buildingTileObjects">List of buildings on the map</param>
    /// <param name="tileGrid">The model representation of the map, made out of gametiles</param>
    /// <param name="mapsize">Size of the map</param>
    /// <param name="random">Initialized random object</param>
    public Visitor(int x, int y, List<TileObject> tileObjects, List<BuildingTileObject> buildingTileObjects, GameTile[,] tileGrid, int mapsize, System.Random random)
    {
        _random = random;
        _pastPathDecisions = new BlackListOfTiles();
        _x = x;
        _y = y;
        _tileObjectsFromModel = tileObjects;
        _buildingTileObjectsFromModel = buildingTileObjects;
        _tileGridFromModel = tileGrid;
        _mapSize = mapsize;

        _atDest = false;
        _inQue = false;
        _inFun = false;
        _goingHome = false;
        _wentHome = false;
        
        _money = _random.Next(50+_buildingTileObjectsFromModel.Count * 50, 300+_buildingTileObjectsFromModel.Count*100);
        _moneyLimit = _random.Next(10+_buildingTileObjectsFromModel.Count*5, 10+_buildingTileObjectsFromModel.Count*20);

        //random tileobject-ek _destinations-be
        _destinations = new List<BuildingTileObject>();
        if (_tileObjectsFromModel.Count == 0)
        {
            _goingHome = true;
        }
        else
        {
            List<int> rand = new List<int>();
            while(rand.Count < _buildingTileObjectsFromModel.Count/2 + 1)
            {
                int num = _random.Next(0, _buildingTileObjectsFromModel.Count);
                if(!rand.Contains(num))
                {
                    rand.Add(num);
                }
            }

            foreach(int num in rand)
            {
                _destinations.Add(_buildingTileObjectsFromModel[num]);
            }
        }

        //statok
        _happiness = 100;
        _hunger = 100;
        _thirst = 100;
        _toilet = 100;
    }

    /// <summary>
    /// Makes visitor take a step towards its destination and makes them enter the destination building if they arrived
    /// Visitors follow a straight path until they arrive at crossroads or hit a dead end.
    /// They start going backwards at dead ends, and they choose their direction at crossroads based on the distance from the destination
    /// If they are going home 0,0 is their destination. When they arrived a variables is set to true and the model gets rid of them
    /// </summary>
    public bool Step()
    {
        if (!_goingHome && _destinations.Count == 0)
        {
            _goingHome = true;
            return false;
        }

        if (!_goingHome)
        {
            List<TileObject> nearbyBuildings = getNearbyBuildings();
            int ind = 0;
            while (ind < nearbyBuildings.Count && nearbyBuildings[ind] != _destinations[0])  // HA megérkezett, belép az épület queue-ba
            {
                ind++;
            }
            if (ind < nearbyBuildings.Count)
            {
                bool entered = ((BuildingTileObject)nearbyBuildings[ind]).EnterQue(this);
                _destinations.RemoveAt(0);
                _pastPathDecisions.clearList();
                removePastCords();
                if (entered)
                {
                    AtDest = true;

                }
            }
        }

        if (!AtDest)  //HA még nem érkezett meg, lép előre
        {
            List<TileObject> neighborPaths = getNeightborPaths();  // érvényes szomszédos utak
            if (neighborPaths.Count == 0) // megfordul
            {
                int tmpx = _x;
                int tmpy = _y;
                _x = _lastX;
                _y = _lastY;
                _lastX = tmpx;
                _lastY = tmpy;

                if (_goingHome && _x == 0 && _y == 0)
                {
                    _wentHome = true;
                }

                return true;
            }
            else if (neighborPaths.Count == 1) //egyenesen megy tovább
            {

                _lastX = _x;
                _lastY = _y;
                _x = neighborPaths[0].MiddlePos.x;
                _y = neighborPaths[0].MiddlePos.y;

                if (_goingHome && _x == 0 && _y == 0)
                {
                    _wentHome = true;
                }

                return true;

            }
            else  // kereszteződéshez ér
            {
                int indx;
                double dist;


                if (!_goingHome && _destinations.Count == 0)
                {
                    _goingHome = true;
                    return false;
                }
                dist = (double)Mathf.Sqrt((float)(_mapSize * _mapSize));
                indx = -1;

                double tmpDist;
                for (int i = 0; i < neighborPaths.Count; i++)  // abba az irányba fog lépni, ami a legközelebb van a célhoz
                {
                    if (_goingHome && neighborPaths[i].MiddlePos.x == 0 && neighborPaths[i].MiddlePos.y == 0)
                    {
                        _lastX = _x;
                        _lastY = _y;
                        _x = neighborPaths[indx].MiddlePos.x;
                        _y = neighborPaths[indx].MiddlePos.y;
                        _wentHome = true;
                        return true;
                    }

                    tmpDist = Visitor.CalculateDistance(neighborPaths[i].MiddlePos.x, neighborPaths[i].MiddlePos.y, _goingHome ? 0 : _destinations[0].MiddlePos.x, _goingHome ? 0 : _destinations[0].MiddlePos.y);
                    if ((tmpDist < dist && !(_pastPathDecisions.isInBlackList(neighborPaths[i]))))
                    {
                        dist = tmpDist;
                        indx = i;
                    }
                }
                if(indx == -1)
                {
                    if (_goingHome)
                        _wentHome = true;
                    else
                        pathNotFound();

                    return false;
                }
                
                _lastX = _x;
                _lastY = _y;
                _x = neighborPaths[indx].MiddlePos.x;
                _y = neighborPaths[indx].MiddlePos.y;
                _pastPathDecisions.addTileObject(neighborPaths[indx]);

                if (_goingHome && _x == 0 && _y == 0)
                {
                    _wentHome = true;
                }

                return true;
            }
        }


        return false; //lépett-e
    }

    /// <summary>
    /// Finds new destination based on the visitors stats. Destinations already in their list takes precedence
    /// </summary>
    public void RefreshDestinations()
    {
        if (_goingHome)
            return;

        if (_happiness <= 0)
        {
            GoHome();
            return;
        }

        int min = Math.Min(_happiness, Math.Min(_hunger, Math.Min(_thirst, _toilet)));
        Provides prov;

        if(_happiness <= min)
        {
            prov = Provides.Fun;
        }
        else if (_hunger <= min)
        {
            prov = Provides.Food;
        }
        else if(_thirst <= min)
        {
            prov = Provides.Drink;
        }
        else
        {
            prov = Provides.Toilet;
        }

        if(_destinations.Count == 0 || _destinations[0].Provides != prov)
        {
            int i = 1;
            BuildingTileObject dest;
            while(i < _destinations.Count)
            {
                if(_destinations[i].Provides == prov)
                {
                    dest = _destinations[i];
                    _destinations.RemoveAt(i);
                    setDestination(dest);
                }

                i++;
            }

            if(i >= _destinations.Count)
            {
                int rand;
                //try selecting random building 3 times
                for(int j = 0; j < 3; j++)
                {
                    rand = _random.Next(0, _buildingTileObjectsFromModel.Count);
                    if(!_destinations.Contains(_buildingTileObjectsFromModel[rand]) && _buildingTileObjectsFromModel[rand].Provides == prov)
                    {
                        setDestination(_buildingTileObjectsFromModel[rand]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// When a visitor enters a que some of its variables are altered with this function
    /// </summary>
    public void EnterQue()
    {
        _inQue = true;
        _inFun = false;
        _atDest = false;

        _lastTileWasPath = false;
    }

    /// <summary>
    /// When a visitor gets inside a building some of its variables are altered with this function
    /// </summary>
    public void Inside()
    {
        _inQue = false;
        _inFun = true;
        _atDest = false;
    }

    /// <summary>
    /// Returns whether a visitor can pay a price or not
    /// </summary>
    /// <param name="amount">Amount that needs to be payed</param>
    public bool CanPay(int amount)
    {
        if (amount > _money || amount > _moneyLimit)
        {
            _happiness--;
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Money is deducted from the visitor's wallet
    /// </summary>
    /// <param name="amount">Amount that needs to be payed</param>
    public int Pay(int amount)
    {
        _money -= amount;

        if (_money <= 0)
            _goingHome = true;

        return amount;
    }

    /// <summary>
    /// When a visitor gets kicked from a building some of its variables are altered with this function
    /// </summary>
    public void Kicked()
    {
        _inQue = false;
        _inFun = false;
        _atDest = false;

        if (_happiness <= 0 && !_goingHome)
            GoHome();

        RefreshDestinations();
    }

    /// <summary>
    /// Visitor starts going home
    /// </summary>
    public void GoHome()
    {
        _goingHome = true;
        _atDest = false;
        _destinations.Clear();
    }

    /// <summary>
    /// Calculates distance between coordinates
    /// </summary>
    /// <param name="fromx">X coordinate of current position</param>
    /// <param name="fromy">Y coordinate of current position</param>
    /// <param name="tox">X coordinate of destination</param>
    /// <param name="toy">Y coordinate of destination</param>
    public static double CalculateDistance(int fromx, int fromy, int tox, int toy)
    {
        return Math.Sqrt(Math.Pow((double)(tox - fromx), 2.0) + Math.Pow((double)(toy - fromy), 2.0));
    }

    /// <summary>
    /// Gets the paths next to the visitor which they are allowed to walk on (not the previous one)
    /// </summary>
    public List<TileObject> getNeightborPaths()
    {
        List<TileObject> tmpList = new List<TileObject>();
        if (_x + 1 < _mapSize && _x + 1 != _lastX && _tileGridFromModel[_x + 1, _y].TileObject is PathTileObject) tmpList.Add(_tileGridFromModel[_x + 1, _y].TileObject);
        if (_x - 1 >= 0 && _x - 1 != _lastX  && _tileGridFromModel[_x - 1, _y].TileObject is PathTileObject) tmpList.Add(_tileGridFromModel[_x - 1, _y].TileObject);
        if (_y + 1 < _mapSize && _y + 1 != _lastY && _tileGridFromModel[_x, _y+1].TileObject is PathTileObject) tmpList.Add(_tileGridFromModel[_x, _y+1].TileObject);
        if (_y - 1 >= 0 && _y - 1 != _lastY &&  _tileGridFromModel[_x, _y - 1].TileObject is PathTileObject) tmpList.Add(_tileGridFromModel[_x, _y - 1].TileObject);

        return tmpList;
        
    }

    /// <summary>
    /// Gets the buildings next to the visitor
    /// </summary>
    public List<TileObject> getNearbyBuildings()
    {
        List<TileObject> tmpList = new List<TileObject>();
        if (_x + 1 < _mapSize && _x + 1 != _lastX && _tileGridFromModel[_x + 1, _y].TileObject is BuildingTileObject) tmpList.Add(_tileGridFromModel[_x + 1, _y].TileObject);
        if (_x - 1 >= 0 && _x - 1 != _lastX && _tileGridFromModel[_x - 1, _y].TileObject is BuildingTileObject) tmpList.Add(_tileGridFromModel[_x - 1, _y].TileObject);
        if (_y + 1 < _mapSize && _y + 1 != _lastY && _tileGridFromModel[_x, _y + 1].TileObject is BuildingTileObject) tmpList.Add(_tileGridFromModel[_x, _y + 1].TileObject);
        if (_y - 1 >= 0 && _y - 1 != _lastY && _tileGridFromModel[_x, _y - 1].TileObject is BuildingTileObject) tmpList.Add(_tileGridFromModel[_x, _y - 1].TileObject);
        return tmpList;
    }

    /// <summary>
    /// Gets the decorations next to the visitor
    /// </summary>
    /// <returns></returns>
    public List<TileObject> getNearbyDecorations()
    {
        List<TileObject> tmpList = new List<TileObject>();
        if (_x + 1 < _mapSize && _x + 1 != _lastX && _tileGridFromModel[_x + 1, _y].TileObject is DecorationTileObject) tmpList.Add(_tileGridFromModel[_x + 1, _y].TileObject);
        if (_x - 1 >= 0 && _x - 1 != _lastX && _tileGridFromModel[_x - 1, _y].TileObject is DecorationTileObject) tmpList.Add(_tileGridFromModel[_x - 1, _y].TileObject);
        if (_y + 1 < _mapSize && _y + 1 != _lastY && _tileGridFromModel[_x, _y + 1].TileObject is DecorationTileObject) tmpList.Add(_tileGridFromModel[_x, _y + 1].TileObject);
        if (_y - 1 >= 0 && _y - 1 != _lastY && _tileGridFromModel[_x, _y - 1].TileObject is DecorationTileObject) tmpList.Add(_tileGridFromModel[_x, _y - 1].TileObject);
        return tmpList;
    }

    /// <summary>
    /// Put a new building to the front of the destination list
    /// </summary>
    /// <param name="bto">Next destination</param>
    public bool setDestination(BuildingTileObject bto)
    {
        if(_tileObjectsFromModel.Contains(bto))
        {
            _destinations.Insert(0, bto);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Last coords are reset
    /// </summary>
    public void removePastCords()  
    {
        _lastX = 0;
        _lastY = 0;
        
    }

    /// <summary>
    /// Called when there are no more paths to go on, destination is unreachable
    /// Next destination is the one after the removed destination, or 0,0 if the list is empty
    /// </summary>
    public void pathNotFound()
    {
        _destinations.RemoveAt(0);
        _pastPathDecisions.clearList();
        removePastCords();

        if (_destinations.Count == 0)
            GoHome();
    }
}
