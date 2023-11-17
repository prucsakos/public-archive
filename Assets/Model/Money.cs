using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// EVENT: MoneyChanged  
/// A pénzt kezeli az osztály.
/// Value property a jelenlegi összeg.
/// </summary>
public class Money
{
    public event EventHandler MoneyChanged;
    private int _money;
    private List<TileObject> _buildings;
    public int Value { get { return _money; } set { _money = value; } }
    
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="ListTileObject">Buildings</param>
    /// <param name="value">Starting balance</param>
    public Money( List<TileObject> ListTileObject, int value )
    {
        _buildings = ListTileObject;
        _money = value;
        OnMoneyChanged();
    }

    /// <summary>
    /// Subtracts upkeep cost from the player's money
    /// </summary>
    public void Update()
    {
        foreach (TileObject to in _buildings)
        {
            if (to is BuildingTileObject)
            {
                BuildingTileObject helper = (BuildingTileObject)to;
                if (helper.IsActive)
                {
                    _money -= to.Upkeep;
                }
            }
            /*else
            {
                _money -= to.Upkeep;
            }*/
        }
        OnMoneyChanged();

    }

    /// <summary>
    /// Ads to the balance
    /// </summary>
    /// <param name="value">Money amount</param>
    public void ReciveMoney(int value)
    {
        _money = _money + value;
        OnMoneyChanged();
    }

    /// <summary>
    /// Subtracts from the balance
    /// </summary>
    /// <param name="value">Money amount</param>
    public void PayMoney(int value)
    {
        _money = _money - value;
        OnMoneyChanged();
    }

    /// <summary>
    /// Set player's money to a specific amount
    /// </summary>
    /// <param name="value">Money amount</param>
    public void SetMoney(int value)
    {
        _money = value;
    }

    /// <summary>
    /// Event
    /// </summary>
    public void OnMoneyChanged()
    {
        if (MoneyChanged != null)
            MoneyChanged(this, EventArgs.Empty);
    }
}
