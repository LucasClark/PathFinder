using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lucas Clark u3170518
namespace PathFinderGame
{
    public class Tile 
    {
        public Vector2 Position;
        public bool IsWall;
        public bool HasBeenChecked;
        public bool HasBeenChanged;
        public bool IsExit;
        public bool IsStart;
        public TileObject PreviouseTile;
        public Vector2 NextTile;
        public float Cost;
        public float EstimatedCost;
        public float ActualCost()
        {
            return EstimatedCost + Cost;
        }
        public Color TileColor;
        public Tile(TileObject myTile)
        {
            Position = myTile.Position;
            IsWall = myTile.IsWall;
            HasBeenChecked = myTile.HasBeenChecked;
            HasBeenChanged = myTile.HasBeenChanged;
            IsExit = myTile.IsExit;
            IsStart = myTile.IsStart;
            PreviouseTile = myTile.PreviouseTile;
            NextTile = myTile.NextTile;
            Cost = myTile.Cost;
            TileColor = myTile.TileColor;
        }


    }
}
