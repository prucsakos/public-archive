using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores the path decisions of Visitors. 
/// </summary>
public class BlackListOfTiles
{
    private List<TileObject> _blackList;

    public BlackListOfTiles()
    {
        _blackList = new List<TileObject>();
    }
    /// <summary>
    /// Clear the decision list.
    /// </summary>
    public void clearList()
    {
        _blackList.Clear();
    }
    /// <summary>
    /// Checks if the decision list contains the given TileObject.
    /// </summary>
    /// <param name="to">TileObject</param>
    /// <returns></returns>
    public bool isInBlackList(TileObject to)
    {
        if (_blackList.Contains(to)) return true;
        return false;
    }
    /// <summary>
    /// Adds a new TileObject to the decision list.
    /// </summary>
    /// <param name="to">TileObject</param>
    public void addTileObject(TileObject to)
    {
        if (!isInBlackList(to))
        {
            _blackList.Add(to);
        }
    }
}
