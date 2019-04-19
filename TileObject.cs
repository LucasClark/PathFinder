using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lucas Clark u3170518
namespace PathFinderGame
{
    public class TileObject : MonoBehaviour
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
        Renderer ren;
        private Color tileColor;
        public Color TileColor
        {
            get { return tileColor; }
            set
            {
                if (IsStart != true)
                    if (IsExit != true)
                    {
                        tileColor = value;
                    }
            }
        }
        private void Start()
        {
            ren = gameObject.GetComponent<Renderer>();
        }

        public void SetColor(Color myColor)
        {
            ren.material.color = myColor;
        }

        public void TileSetValues(Tile myTile)
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
            SetColor(myTile.TileColor);
        }

        public void ChangeColor()
        {
            if (IsStart == true)
            {
                tileColor = Color.green;
            }
            else if (IsExit == true)
            {
                tileColor = Color.red;
            }
            else if (IsWall == true)
            {
                tileColor = Color.black;
            }
            else if (HasBeenChecked == true)
            {
                tileColor = Color.grey;
            }
            else if (HasBeenChanged == true)
            {
                tileColor = Color.blue;
            }
            else
            {
                tileColor = Color.white;
            }
            SetColor(tileColor);
        }

    }
}
