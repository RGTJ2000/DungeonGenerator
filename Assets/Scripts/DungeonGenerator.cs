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
    public GridMeshGenerator gridMeshGenerator;

    [SerializeField] GameObject grid_prefab;
    [SerializeField] int grid_halfWidth;
    [SerializeField] int grid_halfHeight;
    [SerializeField] Material grid_greenMat;
    [SerializeField] Material grid_blackMat;
    [SerializeField] Material grid_whiteMat;
    [SerializeField] Material grid_greyMat;
    [SerializeField] Material grid_startMat;
    [SerializeField] Material grid_endMat;

    [SerializeField] int max_numberOfNodes = 50;
    //[SerializeField] int current_node;
    [SerializeField] int current_gen;
    public int mainPath_nodeCount = 10;

    [SerializeField] WallsOffset default_wallsOffset = new WallsOffset(1, 1, 1, 1);
    [SerializeField] HallStats default_hallStats = new HallStats(4, 3, 0, false);
    [SerializeField] HallStats zeroed_hallStats = new HallStats(0, 0, 0, false);

    [SerializeField] public int hall_maxLength;
    [SerializeField] public int hall_minLength;
    [SerializeField] private int hall_minWidth;
    [SerializeField] private int hall_maxWidth;
    [SerializeField] public int max_roomHeight;
    [SerializeField] public int max_roomWidth;

    private hallProbability neutral_hProb;
    [SerializeField] private hallProbability sw_hprob;
    [SerializeField] private float room_prob = 0.5f;  //higher probablity makes more rooms.

    private int gridWidth;
    private int gridHeight;
    private Vector2 origin_position;

    private NodeData[] node_array;
    private cellData[,] cellMatrix;

    private cellType[,] c_type_matrix;

    GameObject gridContainer;




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

    private int endNode_index = 0; //the first empty slot in the array
    private int pointerToGen0Node = 0;

    private bool gen1_complete = false;
    public bool gen2_complete = false;

    public bool currentlyGenerating = false;
    /*
     * METHODS BELOW *********************************************************************************
     */

    private void Awake()
    {
        _inputControls = new InputControls();


    }

    private void OnEnable()
    {
        _inputControls.General.Enable();

        _inputControls.General.Refresh.started += OnRefresh;

        _inputControls.General.CreateNextNode.started += OnCreateNextNode;

        _inputControls.General.GenerateFullLayout.started += OnGenerateFullLayout;

    }

    private void OnDisable()
    {
        _inputControls.General.Disable();

        _inputControls.General.Refresh.started -= OnRefresh;
        _inputControls.General.CreateNextNode.started -= OnCreateNextNode;
        _inputControls.General.GenerateFullLayout.started -= OnGenerateFullLayout;

    }

    private void Update()
    {

    }

    void Start()
    {
        gridContainer = new GameObject("GridContainer");

        node_array = new NodeData[max_numberOfNodes];
        endNode_index = 0;
        current_gen = 0;

        gridWidth = grid_halfWidth * 2;
        gridHeight = grid_halfHeight * 2;

        origin_position = new Vector2(grid_halfWidth, grid_halfHeight);

        //cellMatrix = new cellData[gridWidth, gridHeight];

        c_type_matrix = new cellType[gridWidth, gridHeight];

        InitializeCTypeMatrix();

        neutral_hProb = new hallProbability(1, 1, 1, 1);
        sw_hprob = new hallProbability(1, 3, 3, 1);

    }


    private void InitializeCTypeMatrix()
    {
        for (int j = 0; j < gridHeight; j++)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                c_type_matrix[i, j] = cellType.undef;
            }
        }
        Debug.Log("c_type_matrix initialized. c_type_matrix[0,0]=" + c_type_matrix[0, 0].ToString());
    }

    private void OnCreateNextNode(InputAction.CallbackContext context)
    {

        //if (pointerToGen0Node >= mainPath_nodeCount - 1) //reset nodes
        if (!gen1_complete)
        {
            int i = endNode_index;

            if (i < mainPath_nodeCount) //make main path
            {
                Debug.Log("ON NODE:" + i);
                if (i == 0)
                {
                    CreateNode(i - 1, Vector2.zero, -1, current_gen); //puts node at this offset

                    SelectAndAddRandomHallsToNode(i, sw_hprob, 1, 1);

                }
                else if (i == mainPath_nodeCount - 1)
                {
                    AddNodesToParentHalls(i - 1);


                    current_gen++;
                }
                else
                {
                    AddNodesToParentHalls(i - 1);

                    SelectAndAddRandomHallsToNode(i, sw_hprob, 1, 1);


                }


            }
            else //make expansion paths
            {
                Debug.Log("ADDING hallways and nodes to Main Path.");
                for (int mainPathNode = 0; mainPathNode < mainPath_nodeCount; mainPathNode++)
                {
                    SelectAndAddRandomHallsToNode(mainPathNode, neutral_hProb, 0, 3);
                    AddNodesToParentHalls(mainPathNode);
                }

                gen1_complete = true;
                //pointerToGen0Node++;

            }

            gridMeshGenerator.ClearMesh();
            WriteAllNodestoMatrix();
            gridMeshGenerator.GenerateMesh(c_type_matrix, new Vector2(-grid_halfWidth, -grid_halfHeight));
        }
        else
        {
            Debug.Log("Generation complete. Refresh [SPACE] to start new generation.");
        }





    }

    private void OnGenerateFullLayout(InputAction.CallbackContext context)
    {
        GenerateFullLayout();


    }

    public void GenerateFullLayout()
    {
        if (!currentlyGenerating && !gen2_complete)
        {
            StartCoroutine(GenerateLayout());

        }
        else
        {
            if (gen2_complete)
            {
                Debug.Log("Generation complete. Refresh [SPACE] to start new generation.");

            }
            else
            {
                Debug.Log("Still generating. Please wait.");
            }
        }



    }
    void OnRefresh(InputAction.CallbackContext context)
    {
        Refresh();
    }

    public void Refresh()
    {
        Debug.Log("Destroying Grid Objects.");
        gridMeshGenerator.ClearMesh();
        InitializeCTypeMatrix();

        endNode_index = 0;
        pointerToGen0Node = 0;
        current_gen = 0;
        gen1_complete = false;
        gen2_complete = false;


    }

    public void MainPathNodesUp()
    {
        mainPath_nodeCount++;
        if (mainPath_nodeCount > 100)
        {
            mainPath_nodeCount = 100;
        }
    }

    public void MainPathNodesDown()
    {
        mainPath_nodeCount--;
        if (mainPath_nodeCount < 1)
        {
            mainPath_nodeCount = 1;
        }
    }

    public void SetMainPathNodesCount(int count)
    {
        mainPath_nodeCount = Mathf.Clamp(count, 0, 100);
    }

    public void SetRoomWidthMax(int width)
    {
        max_roomWidth = Mathf.Clamp(width, hall_minWidth, 100);
    }

    public void RoomWidthUp()
    {
        max_roomWidth++;
        if (max_roomWidth > 100)
        {
            max_roomWidth = 100;
        }
    }

    public void RoomWidthDown()
    {
        max_roomWidth--;
        if (max_roomWidth < hall_minWidth)
        {
            max_roomWidth = hall_minWidth;
        }
    }


    public void SetRoomHeightMax(int height)
    {
        max_roomHeight = Mathf.Clamp(height, hall_minWidth, 100);
    }

    public void RoomHeightUp()
    {
        max_roomHeight++;
        if (max_roomHeight > 100)
        {
            max_roomHeight = 100;
        }
    }

    public void RoomHeightDown()
    {
        max_roomHeight--;
        if (max_roomHeight < hall_minWidth)
        {
            max_roomHeight = hall_minWidth;
        }
    }

    public void SetHallLengthMax(int length)
    {
        hall_maxLength = Mathf.Clamp(length, hall_minLength, 200);
    }

    public void HallLengthMaxUp()
    {
        hall_maxLength++;
        if (hall_maxLength > 200)
        {
            hall_maxLength = 200;
        }

    }

    public void HallLengthMaxDown()
    {
        hall_maxLength--;
        if (hall_maxLength < hall_minLength)
        {
            hall_maxLength = hall_minLength;
        }
    }

    public void SetHallLengthMin(int length)
    {
        hall_minLength = Mathf.Clamp(length, 1, 200);
    }

    public void HallLengthMinUp()
    {
        hall_minLength++;
        if (hall_minLength > hall_maxLength)
        {
            hall_minLength=hall_maxLength;
        }
    }
    public void HallLengthMinDown()
    {
        hall_minLength--;
        if (hall_minLength < 1)
        {
            hall_minLength = 1;
        }
    }

    private void ExpandNodeAtRandomAndOffset(int nodeIndex)
    {
        if (Random.value < room_prob)
        {
            int minHeight = node_array[nodeIndex].walls_offset.north + node_array[nodeIndex].walls_offset.south + 1;
            int minWidth = node_array[nodeIndex].walls_offset.east + node_array[nodeIndex].walls_offset.west + 1;
            //expand room
            int height = Random.Range(minHeight, max_roomHeight);
            int width = Random.Range(minWidth, max_roomWidth);

            node_array[nodeIndex].walls_offset = new WallsOffset(height / 2, height / 2, width / 2, width / 2);

            OffsetNodeFromParentHallway(nodeIndex);
        }


    }

    private void OffsetNodeFromParentHallway(int nodeIndex)
    {


        if (nodeIndex > 0)
        {
            NodeData nodeData = node_array[nodeIndex];
            NodeData parentNodeData = node_array[nodeData.parentNode_index];

            int roomHeight = nodeData.walls_offset.north + nodeData.walls_offset.south + 1;
            int roomWidth = nodeData.walls_offset.east + nodeData.walls_offset.west + 1;

            int newOffset = 0;
            int maxOffset = 0;

            float originalOffset_x = nodeData.offsetFromParentNode.x;
            float originalOffset_y = nodeData.offsetFromParentNode.y;

            switch (nodeData.directionIndexFromParent)  //shift node AND shift hallway entrance
            {
                case 0: //north
                    maxOffset = (roomWidth - parentNodeData.halls_data.north.width) / 2;
                    newOffset = Random.Range(-maxOffset, maxOffset + 1);
                    nodeData.offsetFromParentNode = new Vector2(originalOffset_x + newOffset, originalOffset_y);
                    nodeData.halls_data.south.offsetFromNodeCenter = nodeData.halls_data.south.offsetFromNodeCenter - newOffset; //halway shift
                    break;

                case 1: //south
                    maxOffset = (roomWidth - parentNodeData.halls_data.south.width) / 2;
                    newOffset = Random.Range(-maxOffset, maxOffset + 1);
                    nodeData.offsetFromParentNode = new Vector2(originalOffset_x - newOffset, originalOffset_y);
                    nodeData.halls_data.north.offsetFromNodeCenter = nodeData.halls_data.north.offsetFromNodeCenter + newOffset; //halway shift

                    break;

                case 2: //east
                    maxOffset = (roomHeight - parentNodeData.halls_data.east.width) / 2;
                    newOffset = Random.Range(-maxOffset, maxOffset + 1);
                    nodeData.offsetFromParentNode = new Vector2(originalOffset_x, originalOffset_y - newOffset);

                    nodeData.halls_data.west.offsetFromNodeCenter = nodeData.halls_data.west.offsetFromNodeCenter + newOffset; //halway shift

                    break;

                case 3: //west
                    maxOffset = (roomHeight - parentNodeData.halls_data.west.width) / 2;
                    newOffset = Random.Range(-maxOffset, maxOffset + 1);
                    nodeData.offsetFromParentNode = new Vector2(originalOffset_x, originalOffset_y + newOffset);

                    nodeData.halls_data.east.offsetFromNodeCenter = nodeData.halls_data.east.offsetFromNodeCenter - newOffset; //halway shift

                    break;

                default:
                    break;



            }

        }



    }


    void SelectAndAddRandomHallsToNode(int nodeIndex, hallProbability h_prob, int minHallAdd, int maxHallAdd)
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

        //Debug.Log("Node " + nodeIndex + ": Available directions" + string.Join(", ", directionsList));

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

        //determine how many hall to add randomly
        int hallsToAdd = Random.Range(minHallAdd, maxHallAdd + 1);

        //choose random hallway direction from list
        for (int i = 0; i < hallsToAdd; i++)
        {
            if (probPool.Count > 0)
            {
                int randomIndex = Random.Range(0, probPool.Count);
                string randomDirection = probPool[randomIndex];
                AddHallToNode(nodeIndex, randomDirection);
                //Debug.Log("Hall added to Node:" + nodeIndex + " in direction:" + randomDirection);

                probPool.RemoveAll(item => item == randomDirection);
            }
        }




    }

    private void AddHallToNode(int nodeIndex, string direction)
    {
        int length = Random.Range(hall_minLength, hall_maxLength + 1);

        int maxWidth;
        switch (direction)
        {

            case ("north"):
                maxWidth = node_array[nodeIndex].walls_offset.east + node_array[nodeIndex].walls_offset.west + 1;

                break;
            case ("south"):
                maxWidth = node_array[nodeIndex].walls_offset.east + node_array[nodeIndex].walls_offset.west + 1;

                break;
            case ("east"):
                maxWidth = node_array[nodeIndex].walls_offset.north + node_array[nodeIndex].walls_offset.south + 1;

                break;
            case ("west"):
                maxWidth = node_array[nodeIndex].walls_offset.north + node_array[nodeIndex].walls_offset.south + 1;

                break;
            default:
                maxWidth = hall_minWidth;
                break;

        }

        if (maxWidth >= hall_maxWidth)
        {
            maxWidth = hall_maxWidth;
        }

        int width = Random.Range(hall_minWidth, maxWidth);

        int offset = Random.Range(-(maxWidth - width) / 2, ((maxWidth - width) / 2) + 1);



        switch (direction)
        {
            case "north":
                node_array[nodeIndex].halls_data.north = new HallStats(length, width, offset, false); ;
                break;
            case "south":
                node_array[nodeIndex].halls_data.south = new HallStats(length, width, offset, false); ;
                break;
            case "east":
                node_array[nodeIndex].halls_data.east = new HallStats(length, width, offset, false); ;
                break;
            case "west":
                node_array[nodeIndex].halls_data.west = new HallStats(length, width, offset, false); ;
                break;
            default:

                break;


        }

        //Debug.Log("Node " + nodeIndex + ": adding hallway WIDTH=" + width + "(" + maxWidth + ") Offset=" + offset);
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

        int createdNodeIndex = CreateNode(parent_index, offset, hall_index, current_gen);

        HallStats entryHall;

        int entryHallOffset;

        switch (hall_index)  //update parent node connection boolean AND add hallway entrance to created node
        {
            case 0:
                node_array[parent_index].halls_data.north.isConnected = true;

                entryHallOffset = (int)node_array[createdNodeIndex].offsetFromParentNode.x - node_array[parent_index].halls_data.north.offsetFromNodeCenter;

                entryHall = new HallStats(1, node_array[parent_index].halls_data.north.width, entryHallOffset, true);

                node_array[createdNodeIndex].halls_data.south = entryHall;

                break;
            case 1:
                node_array[parent_index].halls_data.south.isConnected = true;

                entryHallOffset = -(int)node_array[createdNodeIndex].offsetFromParentNode.x - node_array[parent_index].halls_data.south.offsetFromNodeCenter;


                entryHall = new HallStats(1, node_array[parent_index].halls_data.south.width, entryHallOffset, true);

                node_array[createdNodeIndex].halls_data.north = entryHall;

                break;
            case 2:
                node_array[parent_index].halls_data.east.isConnected = true;

                entryHallOffset = -(int)node_array[createdNodeIndex].offsetFromParentNode.y - node_array[parent_index].halls_data.east.offsetFromNodeCenter;

                entryHall = new HallStats(1, node_array[parent_index].halls_data.east.width, entryHallOffset, true);

                node_array[createdNodeIndex].halls_data.west = entryHall;

                break;
            case 3:
                node_array[parent_index].halls_data.west.isConnected = true;


                entryHallOffset = (int)node_array[createdNodeIndex].offsetFromParentNode.y - node_array[parent_index].halls_data.west.offsetFromNodeCenter;

                entryHall = new HallStats(1, node_array[parent_index].halls_data.west.width, entryHallOffset, true);

                node_array[createdNodeIndex].halls_data.east = entryHall;
                break;
            default:
                break;

        }

        //Debug.Log("Added node to hall index:" + hall_index);

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
            Vector2 newNodeOffset = new Vector2(-offset_x, -offset_y);

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
        for (int i = 0; i < endNode_index; i++)
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

    private void ReevaluateThisNodePositionAfterExpand(int nodeIndex)
    {
        NodeData nodeData = node_array[nodeIndex];
        int parent_index = nodeData.parentNode_index;

        int directionIndex = nodeData.directionIndexFromParent;
        //Debug.Log("Reevaluating Node " + nodeIndex + " Position. Direction from Parent=" + directionIndex);

        if (parent_index != -1)
        {
            NodeData parentData = node_array[nodeData.parentNode_index];
            Vector2 offsetFromParent = nodeData.offsetFromParentNode;
            float originalOffset_x = offsetFromParent.x;
            float originalOffset_y = offsetFromParent.y;


            Vector2 newNodeOffset = Vector2.zero;

            if (directionIndex == 0) //north of parent
            {
                int offset_y = parentData.walls_offset.north + parentData.halls_data.north.length + nodeData.walls_offset.south + 2; //the +2 accounts for the wall cell and moving to the actual node position


                //int offset_x = parentData.halls_data.north.offsetFromNodeCenter; //the centers the node on the hallway
                newNodeOffset = new Vector2(originalOffset_x, offset_y);

                nodeData.offsetFromParentNode = newNodeOffset;

            }
            else if (directionIndex == 1) //south of parent
            {
                int offset_y = parentData.walls_offset.south + parentData.halls_data.south.length + nodeData.walls_offset.north + 2; //the +2 accounts for the wall cell and moving to the actual node position

                //int offset_x = parentData.halls_data.south.offsetFromNodeCenter;
                newNodeOffset = new Vector2(originalOffset_x, -offset_y);

                nodeData.offsetFromParentNode = newNodeOffset;

            }
            else if (directionIndex == 2) //east of parent
            {
                int offset_x = parentData.walls_offset.east + parentData.halls_data.east.length + nodeData.walls_offset.west + 2; //the +2 accounts for the wall cell and moving to the actual node position
                //int offset_y = parentData.halls_data.east.offsetFromNodeCenter;
                newNodeOffset = new Vector2(offset_x, originalOffset_y);

                nodeData.offsetFromParentNode = newNodeOffset;

            }
            else if (directionIndex == 3) //west of parent
            {
                int offset_x = parentData.walls_offset.west + parentData.halls_data.west.length + nodeData.walls_offset.east + 2; //the +2 accounts for the wall cell and moving to the actual node position
                //int offset_y = parentData.halls_data.west.offsetFromNodeCenter;
                newNodeOffset = new Vector2(-offset_x, originalOffset_y);

                nodeData.offsetFromParentNode = newNodeOffset;



            }


        }


    }

    private void WriteNodeToMatrix(int node_index)
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
                cellType thisCellType;

                if (node_index == 0)
                {
                    thisCellType = cellType.start;

                }
                else if (node_index == mainPath_nodeCount - 1)
                {
                    thisCellType = cellType.end;
                }
                else
                {
                    thisCellType = cellType.node;
                }
                CheckCellTypeAndWrite(i, j, thisCellType);


            }

        }

        //Set Node Walls
        for (int i = (startX - 1); i <= endX + 1; i++)
        {
            CheckCellTypeAndWrite(i, startZ - 1, cellType.wall);
            CheckCellTypeAndWrite(i, endZ + 1, cellType.wall);


        }


        for (int j = startZ - 1; j < endZ + 1; j++)
        {
            CheckCellTypeAndWrite(startX - 1, j, cellType.wall);
            CheckCellTypeAndWrite(endX + 1, j, cellType.wall);


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
                CheckCellTypeAndWrite(i, j, cellType.hall);

            }

            //hall walls
            CheckCellTypeAndWrite(startX - 1, j, cellType.wall);
            CheckCellTypeAndWrite(endX + 1, j, cellType.wall);

        }


        //south
        startX = (int)thisNodePosition.x - (int)thisNode.halls_data.south.offsetFromNodeCenter - (int)(thisNode.halls_data.south.width / 2);
        endX = startX + (int)thisNode.halls_data.south.width - 1;
        startZ = (int)thisNodePosition.y - ((int)thisNode.walls_offset.south + 1);
        endZ = startZ - ((int)thisNode.halls_data.south.length - 1);


        for (int j = startZ; j >= endZ; j--)
        {
            for (int i = startX; i <= endX; i++)
            {

                CheckCellTypeAndWrite(i, j, cellType.hall);

            }

            CheckCellTypeAndWrite(startX - 1, j, cellType.wall);
            CheckCellTypeAndWrite(endX + 1, j, cellType.wall);

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

                CheckCellTypeAndWrite(i, j, cellType.hall);

                CheckCellTypeAndWrite(i, startZ + 1, cellType.wall);
                CheckCellTypeAndWrite(i, endZ - 1, cellType.wall);


            }


        }

        //west hall
        startX = (int)thisNodePosition.x - ((int)thisNode.walls_offset.west + 1);
        endX = startX - ((int)thisNode.halls_data.west.length - 1);
        startZ = (int)thisNodePosition.y + (int)thisNode.halls_data.west.offsetFromNodeCenter + (int)(thisNode.halls_data.west.width / 2);
        endZ = startZ - ((int)thisNode.halls_data.west.width - 1);

        for (int j = startZ; j >= endZ; j--)
        {
            for (int i = startX; i >= endX; i--)
            {
                CheckCellTypeAndWrite(i, j, cellType.hall);


                CheckCellTypeAndWrite(i, startZ + 1, cellType.wall);
                CheckCellTypeAndWrite(i, endZ - 1, cellType.wall);


            }


        }

    }


    private void CheckCellTypeAndWrite(int i, int j, cellType thisCellType)
    {
        //this updates the c_type_matrix. Only writes if the cell is undef or if it's a wall
        if (c_type_matrix[i, j] == cellType.wall || c_type_matrix[i, j] == cellType.undef)
        {
            c_type_matrix[i, j] = thisCellType;
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




    private int CreateNode(int parent_index, Vector2 offset, int hallIndex, int gen)
    {
        int parentHallWidth = hall_minWidth;

        if (parent_index >= 0)
        {
            switch (hallIndex)
            {
                case 0: //north
                    parentHallWidth = node_array[parent_index].halls_data.north.width;
                    break;
                case 1:
                    parentHallWidth = node_array[parent_index].halls_data.south.width;

                    break;
                case 2:
                    parentHallWidth = node_array[parent_index].halls_data.east.width;

                    break;
                case 3:
                    parentHallWidth = node_array[parent_index].halls_data.west.width;

                    break;
                default:
                    parentHallWidth = hall_minWidth;

                    break;


            }

        }

        //WallsOffset walls = default_wallsOffset; //how many cells are between node an wall
        WallsOffset walls = new WallsOffset(parentHallWidth / 2, parentHallWidth / 2, parentHallWidth / 2, parentHallWidth / 2);



        HallStats n_hall = zeroed_hallStats;
        HallStats s_hall = zeroed_hallStats;
        HallStats e_hall = zeroed_hallStats;
        HallStats w_hall = zeroed_hallStats;
        HallsData halls = new HallsData(n_hall, s_hall, e_hall, w_hall);

        node_array[endNode_index] = new NodeData(parent_index, offset, hallIndex, walls, halls, gen);

        //Debug.Log("Added node " + endNodeIndex + " to Node " + parent_index);
        //Debug.Log("Creating Node:" + endNode_index + " at offset " + offset);

        ExpandNodeAtRandomAndOffset(endNode_index);
        ReevaluateThisNodePositionAfterExpand(endNode_index);

        endNode_index++; //increase End Node Index by 1

        return endNode_index - 1;
    }


    IEnumerator GenerateLayout()
    {
        currentlyGenerating = true;
        Debug.Log("Generation requested.");

        yield return null;

        Debug.Log("Creating MAIN PATH...");
        for (int i = 0; i < mainPath_nodeCount; i++) //generates the main path
        {
            if (i == 0) //make the start node
            {
                CreateNode(i - 1, Vector2.zero, -1, current_gen); //puts node at this offset
                SelectAndAddRandomHallsToNode(i, sw_hprob, 1, 1);

            }
            else if (i == mainPath_nodeCount - 1) //makes the end node
            {
                AddNodesToParentHalls(i - 1);
                current_gen++;
            }
            else //make the intermediate nodes
            {
                AddNodesToParentHalls(i - 1);
                SelectAndAddRandomHallsToNode(i, sw_hprob, 1, 1);
            }
            yield return null; // Lets Unity process a frame

        }
        Debug.Log("MAIN PATH completed.");
        int gen1_startIndex = endNode_index;

        Debug.Log("Creating GEN 1 NODES off Main Path...");
        for (int i = 0; i < mainPath_nodeCount; i++) //step through mainpath nodes, adding halls and nodes.
        {
            SelectAndAddRandomHallsToNode(i, neutral_hProb, 0, 3);
            AddNodesToParentHalls(i);

            yield return null; // Lets Unity process a frame
        }
        Debug.Log("Gen1 Completed.");
        gen1_complete = true;
        current_gen++;
        int gen1_endIndex = endNode_index - 1;

        Debug.Log("Creating GEN 2 NODES off gen1 nodes.");
        for (int i = gen1_startIndex; i <= gen1_endIndex; i++)
        {
            SelectAndAddRandomHallsToNode(i, neutral_hProb, 0, 3);
            AddNodesToParentHalls(i);
            yield return null;
        }
        Debug.Log("Gen2 completed.");

        gen2_complete = true;


        Debug.Log("Beginning Instantiation.");

        yield return null;


        //write the nodes to c_type_matrix
        WriteAllNodestoMatrix();


        //Crop layout and render mesh
        cellType[,] croppedMatrix = ReturnCroppedLayout(c_type_matrix);
        Vector2 croppedOffset = new Vector2(croppedMatrix.GetLength(0) / 2, croppedMatrix.GetLength(1) / 2);
        gridMeshGenerator.GenerateMesh(croppedMatrix, -croppedOffset);

        //reset Generating flag to false
        currentlyGenerating = false;


    }


    private cellType[,] ReturnCroppedLayout(cellType[,] originalMatrix)
    {


        RectInt rectangle = FindLayoutBoundingBox(originalMatrix);

        cellType[,] croppedMatrix = new cellType[rectangle.width, rectangle.height];

        for (int j = rectangle.y; j < rectangle.y + rectangle.height; j++)
        {
            int croppedIndex_y = j - rectangle.y;

            for (int i = rectangle.x; i < rectangle.x + rectangle.width; i++)
            {
                int croppedIndex_x = i - rectangle.x;

                croppedMatrix[croppedIndex_x, croppedIndex_y] = originalMatrix[i, j];

            }
        }


        return croppedMatrix;


    }

    private void WriteAllNodestoMatrix()
    {
        for (int i = 0; i < endNode_index; i++)
        {
            WriteNodeToMatrix(i);
        }

    }

    private RectInt FindLayoutBoundingBox(cellType[,] cellMatrix)
    {
        int minX = cellMatrix.GetLength(0); // columns (set to max initially)
        int maxX = 0;
        int minY = cellMatrix.GetLength(1); // rows (set to max initially)
        int maxY = 0;

        bool foundAny = false;

        // Scan through the entire grid
        for (int j = 0; j < cellMatrix.GetLength(1); j++)
        {
            for (int i = 0; i < cellMatrix.GetLength(0); i++)
            {
                if (cellMatrix[i, j] != cellType.undef)
                {
                    foundAny = true;

                    if (i < minX) minX = i;
                    if (i > maxX) maxX = i;
                    if (j < minY) minY = j;
                    if (j > maxY) maxY = j;
                }
            }
        }

        if (!foundAny)
        {
            // Return an empty rect if no defined values found
            return new RectInt(0, 0, 0, 0);
        }

        // RectInt parameters: x, y, width, height
        return new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);

    }



}
