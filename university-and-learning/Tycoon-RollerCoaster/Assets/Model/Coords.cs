using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coords
{
    int _x;
    int _y;
    public int x { get { return _x; } set { _x = value; } }
    public int y { get { return _y; } set { _y = value; } }

    public Coords()
    {
        _x = 0;
        _y = 0;
    }

    public Coords(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public Coords(Coords coords)
    {
        _x = coords.x;
        _y = coords.y;
    }
}
