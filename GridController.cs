using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Lucas Clark u3170518
namespace PathFinderGame
{
    public class GridController : MonoBehaviour
    {
        public static TileObject startNode;// the current start node. always null until defined by user
        public static TileObject exitNode;// the current exit node. always null until defined by user
        [SerializeField] InputField gridXLength;// input field from the Ui that is used for entering the grids new X length
        [SerializeField] InputField gridYLength;// input field from the Ui that is used for entering the grids new Y length
        public static Vector2 gridLength = new Vector2(25, 50);// the umber of tiles along the grids X and Y
        float screenWidth = 17;// The number of normal sized tiles that cna fit across the screen width
        float screenHeight = 8;// The number of normal sized tiles that cna fit across the screen height
        private bool paintWallsLock;// Is used to stop the user from being able to paint and edit walls while menues are open and other functions are running
        private bool paintEraseWalls = true;
        [SerializeField] GameObject tile;
        private float tileSize = 1;
        private float gridScale;
        public static List<List<GameObject>> myTileObjects = new List<List<GameObject>>();
        public static List<List<TileObject>> myTileObjectsRef = new List<List<TileObject>>();
        private bool changeStart;
        private bool changeExit;

        public void PaintWallsLock(bool inPut)// making a bool function allows it to be attached and used by Unity Ui elements
        {
            paintWallsLock = inPut;
        }

        private void Start()// changes the grid scale and then spawns tiles based on the default gridLength of X = 50 Y = 100
        {
            ScaleGrid();
            SpawnTiles();
        }

        private void FixedUpdate()//The physics update cycle inside unity....
        {
            if (Input.GetButton("Fire1"))
            {
                if (paintWallsLock == false)
                {
                    PaintWalls();//because PaintWalls uses rayCast's its most optimised calling it inside the physics update loop
                }
            }
        }

        public void SetGridSize()// Allows the ChangeGridSize function to be controlled by Unity Ui elements
        {
            // using the Ui text elements from gridXLength and gridYLength which are inputFields when this is called it set the new size of the grid to those values
            ChangeGridSize(int.Parse(gridYLength.textComponent.text), int.Parse(gridXLength.textComponent.text));
        }

        void ChangeGridSize(int changeX, int changeY)//Clears the old grid and then generates a new one
        {
            foreach (List<GameObject> tileList in myTileObjects)
                foreach (GameObject tileObj in tileList)
                    Destroy(tileObj);

            myTileObjects.Clear();//clears all values from the lists to stop data leaks
            myTileObjectsRef.Clear();
            myTileObjects = new List<List<GameObject>>();
            myTileObjectsRef = new List<List<TileObject>>();
            gridLength.x = changeX;
            gridLength.y = changeY;
            SpawnTiles();
            ScaleGrid();
        }

        void ScaleGrid()//changes the sclae of the grid based on the width and height of the screen
        {
            if ((screenHeight / (gridLength.x * tileSize)) < (screenWidth / (gridLength.y * tileSize)))
                gridScale = screenHeight / (gridLength.x * tileSize);
            else
                gridScale = screenWidth / (gridLength.y * tileSize);

            gameObject.transform.localScale = new Vector3(gridScale, gridScale, gridScale);
        }

        public void PaintOrErase()// A Toggle for the Paintwalls feature Ui element switching between making and erasing walls
        {
            if (paintEraseWalls == true)
            {
                paintEraseWalls = false;
            }
            else
                paintEraseWalls = true;
        }

        public void SetStartPoint()
        {
            changeStart = true;
            changeExit = false;
        }

        public void SetExitPoint()
        {
            changeExit = true;
            changeStart = false;
        }

        public void ClearGrid()//sets the values of all the tiles on the current grid back to defalut 
        {
            TileObject myTile;// cache myTile for use in this function
            for (int X = 0; X < gridLength.x; X++)
            {
                for (int Y = 0; Y < gridLength.y; Y++)
                {
                    myTile = myTileObjectsRef[X][Y];
                    myTile.IsWall = false;
                    myTile.HasBeenChanged = false;
                    myTile.HasBeenChecked = false;
                    myTile.ChangeColor();

                }
            }
        }


