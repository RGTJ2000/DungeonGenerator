using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] GameObject grid_prefab;
    [SerializeField] int grid_halfWidth;
    [SerializeField] int grid_halfHeight;
    [SerializeField] Material grid_greenMat;
    [SerializeField] Material grid_blackMat;
    [SerializeField] Material grid_whiteMat;
    [SerializeField] Material grid_greyMat;

    [SerializeField] int max_numberOfNodes = 50;
    //[SerializeField] int current_node;
    [SerializeField] int current_gen;
    [SerializeField] int mainPath_nodeCount = 0;

    [SerializeField] WallsOffset default_wallsOffset = new WallsOffset(1, 1, 1, 1);
    [SerializeField] HallStats default_hallStats = new HallStats(4, 3, 0, false);
    [SerializeField] HallStats zeroed_hallStats = new HallStats(0, 0, 0, false);

    [SerializeField] private int hall_maxLength;
    [SerializeField] private int hall_minWidth;
    [SerializeField] private int hall_maxWidth;
    [SerializeField] private int max_roomHeight;
    [SerializeField] private int max_roomWidth;

    private hallProbability neutral_hProb;
    [SerializeField] private hallProbability sw_hprob;
    [SerializeField] private float room_prob = 0.5f;  //higher probablity makes more rooms.

    private int gridWidth;
    private int gridHeight;
    private Vector2 origin_position;

    private NodeData[] node_array;
    private cellData[,] cellMatrix;

    GameObject gridContainer;


    private enum cellType
    {
        undef,
        node,
        wall,
        hall

    }

    private struct cellData
    {
        public cellType type;
        public GameObject cell_obj;

        public cellData(cellType type, GameObject obj)
        {
            this.type = type;
            this.cell_obj = obj;
        }
    }

    [System.Serializable]
    private struct hallProbability
    {
        public int northProb;
        public int southProb;
        public int eastProb;
        public int westProb;

        public hallProbability(int n, int s, int e, int w)
        {
            this.northProb = n;
            this.southProb = s;
            this.eastProb = e;
            this.westProb = w;
        }
    }

    private TextMeshPro _textMeshPro;

    private InputControls _inputControls;

    private int endNodeIndex = 0; //the first empty slot in the array

    private void Awake()
    {
        _inputControls = new InputControls();


    }

    private void OnEnable()
    {
        _inputControls.General.Enable();

        _inputControls.General.NodeSize.started += OnNodeSize;

        _inputControls.General.Refresh.started += OnRefresh;

    }

    private void OnDisable()
    {
        _inputControls.General.Disable();

        _inputControls.General.NodeSize.started -= OnNodeSize;
        _inputControls.General.Refresh.started -= OnRefresh;
    }

    private void OnNodeSize(InputAction.CallbackContext context)
    {

        float newValue = context.ReadValue<float>();

        int n = node_array[0].walls_offset.north;
        int s = node_array[0].walls_offset.south;
        int e = node_array[0].walls_offset.east;
        int w = node_array[0].walls_offset.west;

        // Positive button pressed (from 0 to 1)
        if (newValue == 1f)
        {
            //Debug.Log("Positive button pressed - Increasing node size");


            node_array[0].walls_offset = new WallsOffset(n + 1, s + 1, e + 1, w + 1);
        }
        // Negative button pressed (from 0 to -1)
        else if (newValue == -1f)
        {
            //Debug.Log("Negative button pressed - Decreasing node size");
            node_array[0].walls_offset = new WallsOffset(n - 1, s - 1, e - 1, w - 1);

        }

        ReevaluateOtherNodePositions(0);

        DestroyGridObjects();
        ResetCellMatrix();

        InstantiateAllNodes();

        //ResetGrid();
        //UpdateGridData();
        //RedrawGrid();

    }
    void Start()
    {
        gridContainer = new GameObject("GridContainer");

        node_array = new NodeData[max_numberOfNodes];
        endNodeIndex = 0;
        current_gen = 0;

        gridWidth = grid_halfWidth * 2;
        gridHeight = grid_halfHeight * 2;

        origin_position = new Vector2(grid_halfWidth, grid_halfHeight);

        cellMatrix = new cellData[gridWidth, gridHeight];

        neutral_hProb = new hallProbability(1, 1, 1, 1);
        sw_hprob = new hallProbability(1, 3, 3, 1);

        //InitializeGrid();

        CreateMainPath();

        InstantiateAllNodes();

        //AddNodesToParentHalls(0);



        //UpdateGridData();

        //RedrawGrid();






    }

    private void CreateMainPath()
    {

        for (int i = 0; i < mainPath_nodeCount; i++)
        {

            if (i == 0)
            {
                CreateNode(i - 1, Vector2.zero, current_gen);

                ExpandNodeAtRandom(i);

                AddRandomHallsToNode(i, sw_hprob, 1);
            }
            else if (i == mainPath_nodeCount - 1)
            {
                AddNodesToParentHalls(i - 1);
                ExpandNodeAtRandom(i);

            }
            else
            {
                AddNodesToParentHalls(i - 1);
                ExpandNodeAtRandom(i);
                AddRandomHallsToNode(i, sw_hprob, 1);
            }



        }
        Debug.Log("End Node Index is now " + endNodeIndex);


    }

    private void ExpandNodeAtRandom(int nodeIndex)
    {
        if (Random.value < room_prob)
        {
            //expand room
            int height = Random.Range(default_wallsOffset.north + default_wallsOffset.south + 3, max_roomHeight);
            int width = Random.Range(default_wallsOffset.east+default_wallsOffset.west +3, max_roomWidth);

            node_array[nodeIndex].walls_offset = new WallsOffset( height/2, height/2, width/2, width/2 );
        }

    }

    void OnRefresh(InputAction.CallbackContext context)
    {
        DestroyGridObjects();
        ResetCellMatrix();
        endNodeIndex = 0;
        CreateMainPath();

        InstantiateAllNodes();
    }

    private void InstantiateAllNodes()
    {
        for (int i = 0; i < endNodeIndex; i++)
        {
            InstantiateNodeLayout(i);
        }

    }

    void AddRandomHallsToNode(int nodeIndex, hallProbability h_prob, int maxHallCheck)
    {
        NodeData nodeData = node_array[nodeIndex];

        List<string> directionsList = new List<string>();

        //get available directions to add
        if (nodeData.halls_data.north.length == 0)
        {
            directionsList.Add("north");
        }
        if (nodeData.halls_data.south.length == 0)
        {
            directionsList.Add("south");
        }
        if (nodeData.halls_data.east.length == 0)
        {
            directionsList.Add("east");
        }
        if (nodeData.halls_data.west.length == 0)
        {
            directionsList.Add("west");
        }

        Debug.Log("Node " + nodeIndex + ": Available directions" + string.Join(", ", directionsList));

        List<string> probPool = new List<string>();

        foreach (string direction in directionsList)
        {
            int fillCount;

            switch (direction)
            {
                case "north":
                    fillCount = h_prob.northProb;
                    break;
                case "south":
                    fillCount = h_prob.southProb;
                    break;
                case "east":
                    fillCount = h_prob.eastProb;
                    break;
                case "west":
                    fillCount = h_prob.westProb;
                    break;
                default:
                    fillCount = 0;
                    break;

            }

            for (int i = 0; i < fillCount; i++)
            {
                probPool.Add(direction);
            }

        }

        //choose random hallway direction from list
        if (probPool.Count > 0)
        {
            string randomDirection = probPool[Random.Range(0, probPool.Count)];

            AddHallToNode(nodeIndex, randomDirection);
        }
        else
        {
            Debug.Log("No available directions found.");
        }



    }


    private void AddHallToNode(int nodeIndex, string direction)
    {
        int length = Random.Range(1, hall_maxLength + 1);

        int maxWidth = hall_maxWidth;
        /*
        int maxWidth;

        switch (direction)
        {
            case "north":
                maxWidth = node_array[nodeIndex].walls_offset.east + node_array[nodeIndex].walls_offset.west + 1;
                break;
            case "south":
                maxWidth = node_array[nodeIndex].walls_offset.east + node_array[nodeIndex].walls_offset.west + 1;
                break;
            case "east":
                maxWidth = node_array[nodeIndex].walls_offset.north + node_array[nodeIndex].walls_offset.south + 1;
                break;
            case "west":
                maxWidth = node_array[nodeIndex].walls_offset.north + node_array[nodeIndex].walls_offset.south + 1;
                break;
            default:
                maxWidth = hall_minWidth;
                break;


        }
        */

        int width = Random.Range(hall_minWidth, maxWidth + 1);

        int offset = Random.Range(-(maxWidth - width) / 2, ((maxWidth - width) / 2) + 1);

        HallStats hallStats = new HallStats(length, width, offset, false);

        switch (direction)
        {
            case "north":
                node_array[nodeIndex].halls_data.north = hallStats;
                break;
            case "south":
                node_array[nodeIndex].halls_data.south = hallStats;
                break;
            case "east":
                node_array[nodeIndex].halls_data.east = hallStats;
                break;
            case "west":
                node_array[nodeIndex].halls_data.west = hallStats;
                break;
            default:

                break;


        }
    }

    void AddNodesToParentHalls(int parent_index)
    {
        Vector2[] hallways = GetOpenHallwayPositions(parent_index);

        for (int i = 0; i < hallways.Length; i++)
        {
            if (hallways[i] != Vector2.zero)
            {
                AddNodeAtHallOffset(parent_index, hallways[i], i);
            }



        }
    }



    private void AddNodeAtHallOffset(int parent_index, Vector2 offset, int hall_index)
    {

        int createdNodeIndex = CreateNode(parent_index, offset, current_gen);

        HallStats entryHall;

        switch (hall_index)  //update parent node connection boolean AND add hallway entrance to created node
        {
            case 0:
                node_array[parent_index].halls_data.north.isConnected = true;

                entryHall = new HallStats(1, node_array[parent_index].halls_data.north.width, -node_array[parent_index].halls_data.north.offsetFromNodeCenter, true);

                node_array[createdNodeIndex].halls_data.south = entryHall;

                break;
            case 1:
                node_array[parent_index].halls_data.south.isConnected = true;

                entryHall = new HallStats(1, node_array[parent_index].halls_data.south.width, -node_array[parent_index].halls_data.south.offsetFromNodeCenter, true);

                node_array[createdNodeIndex].halls_data.north = entryHall;

                break;
            case 2:
                node_array[parent_index].halls_data.east.isConnected = true;

                entryHall = new HallStats(1, node_array[parent_index].halls_data.east.width, -node_array[parent_index].halls_data.east.offsetFromNodeCenter, true);

                node_array[createdNodeIndex].halls_data.west = entryHall;

                break;
            case 3:
                node_array[parent_index].halls_data.west.isConnected = true;

                entryHall = new HallStats(1, node_array[parent_index].halls_data.west.width, -node_array[parent_index].halls_data.west.offsetFromNodeCenter, true);

                node_array[createdNodeIndex].halls_data.east = entryHall;
                break;
            default:
                break;

        }

        //Debug.Log("Added node to hall index:" + hall_index);

    }

    private void Update()
    {

    }

    private Vector2[] GetOpenHallwayPositions(int nodeIndex)
    {
        Vector2[] positionsArray = new Vector2[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };

        NodeData nodeData = node_array[nodeIndex];

        if (nodeData.halls_data.north.length > 0 && !nodeData.halls_data.north.isConnected)
        {
            int offset_y = nodeData.walls_offset.north + nodeData.halls_data.north.length + default_wallsOffset.south + 2; //the +2 accounts for the wall cell and moving to the actual node position
            int offset_x = nodeData.halls_data.north.offsetFromNodeCenter;
            Vector2 newNodeOffset = new Vector2(offset_x, offset_y);

            //positionsArray[0] = nodeData.offsetFromParentNode + GetParentNodePosition(nodeIndex) + newNodeOffset;
            positionsArray[0] = newNodeOffset;
        }


        if (nodeData.halls_data.south.length > 0 && !nodeData.halls_data.south.isConnected)
        {
            int offset_y = nodeData.walls_offset.south + nodeData.halls_data.south.length + default_wallsOffset.north + 2; //the +2 accounts for the wall cell and moving to the actual node position
            int offset_x = nodeData.halls_data.south.offsetFromNodeCenter;
            Vector2 newNodeOffset = new Vector2(offset_x, -offset_y);

            //positionsArray[1] = nodeData.offsetFromParentNode + GetParentNodePosition(nodeIndex) + newNodeOffset;
            positionsArray[1] = newNodeOffset;

        }

        if (nodeData.halls_data.east.length > 0 && !nodeData.halls_data.east.isConnected)
        {
            int offset_x = nodeData.walls_offset.east + nodeData.halls_data.east.length + default_wallsOffset.west + 2; //the +2 accounts for the wall cell and moving to the actual node position
            int offset_y = nodeData.halls_data.east.offsetFromNodeCenter;
            Vector2 newNodeOffset = new Vector2(offset_x, -offset_y);

            //positionsArray[2] = nodeData.offsetFromParentNode + GetParentNodePosition(nodeIndex) + newNodeOffset;
            positionsArray[2] = newNodeOffset;

        }

        if (nodeData.halls_data.west.length > 0 && !nodeData.halls_data.west.isConnected)
        {
            int offset_x = nodeData.walls_offset.west + nodeData.halls_data.west.length + default_wallsOffset.east + 2; //the +2 accounts for the wall cell and moving to the actual node position
            int offset_y = nodeData.halls_data.west.offsetFromNodeCenter;
            Vector2 newNodeOffset = new Vector2(-offset_x, offset_y);

            //positionsArray[3] = nodeData.offsetFromParentNode + GetParentNodePosition(nodeIndex) + newNodeOffset;
            positionsArray[3] = newNodeOffset;

        }

        //Debug.Log("Offset N: " + positionsArray[0] + " Offset S: " + positionsArray[1] + " Offset E: " + positionsArray[2] + " Offset W: " + positionsArray[3]);


        return positionsArray;
    }

    private void ReevaluateOtherNodePositions(int resizedNodeIndex)
    {
        for (int i = 0; i < endNodeIndex; i++)
        {
            if (i != resizedNodeIndex)
            {
                NodeData nodeData = node_array[i];
                int parent_index = nodeData.parentNode_index;

                if (parent_index != -1)
                {
                    NodeData parentData = node_array[nodeData.parentNode_index];
                    Vector2 offsetFromParent = nodeData.offsetFromParentNode;

                    if (offsetFromParent.x == 0 && offsetFromParent.y > 0) //north of parent
                    {
                        int offset_y = parentData.walls_offset.north + parentData.halls_data.north.length + nodeData.walls_offset.south + 2; //the +2 accounts for the wall cell and moving to the actual node position

                        int offset_x = parentData.halls_data.north.offsetFromNodeCenter;
                        Vector2 newNodeOffset = new Vector2(offset_x, offset_y);

                        nodeData.offsetFromParentNode = newNodeOffset;

                    }
                    else if (offsetFromParent.x == 0 && offsetFromParent.y < 0) //south of parent
                    {
                        int offset_y = parentData.walls_offset.south + parentData.halls_data.south.length + nodeData.walls_offset.north + 2; //the +2 accounts for the wall cell and moving to the actual node position
                        int offset_x = nodeData.halls_data.south.offsetFromNodeCenter;
                        Vector2 newNodeOffset = new Vector2(offset_x, -offset_y);

                        nodeData.offsetFromParentNode = newNodeOffset;

                    }
                    else if (offsetFromParent.x > 0 && offsetFromParent.y == 0) //east of parent
                    {
                        int offset_x = parentData.walls_offset.east + parentData.halls_data.east.length + nodeData.walls_offset.west + 2; //the +2 accounts for the wall cell and moving to the actual node position
                        int offset_y = nodeData.halls_data.east.offsetFromNodeCenter;
                        Vector2 newNodeOffset = new Vector2(offset_x, -offset_y);

                        nodeData.offsetFromParentNode = newNodeOffset;

                    }
                    else if (offsetFromParent.x < 0 && offsetFromParent.y == 0) //west of parent
                    {
                        int offset_x = parentData.walls_offset.west + parentData.halls_data.west.length + nodeData.walls_offset.east + 2; //the +2 accounts for the wall cell and moving to the actual node position
                        int offset_y = nodeData.halls_data.west.offsetFromNodeCenter;
                        Vector2 newNodeOffset = new Vector2(-offset_x, offset_y);

                        nodeData.offsetFromParentNode = newNodeOffset;



                    }

                }

            }


        }

    }


    private void InstantiateNodeLayout(int node_index)
    {
        NodeData thisNode = node_array[node_index];
        //get node parent position
        Vector2 parentNode_position = GetParentNodePosition(node_index);
        Vector2 thisNodePosition = thisNode.offsetFromParentNode + parentNode_position;



        int startZ;
        int endZ;
        int startX;
        int endX;
        //Set node floor

        startZ = (int)thisNodePosition.y - thisNode.walls_offset.south;
        endZ = (int)thisNodePosition.y + thisNode.walls_offset.north;
        startX = (int)thisNodePosition.x - thisNode.walls_offset.west;
        endX = (int)thisNodePosition.x + thisNode.walls_offset.east;

        for (int j = startZ; j <= endZ; j++)
        {
            for (int i = startX; i <= endX; i++)
            {
                CheckCellAndInstantiate(i, j, cellType.node);

            }

        }

        //Set Node Walls
        for (int i = (startX - 1); i <= endX + 1; i++)
        {
            CheckCellAndInstantiate(i, startZ - 1, cellType.wall);
            CheckCellAndInstantiate(i, endZ + 1, cellType.wall);

            //cellMatrix[i, startZ - 1] = new cellData(cellType.wall, cellMatrix[i, startZ - 1].cell_obj);
            //cellMatrix[i, endZ + 1] = new cellData(cellType.wall, cellMatrix[i, endZ + 1].cell_obj);
        }


        for (int j = startZ - 1; j < endZ + 1; j++)
        {
            CheckCellAndInstantiate(startX - 1, j, cellType.wall);
            CheckCellAndInstantiate(endX + 1, j, cellType.wall);

            //cellMatrix[startX - 1, j] = new cellData(cellType.wall, cellMatrix[startX - 1, j].cell_obj);
            //cellMatrix[endX + 1, j] = new cellData(cellType.wall, cellMatrix[endX + 1, j].cell_obj);
        }

        //Set Hall Floors and Walls
        //north
        startX = (int)thisNodePosition.x + (int)thisNode.halls_data.north.offsetFromNodeCenter - (int)(thisNode.halls_data.north.width / 2);
        endX = startX + (int)thisNode.halls_data.north.width - 1;
        startZ = (int)thisNodePosition.y + (int)thisNode.walls_offset.north + 1;
        endZ = startZ + (int)thisNode.halls_data.north.length - 1;

        for (int j = startZ; j <= endZ; j++) //hall floor
        {
            for (int i = startX; i <= endX; i++)
            {
                CheckCellAndInstantiate(i, j, cellType.hall);

                //cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);
            }

            //hall walls
            CheckCellAndInstantiate(startX - 1, j, cellType.wall);
            CheckCellAndInstantiate(endX + 1, j, cellType.wall);

            //cellMatrix[startX - 1, j] = new cellData(cellType.wall, cellMatrix[startX - 1, j].cell_obj);
            //cellMatrix[endX + 1, j] = new cellData(cellType.wall, cellMatrix[endX + 1, j].cell_obj);
        }


        //south
        startX = (int)thisNodePosition.x + (int)thisNode.halls_data.south.offsetFromNodeCenter - (int)(thisNode.halls_data.south.width / 2);
        endX = startX + (int)thisNode.halls_data.south.width - 1;
        startZ = (int)thisNodePosition.y - ((int)thisNode.walls_offset.south + 1);
        endZ = startZ - ((int)thisNode.halls_data.south.length - 1);

        for (int j = startZ; j >= endZ; j--)
        {
            for (int i = startX; i <= endX; i++)
            {

                CheckCellAndInstantiate(i, j, cellType.hall);

                //cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);
            }

            CheckCellAndInstantiate(startX - 1, j, cellType.wall);
            CheckCellAndInstantiate(endX + 1, j, cellType.wall);

            //cellMatrix[startX - 1, j] = new cellData(cellType.wall, cellMatrix[startX - 1, j].cell_obj);
            //cellMatrix[endX + 1, j] = new cellData(cellType.wall, cellMatrix[endX + 1, j].cell_obj);
        }

        //east
        startX = (int)thisNodePosition.x + (int)thisNode.walls_offset.east + 1;
        endX = startX + (int)thisNode.halls_data.east.length - 1;
        startZ = (int)thisNodePosition.y - (int)thisNode.halls_data.east.offsetFromNodeCenter + (int)(thisNode.halls_data.east.width / 2);
        endZ = startZ - ((int)thisNode.halls_data.east.width - 1);

        for (int j = startZ; j >= endZ; j--)
        {
            for (int i = startX; i <= endX; i++)
            {

                CheckCellAndInstantiate(i, j, cellType.hall);
                //cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);

                CheckCellAndInstantiate(i, startZ + 1, cellType.wall);
                CheckCellAndInstantiate(i, endZ - 1, cellType.wall);

                //cellMatrix[i, startZ + 1] = new cellData(cellType.wall, cellMatrix[i, startZ + 1].cell_obj);
                //cellMatrix[i, endZ - 1] = new cellData(cellType.wall, cellMatrix[i, endZ - 1].cell_obj);

            }


        }

        //west
        startX = (int)thisNodePosition.x - ((int)thisNode.walls_offset.west + 1);
        endX = startX - ((int)thisNode.halls_data.west.length - 1);
        startZ = (int)thisNodePosition.y + (int)thisNode.halls_data.west.offsetFromNodeCenter + (int)(thisNode.halls_data.west.width / 2);
        endZ = startZ - ((int)thisNode.halls_data.west.width - 1);

        for (int j = startZ; j >= endZ; j--)
        {
            for (int i = startX; i >= endX; i--)
            {
                CheckCellAndInstantiate(i, j, cellType.hall);

                //cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);

                CheckCellAndInstantiate(i, startZ + 1, cellType.wall);
                CheckCellAndInstantiate(i, endZ - 1, cellType.wall);

                //cellMatrix[i, startZ + 1] = new cellData(cellType.wall, cellMatrix[i, startZ + 1].cell_obj);
                //cellMatrix[i, endZ - 1] = new cellData(cellType.wall, cellMatrix[i, endZ - 1].cell_obj);

            }


        }


    }

    private void CheckCellAndInstantiate(int i, int j, cellType _cellType)
    {
        if (cellMatrix[i, j].cell_obj == null)
        {
            Vector3 gridPoint = new Vector3(i - grid_halfWidth, 0f, j - grid_halfHeight);
            GameObject currentCell = Instantiate(grid_prefab, gridPoint, Quaternion.identity);
            currentCell.transform.SetParent(gridContainer.transform);

            cellMatrix[i, j] = new cellData(_cellType, currentCell);

        }
        else if ((_cellType == cellType.hall || _cellType == cellType.node) && cellMatrix[i, j].type == cellType.wall)
        {
            cellMatrix[i, j].type = _cellType;
        }


        UpdateCellPrefabParameters(i, j);
    }


    private void DestroyGridObjects()
    {
        foreach (Transform child in gridContainer.transform)
        {
            Destroy(child.gameObject);

        }
    }

    private void UpdateGridData()
    {
        for (int node_index = 0; node_index < endNodeIndex; node_index++)
        {
            //Debug.Log("Updating grid for node:" + node_index);

            NodeData thisNode = node_array[node_index];
            //get node parent position
            Vector2 parentNode_position = GetParentNodePosition(node_index);
            Vector2 thisNodePosition = thisNode.offsetFromParentNode + parentNode_position;



            int startZ;
            int endZ;
            int startX;
            int endX;
            //Set node floor

            startZ = (int)thisNodePosition.y - thisNode.walls_offset.south;
            endZ = (int)thisNodePosition.y + thisNode.walls_offset.north;
            startX = (int)thisNodePosition.x - thisNode.walls_offset.west;
            endX = (int)thisNodePosition.x + thisNode.walls_offset.east;

            for (int j = startZ; j <= endZ; j++)
            {
                for (int i = startX; i <= endX; i++)
                {
                    cellMatrix[i, j] = new cellData(cellType.node, cellMatrix[i, j].cell_obj);
                }

            }

            //Set Node Walls
            for (int i = (startX - 1); i <= endX + 1; i++)
            {
                cellMatrix[i, startZ - 1] = new cellData(cellType.wall, cellMatrix[i, startZ - 1].cell_obj);
                cellMatrix[i, endZ + 1] = new cellData(cellType.wall, cellMatrix[i, endZ + 1].cell_obj);
            }


            for (int j = startZ - 1; j < endZ + 1; j++)
            {
                cellMatrix[startX - 1, j] = new cellData(cellType.wall, cellMatrix[startX - 1, j].cell_obj);
                cellMatrix[endX + 1, j] = new cellData(cellType.wall, cellMatrix[endX + 1, j].cell_obj);
            }

            //Set Hall Floors and Walls
            //north
            startX = (int)thisNodePosition.x + (int)thisNode.halls_data.north.offsetFromNodeCenter - (int)(thisNode.halls_data.north.width / 2);
            endX = startX + (int)thisNode.halls_data.north.width - 1;
            startZ = (int)thisNodePosition.y + (int)thisNode.walls_offset.north + 1;
            endZ = startZ + (int)thisNode.halls_data.north.length - 1;

            for (int j = startZ; j <= endZ; j++) //hall floor
            {
                for (int i = startX; i <= endX; i++)
                {
                    cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);
                }

                //hall walls
                cellMatrix[startX - 1, j] = new cellData(cellType.wall, cellMatrix[startX - 1, j].cell_obj);
                cellMatrix[endX + 1, j] = new cellData(cellType.wall, cellMatrix[endX + 1, j].cell_obj);
            }


            //south
            startX = (int)thisNodePosition.x + (int)thisNode.halls_data.south.offsetFromNodeCenter - (int)(thisNode.halls_data.south.width / 2);
            endX = startX + (int)thisNode.halls_data.south.width - 1;
            startZ = (int)thisNodePosition.y - ((int)thisNode.walls_offset.south + 1);
            endZ = startZ - ((int)thisNode.halls_data.south.length - 1);

            for (int j = startZ; j >= endZ; j--)
            {
                for (int i = startX; i <= endX; i++)
                {
                    cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);
                }

                cellMatrix[startX - 1, j] = new cellData(cellType.wall, cellMatrix[startX - 1, j].cell_obj);
                cellMatrix[endX + 1, j] = new cellData(cellType.wall, cellMatrix[endX + 1, j].cell_obj);
            }

            //east
            startX = (int)thisNodePosition.x + (int)thisNode.walls_offset.east + 1;
            endX = startX + (int)thisNode.halls_data.east.length - 1;
            startZ = (int)thisNodePosition.y - (int)thisNode.halls_data.east.offsetFromNodeCenter + (int)(thisNode.halls_data.east.width / 2);
            endZ = startZ - ((int)thisNode.halls_data.east.width - 1);

            for (int j = startZ; j >= endZ; j--)
            {
                for (int i = startX; i <= endX; i++)
                {
                    cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);

                    cellMatrix[i, startZ + 1] = new cellData(cellType.wall, cellMatrix[i, startZ + 1].cell_obj);
                    cellMatrix[i, endZ - 1] = new cellData(cellType.wall, cellMatrix[i, endZ - 1].cell_obj);

                }


            }

            //west
            startX = (int)thisNodePosition.x - ((int)thisNode.walls_offset.west + 1);
            endX = startX - ((int)thisNode.halls_data.west.length - 1);
            startZ = (int)thisNodePosition.y + (int)thisNode.halls_data.west.offsetFromNodeCenter + (int)(thisNode.halls_data.west.width / 2);
            endZ = startZ - ((int)thisNode.halls_data.west.width - 1);

            for (int j = startZ; j >= endZ; j--)
            {
                for (int i = startX; i >= endX; i--)
                {
                    cellMatrix[i, j] = new cellData(cellType.hall, cellMatrix[i, j].cell_obj);

                    cellMatrix[i, startZ + 1] = new cellData(cellType.wall, cellMatrix[i, startZ + 1].cell_obj);
                    cellMatrix[i, endZ - 1] = new cellData(cellType.wall, cellMatrix[i, endZ - 1].cell_obj);

                }


            }


        }





    }
    private void ResetCellMatrix()
    {
        for (int j = 0; j < gridHeight; j++)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                cellMatrix[i, j] = new cellData(cellType.undef, null);
                //cellMatrix[i, j] = new cellData(cellType.undef, cellMatrix[i, j].cell_obj);
            }
        }

    }
    private void RedrawGrid()
    {

        for (int j = 0; j < gridHeight; j++)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                UpdateCellPrefabParameters(i, j);
            }
        }

    }

    private Vector2 GetParentNodePosition(int node_index)
    {
        Vector2 parentPos;

        if (node_array[node_index].parentNode_index == -1) //no parent
        {
            parentPos = origin_position;
            //Debug.Log("ParentPos assigned as:" + parentPos);
        }
        else
        {
            int parentIndex = node_array[node_index].parentNode_index;
            //Debug.Log("Getting Parent Node Position. Current Node:" + node_index + " parent index:" + parentIndex);

            parentPos = node_array[parentIndex].offsetFromParentNode + GetParentNodePosition(parentIndex);
        }

        return parentPos;
    }

    private void UpdateCellPrefabParameters(int i, int j)
    {
        TextMeshProUGUI _tmp = cellMatrix[i, j].cell_obj.GetComponentInChildren<TextMeshProUGUI>();

        Material materialToUse;

        if (_tmp != null)
        {
            string text;

            switch (cellMatrix[i, j].type)
            {
                case cellType.undef:
                    text = "UD";
                    materialToUse = grid_greenMat;
                    break;
                case cellType.wall:
                    text = "W";
                    materialToUse = grid_whiteMat;
                    break;
                case cellType.node:
                    text = "N";
                    materialToUse = grid_blackMat;
                    break;
                case cellType.hall:
                    text = "H";
                    materialToUse = grid_greyMat;
                    break;
                default:
                    text = "X";
                    materialToUse = grid_greenMat;
                    break;
            }

            cellMatrix[i, j].cell_obj.GetComponent<MeshRenderer>().material = materialToUse;
            _tmp.text = text;
        }
        else
        {
            Debug.Log("TextMeshPro component not found.");
        }
    }

    private void InitializeGrid()
    {
        GameObject gridContainer = new GameObject("GridContainer");

        if (grid_prefab != null)
        {
            for (int j = 0; j < gridHeight; j++) // the < makes sure count stops at gridHeight-1 because index starts at 0
            {
                float zPosition = j - grid_halfHeight; //(-grid_halfHeight) keeps the grid draw centered at 0,0 worldspace

                for (int i = 0; i < gridWidth; i++)
                {
                    Vector3 gridPoint = new Vector3(i - grid_halfWidth, 0f, zPosition);
                    GameObject currentCell = Instantiate(grid_prefab, gridPoint, Quaternion.identity);
                    currentCell.transform.SetParent(gridContainer.transform);

                    cellMatrix[i, j] = new cellData(cellType.undef, currentCell);

                    UpdateCellPrefabParameters(i, j);
                }

            }

        }
        else
        {
            Debug.Log("grid_prefab is NULL.");
        }
    }

    private int CreateNode(int parent_index, Vector2 offset, int gen)
    {

        WallsOffset walls = default_wallsOffset; //how many cells are between node an wall
        HallStats n_hall = zeroed_hallStats;
        HallStats s_hall = zeroed_hallStats;
        HallStats e_hall = zeroed_hallStats;
        HallStats w_hall = zeroed_hallStats;
        HallsData halls = new HallsData(n_hall, s_hall, e_hall, w_hall);

        node_array[endNodeIndex] = new NodeData(parent_index, offset, walls, halls, gen);

        //Debug.Log("Added node " + endNodeIndex + " to Node " + parent_index);


        endNodeIndex++; //increase End Node Index by 1

        return endNodeIndex - 1;
    }

    private void ResetNode(int nodeIndex)
    {
        WallsOffset walls = default_wallsOffset; //how many cells are between node an wall
        HallStats n_hall = zeroed_hallStats;
        HallStats s_hall = zeroed_hallStats;
        HallStats e_hall = zeroed_hallStats;
        HallStats w_hall = zeroed_hallStats;
        HallsData halls = new HallsData(n_hall, s_hall, e_hall, w_hall);

        node_array[nodeIndex] = new NodeData(nodeIndex - 1, Vector2.zero, walls, halls, -1);

    }
}
