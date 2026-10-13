using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class ModelTests
{
    
    public void logvisitor(Visitor v)
    {
        Debug.Log("Visitor pos: X:" + v.X + ", Y:" + v.Y + "||" + v.AtDest);
    }
    
    [Test]
    public void Init()
    {
        GameModel gm = new GameModel(10);
        gm.SetupGrid();

        Assert.NotNull(gm);
        Assert.That(gm.getTileObject(5,5), Is.EqualTo(null));
        
    }

    [Test]
    public void BuildAndDestroy()
    {
        GameModel gm = new GameModel(20);
        gm.SetupGrid();

        gm.PathSelected();
        Assert.IsTrue(gm.Build(1, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(2, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(3, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(4, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(5, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(6, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(7, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(8, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(9, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 1));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 2));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 3));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 4));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 5));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 6));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 7));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(10, 8));
        

        gm.BuildingSelected(BuildingType.Drinks);
        Assert.IsFalse(gm.Build(0, 0));
        Assert.IsTrue(gm.Build(10, 10));

        Assert.NotNull(gm.getTileObject(9, 9));
        Assert.NotNull(gm.getTileObject(9, 10));
        Assert.NotNull(gm.getTileObject(9, 11));
        Assert.NotNull(gm.getTileObject(10, 9));
        Assert.NotNull(gm.getTileObject(10, 10));
        Assert.NotNull(gm.getTileObject(10, 11));
        Assert.NotNull(gm.getTileObject(11, 11));
        Assert.IsNull(gm.getTileObject(12, 12));
        Assert.IsNull(gm.getTileObject(8, 8));

        gm.BuildingSelected(BuildingType.Drinks);
        Assert.IsFalse(gm.Build(11, 11));

        gm.Destroy(11, 11);
        Assert.IsNull(gm.getTileObject(10,10));
        Assert.IsNull(gm.getTileObject(10, 10));
        Assert.IsNull(gm.getTileObject(9, 9));
        Assert.IsNull(gm.getTileObject(11, 11));

    }
    [Test]
    public void TileObjects()
    {
        GameModel gm = new GameModel(10);
        gm.SetupGrid();

        gm.PathSelected();
        gm.Build(0, 0);
        gm.PathSelected();
        gm.Build(1, 0);
        gm.PathSelected();
        gm.Build(2, 0);
        gm.PathSelected();
        gm.Build(3, 0);
        gm.PathSelected();
        gm.Build(3, 1);

        int cnt = gm.TileObjects.Count;
        gm.BuildingSelected(BuildingType.Drinks);
        TileObject t = gm.SelectedTileObject;
        gm.Build(3, 3);
        Assert.That(gm.TileObjects.Count, Is.EqualTo(cnt + 1));
        Assert.IsTrue(gm.TileObjects.Contains(t));
        Assert.That(gm.getTileObject(3, 3).MiddlePos.x, Is.EqualTo(3));
        Assert.That(gm.getTileObject(3, 3).MiddlePos.y, Is.EqualTo(3));
        gm.Destroy(3, 3);
        Assert.IsFalse(gm.TileObjects.Contains(t));
        Assert.That(gm.TileObjects.Count, Is.EqualTo(cnt));
    }
    
    [Test]
    public void MoneyTest()
    {
        GameModel gm = new GameModel(20);
        gm.SetupGrid();

        gm.PathSelected();
        gm.Build(0, 0);
        gm.PathSelected();
        gm.Build(1, 0);
        gm.PathSelected();
        gm.Build(2, 0);
        gm.PathSelected();
        gm.Build(3, 0);
        gm.PathSelected();
        gm.Build(3, 1);


        gm.MoneyObject.SetMoney(200);
        Assert.That(gm.Money, Is.EqualTo(200));
        gm.BuildingSelected(BuildingType.Test); // price 50, unkeep 10, 3x3
        int price = gm.SelectedTileObject.Price;
        
        gm.Build(3, 3);
        Assert.That(gm.Money, Is.EqualTo(200 - price));
        gm.MoneyObject.PayMoney(50);
        gm.MoneyObject.ReciveMoney(20);
        Assert.That(gm.Money, Is.EqualTo(200 - price - 30));
    }
    
    [Test]
    public void Pathfinding1()
    {
        GameModel gm = new GameModel(10);
        gm.SetupGrid();
        Assert.That(gm.getGameTile(0, 0).TileObject.TileType, Is.EqualTo(TileType.Path));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(1, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(2, 0));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(2, 1));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(2, 2));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(3, 2));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(4, 2));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(2, 3));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(2, 4));
        gm.PathSelected();
        Assert.IsTrue(gm.Build(1,2));
        gm.BuildingSelected(BuildingType.Restaurant);
        Assert.IsTrue(gm.Build(6, 2));
        gm.BuildingSelected(BuildingType.Toilet); //1x1
        gm.Build(3, 3);
        gm.secPassed((float)1000);


        gm.spawnVisitors();
        Assert.IsTrue(gm.Visitors[0].setDestination((BuildingTileObject)gm.getTileObject(6, 2)));
        Visitor Vis = gm.Visitors[0];

        Assert.IsFalse(Vis.AtDest);
        Assert.That(Vis.X, Is.EqualTo(0));
        Assert.That(Vis.Y, Is.EqualTo(0));
        Vis.Step();       
        Assert.That(Vis.X, Is.EqualTo(1));
        Assert.That(Vis.Y, Is.EqualTo(0));
        Vis.Step();
        Assert.That(Vis.X, Is.EqualTo(2));
        Assert.That(Vis.Y, Is.EqualTo(0));
        Vis.Step();
        Assert.That(Vis.X, Is.EqualTo(2));
        Assert.That(Vis.Y, Is.EqualTo(1));
        Vis.Step();
        Assert.That(Vis.X, Is.EqualTo(2));
        Assert.That(Vis.Y, Is.EqualTo(2));
        Assert.That(Vis.getNeightborPaths().Count, Is.EqualTo(3));

        Vis.Step();
        Assert.That(Vis.X, Is.EqualTo(3));
        Assert.That(Vis.Y, Is.EqualTo(2));
        Assert.That(Vis.getNearbyBuildings().Count, Is.EqualTo(1));
        Vis.Step();
        Assert.That(Vis.X, Is.EqualTo(4));
        Assert.That(Vis.Y, Is.EqualTo(2));
        Vis.Step();
        Assert.That(Vis.X, Is.EqualTo(4));
        Assert.That(Vis.Y, Is.EqualTo(2));

        Assert.IsTrue(Vis.AtDest);


    }
    [Test]
    public void GameModelFunctions()
    {
        GameModel gm = new GameModel(10);
        gm.SetupGrid();

        gm.PathSelected();
        gm.Build(1, 0);
        gm.PathSelected();
        gm.Build(2, 0);
        gm.PathSelected();
        gm.Build(3, 0);
        gm.PathSelected();
        gm.Build(4, 0);
        gm.PathSelected();
        gm.Build(5, 0);
        gm.PathSelected();
        gm.Build(5, 1);
        gm.PathSelected();
        gm.Build(5, 2);
        gm.PathSelected();
        gm.Build(5, 3);

        gm.BuildingSelected(BuildingType.Buffet);
        TileObject to = gm.SelectedTileObject;
        gm.Build(5, 5);
        

        Assert.That(gm.getTileType(5, 5), Is.EqualTo(TileType.Building));
        Assert.That(gm.getTileObject(5, 5), Is.EqualTo(to));
        

        gm.PathSelected();
        gm.Build(0, 0);
        gm.spawnVisitors();
        gm.spawnVisitors();
        gm.spawnVisitors();
        Assert.That(gm.getVisitorCount(0, 0), Is.EqualTo(3));

        
    }

    [Test]
    public void VisitorUpdate()
    {
        GameModel gm = new GameModel(10);
        gm.SetupGrid();

        gm.PathSelected();
        gm.Build(0, 0);
        gm.PathSelected();
        gm.Build(0, 1);
        gm.PathSelected();
        gm.Build(0, 2);
        gm.BuildingSelected(BuildingType.Buffet);
        gm.Build(2, 3);

        gm.spawnVisitors();
        Visitor Vis = gm.Visitors[0];
        BuildingTileObject bto = (BuildingTileObject)gm.getTileObject(2, 3);
        Vis.setDestination(bto);
        Assert.That(Vis.X, Is.EqualTo(0));
        Assert.That(Vis.Y, Is.EqualTo(0));

        gm.UpdateVisitors();
        Assert.That(Vis.X, Is.EqualTo(0));
        Assert.That(Vis.Y, Is.EqualTo(1));
        gm.UpdateVisitors();
        Assert.That(Vis.X, Is.EqualTo(0));
        Assert.That(Vis.Y, Is.EqualTo(2));
        
        Assert.IsFalse(bto.IsActive);
        gm.secPassed(bto.BuildTime + (float)50);
        Assert.IsTrue(bto.IsActive);
        gm.UpdateVisitors();
        Assert.IsTrue(Vis.AtDest);

    }
    
}
