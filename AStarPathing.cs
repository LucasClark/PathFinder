using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lucas Clark u3170518
namespace PathFinderGame
{
    public class AStarPathing : PathFinder
    {
        public List<Tile> FindPath(List<List<TileObject>> myList, TileObject startPoint, TileObject exitPoint, Vector2 gridLength)
        {
            //This seaction initalises the code used for the algorithm such as movement direction and heuristc
            int searchDirections;//is used to determine if you are allowed to move diagonal while searching for a path
            if (GameController.Diagonal == true)
                searchDirections = 8;//search all squares touching yours
            else
                searchDirections = 4;//search up, down, left, right

            int searchType = 4;// determines which directions the heuristic will check. same as above 
            bool isEcludian = false;
            switch (GameController.SearchType)// 0 manhaton. 1 diagonal. 2 Ecludian
            {
                case 0:
                    searchType = 4;
                    isEcludian = false;
                    break;
                case 1:
                    searchType = 8;
                    isEcludian = false;
                    break;
                case 2:
                    searchType = 8;
                    isEcludian = true;
                    break;
            }
            

            for (int X = 0; X < myList.Count; X++)//sets all the values to a high number as we canot store infinity in a float
                for (int Y = 0; Y < myList[X].Count; Y++)
                {
                    myList[X][Y].EstimatedCost = 999999;
                    myList[X][Y].HasBeenChanged = false;
                    myList[X][Y].HasBeenChecked = false;
                }


            List<TileObject> searchList = new List<TileObject>();//this is the open list which is used to store the current possible path options 
            myList[(int)exitPoint.Position.x][(int)exitPoint.Position.y].EstimatedCost = 0;// set the exit node estimated cost to 0 so we can use it as the start point for the heuristic 
            Vector2 cT = exitPoint.Position;
            int stopper = 0;
            int stop = 100000;//the maximuim limit the algorithm can cycle before it is stopped. This is put in to reduced the number of computer crashes due to bad logic
            int searchRef = 0;
            searchList.Add(myList[(int)exitPoint.Position.x][(int)exitPoint.Position.y]);
            float bakeMod = 1f;//can be used to adjust the amount of influence the heuristic has on path finding. make it lower to have it closer to a Dijkstras or higher makes it closer to Best first search. I have left it at 1 as it shows it in its best comparison for this application
            Vector2 tileToCheck = cT;
            TileObject tileBeingChecked;
            float moveCost = 1f;
            while (stopper < stop)//while there are unchecked nodes or while the stoppper is below its allowed limit
            {
                for (int i = 0; i < searchType; i++)// if its 4 it only checks up down left right. if its 8 it goes diagonal as well
                {
                    switch (i)//sets what tile to check next based on the position of the current tile. also sets the cost to move to that tile based to the searchtype settings
                    {
                        case 0:
                            tileToCheck = new Vector2(cT.x + 1, cT.y);//right
                            moveCost = 1;
                            break;
                        case 1:
                            tileToCheck = new Vector2(cT.x, cT.y + 1);//up
                            moveCost = 1;
                            break;
                        case 2:
                            tileToCheck = new Vector2(cT.x - 1, cT.y);//left
                            moveCost = 1;
                            break;
                        case 3:
                            tileToCheck = new Vector2(cT.x, cT.y - 1);//down
                            moveCost = 1;
                            break;
                        case 4:
                            tileToCheck = new Vector2(cT.x + 1, cT.y + 1);//up right
                            moveCost = 1.4f;
                            break;
                        case 5:
                            tileToCheck = new Vector2(cT.x + 1, cT.y - 1);//down Right
                            moveCost = 1.4f;
                            break;
                        case 6:
                            tileToCheck = new Vector2(cT.x - 1, cT.y - 1);//down left
                            moveCost = 1.4f;
                            break;
                        case 7:
                            tileToCheck = new Vector2(cT.x - 1, cT.y + 1);//up right
                            moveCost = 1.4f;
                            break;
                    }
                    if (isEcludian == true)// in an Ecludian Heuristic it is assumed that moving in any direction is the same cost and therefore the cost will allways be 1
                    {
                        moveCost = 1;
                    }
                    if (tileToCheck.x < gridLength.x && tileToCheck.x >= 0 && tileToCheck.y < gridLength.y && tileToCheck.y >= 0)// if the tile is not inside the grid skip it
                    {
                        tileBeingChecked = myList[(int)tileToCheck.x][(int)tileToCheck.y];
                            if (myList[(int)cT.x][(int)cT.y].EstimatedCost + (moveCost * bakeMod) < tileBeingChecked.EstimatedCost && tileBeingChecked.HasBeenChanged == false)//checks if the cost of the current tile is less than the current lowest cost to get to that tile
                        {
                                tileBeingChecked.EstimatedCost = myList[(int)cT.x][(int)cT.y].EstimatedCost + (moveCost * bakeMod);// sets the cost of the node by adding the movement cost to the tile from the current tile
                            tileBeingChecked.HasBeenChanged = true;
                                searchList.Add(tileBeingChecked);//adds the tileBeingChecked to the open list
                        }
                    }
                }
                searchRef = 0;
                for (int i = 0; i < searchList.Count; i++)//finds the next lowest tile to the Heuristic to set is value
                    if (searchList[i].EstimatedCost < searchList[searchRef].EstimatedCost)
                    {
                        searchRef = i;
                    }

                if (searchList.Count == 0)//once all tiles have been set break the while loop
                {
                    Debug.Log("No more options");
                    break;
                }
                cT = searchList[searchRef].Position;
                searchList.RemoveAt(searchRef);
            }
                Debug.Log("Set Path");




                List<Tile> changeLog = new List<Tile>();//Creates a new List to recorded the changes made by the algorithm
            for (int X = 0; X < myList.Count; X++)//sets all the values to a high number as we canot store infinity in a int
            {
                for (int Y = 0; Y < myList[X].Count; Y++)
                {
                    myList[X][Y].Cost = 999999;
                    myList[X][Y].HasBeenChanged = false;
                    myList[X][Y].HasBeenChecked = false;
                }
            }
            myList[(int)startPoint.Position.x][(int)startPoint.Position.y].Cost = 0;
            cT = startPoint.Position;
            stopper = 0;
            stop = 100000;
                searchList.Clear();
            searchList = new List<TileObject>();
            searchRef = 0;
            searchList.Add(myList[(int)startPoint.Position.x][(int)startPoint.Position.y]);
            while (stopper < stop)//while there are unchecked nodes or while the stoppper is below its allowed limit
            {
                for (int i = 0; i < searchDirections; i++)// if its 4 it only checks up down left right. if its 8 it goes diagonal as well
                {
                    switch (i)//sets what tile to check next based on the position of the current tile. also sets the cost to move to that tile based to the search direction setting
                    {
                        case 0:
                            tileToCheck = new Vector2(cT.x + 1, cT.y);//right
                            moveCost = 1;
                            break;
                        case 1:
                            tileToCheck = new Vector2(cT.x, cT.y + 1);//up
                            moveCost = 1;
                            break;
                        case 2:
                            tileToCheck = new Vector2(cT.x - 1, cT.y);//left
                            moveCost = 1;
                            break;
                        case 3:
                            tileToCheck = new Vector2(cT.x, cT.y - 1);//down
                            moveCost = 1;
                            break;
                        case 4:
                            tileToCheck = new Vector2(cT.x + 1, cT.y + 1);//up right
                            moveCost = 1.4f;
                            break;
                        case 5:
                            tileToCheck = new Vector2(cT.x + 1, cT.y - 1);//down Right
                            moveCost = 1.4f;
                            break;
                        case 6:
                            tileToCheck = new Vector2(cT.x - 1, cT.y - 1);//down left
                            moveCost = 1.4f;
                            break;
                        case 7:
                            tileToCheck = new Vector2(cT.x - 1, cT.y + 1);//up right
                            moveCost = 1.4f;
                            break;
                    }

                    if (tileToCheck.x < gridLength.x && tileToCheck.x >= 0 && tileToCheck.y < gridLength.y && tileToCheck.y >= 0)// if the tile is not inside the grid skip it
                    {
                        tileBeingChecked = myList[(int)tileToCheck.x][(int)tileToCheck.y];
                        if (tileBeingChecked.IsWall == false && tileBeingChecked.HasBeenChanged == false)
                        {
                            if (myList[(int)cT.x][(int)cT.y].Cost + moveCost < tileBeingChecked.Cost || tileBeingChecked.HasBeenChanged == false)//checks if the cost of the current tile is less than the current lowest cost to get to that tile
                            {
                                tileBeingChecked.PreviouseTile = myList[(int)cT.x][(int)cT.y];
                                tileBeingChecked.Cost = myList[(int)cT.x][(int)cT.y].Cost + moveCost;// sets the cost of the tile by adding the movement cost to the node from the current tile
                                tileBeingChecked.HasBeenChanged = true;
                                tileBeingChecked.TileColor = Color.blue;//changes the color that will be stored to blue as blue is the color used to represent tiles in the open set
                                searchList.Add(tileBeingChecked);//adds the tileBeingChecked to the open list
                                changeLog.Add(new Tile(tileBeingChecked));//adds the action to the change log
                            }
                        }
                    }
                }
                myList[(int)cT.x][(int)cT.y].TileColor = Color.grey;//changes the color of the tile to grey as that is the colour used to show that it has been checked
                changeLog.Add(new Tile(myList[(int)cT.x][(int)cT.y]));//adds the action to the change log
                searchRef = 0;//sets the referance to 0 so it starts at the first item in the open list
                for (int i = 0; i < searchList.Count; i++)
                    if (searchList[i].ActualCost() < searchList[searchRef].ActualCost())//if searchlist[i] has a lower actual cost then the current searchRef tile that tile is now the searchRef tile
                    {
                            searchRef = i;
                    }
                    else if (searchList[i].ActualCost() == searchList[searchRef].ActualCost())//if searchlist[i] has the same actual cost as current searchRef tile......
                    {
                        if (searchList[i].EstimatedCost < searchList[searchRef].EstimatedCost)//if that tile has a lower Heurstic cost then the current SearchRef tile that tile is now the searchRef tile
                            searchRef = i;
                    }

                if (searchList.Count == 0)//if the search list is empty break the while loop
                {
                    Debug.Log("No more options");
                    break;
                }
                if (searchList[searchRef].Position == exitPoint.Position)// if you found the exit break the while loop
                {
                    Debug.Log("Found Exit");
                    break;
                }
                cT = searchList[searchRef].Position;//set the lowest found seachRef to the current tile
                searchList.RemoveAt(searchRef);//Remove the current tile from the open set
                stopper += 1;//increment stopper 
            }
            Debug.Log("FoundPath");
            GameController.NumberOfTilesSearched = (changeLog.Count / 2);// the number of tiles checked is half the number of actions taken to find the tiles as each tile that is checked is first added to teh open set and then checked after
            return changeLog;//send the recorded chang log back to the pathfinder interface
        }
    }
}
