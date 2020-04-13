//Adam Delo
//depth first search recursive backtracking algorithm for maze generation
//minimum value of 2 and maximum value of 20 displayed for this project

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawn_Script : MonoBehaviour
{
    //cell class for creation of cells and cell walls
    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject north;//1
        public GameObject east;//2
        public GameObject west;//3
        public GameObject south;//4
    }

    //wall size and position initialization
    private GameObject wall;
    private float wallLength = 1.0f;
    private Vector3 initialPos;

    //size of the maze x-axis and y-axis
    public static int xSize = 10;
    public static int ySize = 10;

    //display variables for game view
    public Material wallMaterial;
    public GameObject completeText;
    GameObject cube;
    public Text percent;
    private float percentComplete = 0f;
    public Slider[] sliders;
    public Button startButton;

    //gameobject for holding all maze cells
    private GameObject wallHolder;

    //array of cells within the maze
    private Cell[] cells;
    private int currentCell = 0;
    private int visitedCells = 0;
    private int totalCells = 0;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0; 

    //initializes the wall gameobject and sets all values to minimum values
    private void Start()
    {
        percent.text = "0%";

        completeText.SetActive(false);

        //initializes a wall object to be placed into the scene
        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.gameObject.transform.localScale = new Vector3(0.2f, 1, 1);
        wall.GetComponent<Renderer>().material = wallMaterial;

        cube = GameObject.Find("Cube");
        cube.SetActive(false);
    }

    //changes the size of the maze based on input from the xSlider and ySlider
    public void Change_xSize(float x)
    {
        xSize = (int)x;
    }
    public void Change_ySize(float y)
    {
        ySize = (int)y;
    }

    //initializes the base cube for construction of the maze
    public void InitializeCube()
    {
        cube.SetActive(true);
    }

    //starts the spawning of the maze by calling "CreateWalls" function on button click
    public void StartSpawn()
    {
        //set sliders and start button to not interactable so the maze can complete without errors
        sliders[0].interactable = !sliders[0].interactable;
        sliders[1].interactable = !sliders[1].interactable;
        startButton.interactable = !startButton.interactable;

        completeText.SetActive(false);
        cube.GetComponent<MeshRenderer>().enabled = true;
        //creates the starting walls of all of the tiles
        CreateWalls();
    }

    //creates the maze walls in a tile fashion (cells)
    //creates both vertial and horizontal walls on all cell locations (north, east, west, south)
    void CreateWalls()
    {
        //gameobject to hold entire maze in sample Hierarchy 
        wallHolder = new GameObject();
        wallHolder.name = "Maze";

        //sets the initial position of the walls and then assigns each wall location to the new position
        initialPos = new Vector3((-xSize / 2) + wallLength / 2, (-ySize / 2) + wallLength / 2);
        Vector3 myPos = initialPos;
        GameObject tempWall;

        //creates walls on the x-axis of the maze
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
                //creates offsets for the walls to align properly
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
                //spawns in the new wall
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //creates walls on the y-axis of the maze
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                //creates offsets for the walls to align properly
                myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
                //spawns in the new wall
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f,90.0f,0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //starts the creation of the cells
        CreateCells();
    }

    //creates the maze cells to finish construction of the base of the maze (tiles/cells)
    void CreateCells()
    {
        //clears the list of last cells accessed
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;

        //gameobject variable for all walls in the maze
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];

        //processing variables for construction of the cells
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        //gets all children of the maze wallHolder and sets it into its own variable of all the walls
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        //assigns walls to the cells array of cells
        for (int cellProcess = 0; cellProcess < cells.Length; cellProcess++)
        {
            if (termCount == xSize)
            {
                eastWestProcess++;
                termCount = 0;
            }

            //east and west processing of cells
            cells[cellProcess] = new Cell();
            cells[cellProcess].east = allWalls[eastWestProcess];
            cells[cellProcess].south = allWalls[childProcess + (xSize + 1) * ySize];
            
            eastWestProcess++;

            termCount++;
            childProcess++;
            cells[cellProcess].west = allWalls[eastWestProcess];
            cells[cellProcess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
        }

        //starts the creation of the maze
        CreateMaze();
    }

    //creates the entire maze and calls the function "BreakWall" and "SelectNextCell" to retreive which wall should be destroyed next
    //Depth First Search Algorithm (DFS) to generate the maze
    void CreateMaze()
    {
        //continues if the maze hasnt visited all cells yet
        if (visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                //checks for a random cell to create the next portion of the maze
                SelectNextCell();
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    //destroys the current wall and assigns that location to visited
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;

                    //calculation for total amount of cells processed so far (percentage)
                    percentComplete = (((totalCells - ((float)totalCells - (float)visitedCells)) / (float)totalCells) * 100f);

                    //outputs the amount completed in percentage
                    if (totalCells == visitedCells)
                    {
                        percent.text = "100%";
                    }
                    else
                    {
                        //rounds the percentage down to nearest int to allow for full 100% before completedText is turned active
                        percent.text = (Mathf.Floor(percentComplete).ToString() + "%");
                    }

                    //adds the current cell to previously accessed cells for use in recursive backtracking
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;

                //calculation for total amount of cells processed so far (percentage)
                percentComplete = (((totalCells - ((float)totalCells - (float)visitedCells)) / (float)totalCells) * 100f);

                //outputs the amount completed in percentage
                if (totalCells == visitedCells)
                {
                    percent.text = "100%";
                }
                else
                {
                    //rounds the percentage down to nearest int to allow for full 100% before completedText is turned active
                    percent.text = (Mathf.Floor(percentComplete).ToString() + "%");
                }
                
                //sets the variable startedBuilding to true to relap onto previous
                startedBuilding = true;
            }

            //uses the invoke method to show construction of the maze in real time
            //the invoke method slows down the process of creation, however I thought it looked nice so I kept it in for display purposes
            Invoke("CreateMaze", 0.0f);
        }
        else
        {
            //set sliders and start button to interactable to be able to change maze size once maze is complete
            sliders[0].interactable = !sliders[0].interactable;
            sliders[1].interactable = !sliders[1].interactable;
            startButton.interactable = !startButton.interactable;

            //triggers once the maze has been fully completed
            cube.GetComponent<MeshRenderer>().enabled = false;
            //prints once the maze is completed
            completeText.SetActive(true);    
        }
    }

    //selects the next cell to be processed in the destruction of the maze walls
    void SelectNextCell()
    {
        int length = 0;
        //selected cells and walls connecting to those cells in variables of arrays of ints
        int[] selectedCells = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        //modify the check variable
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;

        //northern cell
        if (currentCell + xSize < totalCells)
        {
            //if cell has not been visited set the value to 1
            if (cells[currentCell + xSize].visited == false)
            {
                selectedCells[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        //eastern cell
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            //if cell has not been visited set the value to 2
            if (cells[currentCell - 1].visited == false)
            {
                selectedCells[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        //western cell
        if (currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            //if cell has not been visited set the value to 3
            if (cells[currentCell + 1].visited == false)
            {
                selectedCells[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }
        
        //southern cell
        if (currentCell - xSize >= 0)
        {
            //if cell has not been visited set the value to 4
            if (cells[currentCell - xSize].visited == false)
            {
                selectedCells[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        //recursive backtracking algorithm to determine next cell to break walls
        if (length != 0)
        {
            int selectedCell = Random.Range(0, length);
            currentNeighbour = selectedCells[selectedCell];
            wallToBreak = connectingWall[selectedCell];
        }
        else
        {
            //backs up until there are no more remaining cells to back up to
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }

    //destroys the specified wall based on which direction the maze construction is going
    void BreakWall()
    {
        //destroys the walls of the current cell when creating the maze
        if (wallToBreak == 1)
        {
            //destroys the northern wall
            Destroy(cells[currentCell].north);
        }
        else if (wallToBreak == 2)
        {
            //destroys the eastern wall
            Destroy(cells[currentCell].east);
        }
        else if (wallToBreak == 3)
        {
            //destroys the western wall
            Destroy(cells[currentCell].west);
        }
        else if (wallToBreak == 4)
        {
            //destroys the southern wall
            Destroy(cells[currentCell].south);
        }
    }

    //sets all base values to 0 and false for starting to build
    //destroys the previous wallHolder variable to allow the next to be constructed
    public void Restart()
    {
        Destroy(wallHolder);
        visitedCells = 0;
        currentCell = 0;
        startedBuilding = false;
        currentNeighbour = 0;
        backingUp = 0;
        wallToBreak = 0;
    }
}