        [SerializeField] InputField brushSizeInput; // the Ui element InputField used for changing brush size
        // This function is to be used by Unity Ui elements 
        public void ChangeBrushSize()//Lets the user change the size of the brush using an area around the initial raycast
        {
            if (float.Parse(brushSizeInput.textComponent.text) >= 0.001f)
            {
                PaintRadius = float.Parse(brushSizeInput.textComponent.text);
            }
            else
            {
                brushSizeInput.textComponent.text = "0.001";
                PaintRadius = 0.001f;
            }
        }
        //Chace data used for the paint walls function.
        static RaycastHit hit;
        static Ray ray;
        static TileObject myTileToPaint;
        static float PaintRadius = 0.1f;
        void PaintWalls()// makes or erases walls based on the position of the users mouse cursor 
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);// sets the ray start and rotatoin in 3D space to the forward direction of the mouse position on the camera plane 
            if (Physics.Raycast(ray, out hit, 10000.0f))
            {
                foreach (Collider item in Physics.OverlapSphere(hit.transform.position, PaintRadius))
                {
                    myTileToPaint = item.transform.GetComponent<TileObject>();
                    if (myTileToPaint.IsStart == true || myTileToPaint.IsExit == true)//stops you bieng able to paint over the start and exit tiles
                        return;

                    if (changeStart == true)// changes the start node to where the user clicks 
                    {
                        if (startNode != null)// changes the prevous start node to its default state if there is one
                        {
                            startNode.IsStart = false;
                            startNode.ChangeColor();
                        }
                        startNode = myTileToPaint;// sets the current node to the new start node
                        myTileToPaint.IsStart = true;
                        myTileToPaint.ChangeColor();
                        changeStart = false;
                        return;
                    }
                    else if (changeExit == true)// changes the exit node to where the user clicks
                    {
                        if (exitNode != null)// changes the prevous exit node to its default state if there is one
                        {
                            exitNode.IsExit = false;
                            exitNode.ChangeColor();
                        }
                        exitNode = myTileToPaint;// sets the current node to the new exit node
                        myTileToPaint.IsExit = true;
                        myTileToPaint.ChangeColor();
                        changeExit = false;
                        return;
                    }
                    else if (paintEraseWalls == true)// if paintWalls is true make the tile a wall
                    {
                        myTileToPaint.IsWall = true;
                        myTileToPaint.ChangeColor();
                    }
                    else if (paintEraseWalls == false)// if paintWalls is false change the tiles to default
                    {
                        myTileToPaint.IsWall = false;
                        myTileToPaint.ChangeColor();
                    }
                }
            }
        }

        void SpawnTiles()//Generates a grid of tiles based on the sizes defined by gridLength
        {
            GameObject tempTile;// Cache data for temp tile
            for (int X = 0; X < gridLength.x; X++)
            {
                myTileObjects.Add(new List<GameObject>());
                myTileObjectsRef.Add(new List<TileObject>());
                for (int Y = 0; Y < gridLength.y; Y++)
                {
                    tempTile = Instantiate(tile);
                    tempTile.name = (X.ToString() + Y.ToString());
                    tempTile.GetComponent<TileObject>().Position = new Vector2(X, Y);
                    tempTile.transform.SetParent(gameObject.transform);
                    tempTile.transform.localPosition = new Vector3(X * tileSize, Y * tileSize, 0);
                    tempTile.transform.localScale = new Vector3(1, 1, 1);
                    myTileObjects[X].Add(tempTile);
                    myTileObjectsRef[X].Add(myTileObjects[X][Y].GetComponent<TileObject>());
                }
            }
        }

        void SpawnTiles(int newX, int newY)//cahnges the grid size based on the new values compared to the current values
        {
            if (newX > gridLength.x || newY > gridLength.y)//if the grid is bigger than the current grid
            {
                GameObject tempTile;// cache data.
                for (int X = (int)gridLength.x; X < newX; X++)
                {
                    if (newX > gridLength.x)// if the X size of the new grid is bigger create a new X List
                    {
                        myTileObjects.Add(new List<GameObject>());
                    }
                    for (int Y = (int)gridLength.y; Y < newY; Y++)
                    {
                        tempTile = Instantiate(tile);
                        tempTile.name = (X.ToString() + Y.ToString());
                        tempTile.GetComponent<TileObject>().Position = new Vector2(X, Y);
                        tempTile.transform.SetParent(gameObject.transform);
                        tempTile.transform.localPosition = new Vector3(X * tileSize, Y * tileSize, 0);
                        tempTile.transform.localScale = new Vector3(1, 1, 1);
                        myTileObjects[X].Add(tempTile);
                        myTileObjectsRef[X].Add(myTileObjects[X][Y].GetComponent<TileObject>());
                    }
                }
            }
            if (newY < gridLength.y)// if the new grid size has a smaller Y value delete tiles fromt the current grid to match the new size
            {
                for (int X = (int)gridLength.x; X > newX; X--)
                    for (int Y = (int)gridLength.y; Y > newY; Y--)
                    {
                        myTileObjectsRef[X].RemoveAt(Y);
                        Destroy(myTileObjects[X][Y].gameObject);
                        myTileObjects[X].RemoveAt(Y);
                    }
            }
            if (newX < gridLength.x)// if the new grid size has a smaller X value delete tiles fromt the current grid to match the new size
            {
                for (int X = (int)gridLength.x; X > newX; X--)
                    if (newX > gridLength.x)
                    {
                        myTileObjectsRef.RemoveAt(X);
                        myTileObjects.RemoveAt(X);
                    }
            }
        }
    }
}