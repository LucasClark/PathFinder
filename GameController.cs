using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Lucas Clark u3170518
namespace PathFinderGame
{
    public class GameController : MonoBehaviour//, GameControllerInterface
    {
        private float speedUpPlayBack = 1;// the number of steps that are played at the same time to increase the speed of play back
        private bool startPlayback = false;// is playback running?
        private PathFinder pathFinder = new DijkstrasPathing();// a pathFinder interface by defult it is set to Dijkstras
        private List<Tile> playbackList;// holds the play back data sent from the algorithm 
        private bool nextStep = false;// used to stop the playback triggering again untill the timer has finished
        private bool newPlayBack = true;// tells weather or not to run a new path search
        private int playBackCounter = 0;
        public static bool Diagonal = false;
        public static int SearchType = 0;//Changes the search type for the Hueristic 0 manhaton. 1 diagonal. 2 Ecludian
        public static int NumberOfTilesSearched = 0;// set by the pathFinder interface shows the number of tiles searched to find an exit 
        [SerializeField] Text TileSearchedText;// the Ui element for the Input field 
        private bool instantPlayBack = false;//holds the value used for the instant play back feature
        [SerializeField] InputField playSpeedInput;
        public delegate void DisplayChanges();//the delegate used for the event
        public static event DisplayChanges displayChanges;


        public void SetSearchType(int searchType) // 0 manhaton. 1 diagonal. 2 Ecludian
        {
            SearchType = searchType;//Changes the search type for the Hueristic
        }


        public void ChangeDiagonal()//Changes weather or not the search algorithms are allowed to move diagonal
        {
            if (Diagonal == true)
                Diagonal = false;
            else
                Diagonal = true;
        }


        // to be used by Unity Ui. sets the pathFinder algorithm to be used to...
        public void SetPathFinder_AStar() { pathFinder = new AStarPathing(); }// A*
        public void SetPathFinder_Dijkstras() { pathFinder = new DijkstrasPathing(); }// Dijkstras
        public void SetPathFinder_BredthFirst() { pathFinder = new BredthFirstSearchPathing(); }//Bredth-First Search
        public void SetPathFinder_BestFirst() { pathFinder = new BestFirstSearch(); }// Best-Frist Search


        private void LateUpdate()// called during the render update frame. 
        {
            if (startPlayback == true)
            {
                DisplayChange();//calls the event and triggers all the tiles that have been subscribed to change to their set values
            }
            TileSearchedText.text = "Tiles Searched: " + NumberOfTilesSearched.ToString();//Sets the text at the bottom of the screen to display the search number
        }


        public void ChangePlayBackSpeed()// used by unity Ui to chnage the playBackSpeed value
        {
            if (int.Parse(playSpeedInput.textComponent.text) >= 1)// if the user value is a valid value use it. else set it to the default
            {
                speedUpPlayBack = int.Parse(playSpeedInput.textComponent.text);
            }
            else
            {
                playSpeedInput.textComponent.text = "1";
                speedUpPlayBack = 1;
            }
        }


        public void PlayBackInstantToggle()// Used by unity Ui to toggle the Boolean varible instantPlayBack
        {
            if (instantPlayBack == true)
                instantPlayBack = false;
            else
                instantPlayBack = true;
        }


        public void PlayBackToggle()//Used by Unity Ui to toggle the playBack feature
        {
            if (startPlayback == true)
            {
                startPlayback = false;
            }
            else
            {
                startPlayback = true;
                nextStep = false;
                PlayBackPath();//Calls PlayBack path to start recording play back.
            }
        }


        public void StopPlayBack()//Set all values used in playback to default and change all the tiles back to default state
        {
            nextStep = true;
            startPlayback = false;
            newPlayBack = true;
            playBackCounter = 0;
            //playbackList.Clear();//ensures that all the data is removed and not left for clean up by unity
            TileObject myTile;//cache data for better preformance.
            for (int X = 0; X < GridController.gridLength.x; X++)
            {
                for (int Y = 0; Y < GridController.gridLength.y; Y++)
                {
                    myTile = GridController.myTileObjectsRef[X][Y];
                    myTile.HasBeenChanged = false;
                    myTile.HasBeenChecked = false;
                    myTile.ChangeColor();
                }
            }
        }


