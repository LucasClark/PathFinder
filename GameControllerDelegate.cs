//Lucas Clark u3170518
namespace PathFinderGame
{
    public class GameControllerDelegate
    {
        Tile myTile;
        public void Search(Tile mine)//Takes tile data and stores it so when the event is called it can update to those values
        {
            myTile = mine;
            GameController.displayChanges += DisplayStuff;//subscribes to the static event displayChanges of GameController
        }

        void DisplayStuff()//Sets the tile it is attached to its new values and then unsubscribs it self 
        {
            GridController.myTileObjectsRef[(int)myTile.Position.x][(int)myTile.Position.y].TileSetValues(myTile);
            GameController.displayChanges -= DisplayStuff;
        }
    }
}
