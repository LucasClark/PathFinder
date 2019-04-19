using System.Collections.Generic;
using UnityEngine;
//Lucas Clark u3170518
namespace PathFinderGame
{
    public interface PathFinder//returns a Recorded list of actions 
    {
        List<Tile> FindPath(List<List<TileObject>> myList, TileObject startPoint, TileObject exitPoint, Vector2 gridLength);
    }
}
