using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile
{
    private TileObject _tileObject;
    private List<Visitor> _visitors;
    private int _x;
    private int _y;
    private bool _showTexture;
    
    public TileObject TileObject { get { return _tileObject; } set { _tileObject = value; } }
    public List<Visitor> Visitors { get { return _visitors; } }
    public int VisitorCount { get { return _visitors.Count; } }
    public bool ShowTexture { get { return _showTexture; } }

    public TileType TileType { get {
            if (_tileObject == null)
                return TileType.Illegal;
            else
                return _tileObject.TileType;
        } }

    /// <summary>
    /// Initialization, sets showTexture based on whether the tile is the middle tile of the tileobject
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="tileObject">TileObject inside the GameTile</param>
    public GameTile(int x, int y, TileObject tileObject = null)
    {
        _tileObject = tileObject;
        _visitors = new List<Visitor>();

        _x = x;
        _y = y;

        if(tileObject == null)
        {
            _showTexture = true;
        }
        else
        {
            Coords middlePos = tileObject.MiddlePos;
            if(_x == middlePos.x && _y == middlePos.y)
            {
                _showTexture = true;
            }
            else
            {
                _showTexture = false;
            }
        }
    }

    /// <summary>
    /// Gets rid of tileObject
    /// </summary>
    public void Reset()
    {
        _tileObject = null;
    }
}