        public void StartPathSearch()//uses the pathFinder interface and the user set values to find a path.
        {
            //the return from FindPath is a recording of the data changes made during path finding.
            playbackList = pathFinder.FindPath(GridController.myTileObjectsRef, GridController.startNode, GridController.exitNode, GridController.gridLength);
            TileObject myTile;//Cache for better preformance.
            for (int X = 0; X < GridController.gridLength.x; X++)
            {
                for (int Y = 0; Y < GridController.gridLength.y; Y++)
                {
                    myTile = GridController.myTileObjectsRef[X][Y];
                    myTile.HasBeenChanged = false;
                    myTile.HasBeenChecked = false;
                }
            }
        }


        void DisplayChange()//calls the diplayChanges event if it has subscribers
        {
            if (displayChanges != null)
            {
                displayChanges();
            }
        }


        public void PlayBackPath()// the main PlayBack function
        {
            if (startPlayback == true && nextStep == false)//if playback is running and its time to check for the next step...
            {
                if (newPlayBack == true)//if its a new play back....
                {
                    StopPlayBack();//make sure the old play back is stoped
                    StartPathSearch();//calculate a new path and get its recording data
                    newPlayBack = false;
                    nextStep = false;
                    startPlayback = true;
                    PlayBackPath();// go back to the start of this function
                }
                else
                {
                    if (playBackCounter < playbackList.Count)// if there is still recording data continue
                    {
                        if (instantPlayBack == true)// if instantPlayBack is on play through all the data and then display it
                        {
                            for (int i = 0; i < playbackList.Count; i++)
                                GridController.myTileObjectsRef[(int)playbackList[i].Position.x][(int)playbackList[i].Position.y].TileSetValues(playbackList[i]);
                            playBackCounter = playbackList.Count;//set the conut to max so it knows the recording has been finished
                            PlayBackPath();
                        }
                        else
                        {
                            StartCoroutine("DisplayFinding");
                        }
                        nextStep = true;
                    }
                    else
                    {
                        PaintPathBack(GridController.exitNode);// display the path that was found
                        nextStep = true;
                        startPlayback = false;
                        newPlayBack = true;
                        playBackCounter = 0;
                    }
                }
            }
        }


        void PaintPathBack(TileObject currentTile)//starting from the exit tile change the color of the tiles along the path 
        {
            if (currentTile.IsStart == true)// when you hit the start tile stop 
            {
                return;
            }
            else if (currentTile.IsExit == true)// if the exit tile has no path to it stop
            {
                if (currentTile.PreviouseTile != null)
                {
                    PaintPathBack(currentTile.PreviouseTile);
                }
            }
            else
            {
                currentTile.SetColor(Color.cyan);//change the color of the tiles on the path
                PaintPathBack(currentTile.PreviouseTile);// calls it self again
            }
        }



        static GameControllerDelegate changingTile;//cache data to improve preformance.
        IEnumerator DisplayFinding()//creates new delegate classes that hold the new values and the tile referance. then subscribs those classes to the display changes event
        {
            changingTile = new GameControllerDelegate();
            changingTile.Search(playbackList[playBackCounter]);
            playBackCounter += 1;
            for (int i = 0; i < speedUpPlayBack; i++)//changes the number of recored steps played at the same time
                if (playBackCounter < playbackList.Count)
                {
                    changingTile = new GameControllerDelegate();
                    changingTile.Search(playbackList[playBackCounter]);
                    playBackCounter += 1;
                }
            yield return new WaitForSeconds(0);
            nextStep = false;
            PlayBackPath();// calls play back path to continue playBack
        }
    }
}
