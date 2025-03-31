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
    [SerializeField] Material grid_startMat;
    [SerializeField] Material grid_endMat;

    [SerializeField] int max_numberOfNodes = 50;
    //[SerializeField] int current_node;
    [SerializeField] int current_gen;
    [SerializeField] int mainPath_nodeCount = 10;

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
        hall,
        start,
        end

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

    private int endNode_index = 0; //the first empty slot in the array
    private int pointerToGen0Node = 0;

    private bool gen1_complete = false;

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

        _inputControls.General.NodeSize.started += OnNodeSize;

        _inputControls.General.Refresh.started += OnRefresh;

        _inputControls.General.CreateNextNode.started += OnCreateNextNode;

        _inputControls.General.GenerateFullLayout.started += OnGenerateFullLayout;

    }

    private void OnDisable()
    {
        _inputControls.General.Disable();

        _inputControls.General.NodeSize.started -= OnNodeSize;
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

        cellMatrix = new cellData[gridWidth, gridHeight];

        neutral_hProb = new hallProbability(1, 1, 1, 1);
        sw_hprob = new hallProbability(1, 3, 3, 1);

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


            StartCoroutine(InstantiateAllNodesCoroutine());

        }
        else
        {
            Debug.Log("Generation complete. Refresh [SPACE] to start new generation.");
        }





    }

    private void OnGenerateFullLayout(InputAction.CallbackContext context)
    {
        if (!gen1_complete)
        {
            StartCoroutine(GenerateLayout());

        }
        else
        {
            Debug.Log("Generation complete. Refresh [SPACE] to start new generation.");
        }

        /*
        if (!gen1_complete)
        {
            currentlyGenerating = true;

            Debug.Log("Creating Main Path...");
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

            }

            int gen1_startIndex = endNode_index;
            
            Debug.Log("Creating gen1 nodes from Main Path...");
            for (int i = 0; i < mainPath_nodeCount; i++) //step through mainpath nodes, adding halls and nodes.
            {
                SelectAndAddRandomHallsToNode(i, neutral_hProb, 0, 3);
                AddNodesToParentHalls(i);


            }
            current_gen++;
            int gen1_endIndex = endNode_index - 1;


            Debug.Log("gen1 nodes generation complete. gen1 Start Index="+gen1_startIndex+" End Index="+gen1_endIndex);
            Debug.Log("node_array endNode_index:"+  endNode_index);
            

            gen1_complete = true;

            InstantiateAllNodes();
            currentlyGenerating = false;
        }
        else
        {
            Debug.Log("Generation complete. Refresh [SPACE] to start new generation.");
        }
        */



    }
    void OnRefresh(InputAction.CallbackContext context)
    {

        DestroyGridObjects();
        //ResetCellMatrix();
        endNode_index = 0;
        pointerToGen0Node = 0;
        current_gen = 0;
        gen1_complete = false;
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
        //ResetCellMatrix();

        InstantiateAllNodes();



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

            OffsetNodeFromHallway(nodeIndex);
        }


    }

    private void OffsetNodeFromHallway(int nodeIndex)
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

    private void InstantiateAllNodes()
    {
        for (int i = 0; i < endNode_index; i++)
        {
            InstantiateNodeLayout(i);
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
        int length = Random.Range(1, hall_maxLength + 1);

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

            //Debug.Log("Node:" + nodeIndex + " expanded position set at " + newNodeOffset);

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

                CheckCellAndInstantiate(i, j, thisCellType, node_index);

            }

        }

        //Set Node Walls
        for (int i = (startX - 1); i <= endX + 1; i++)
        {
            CheckCellAndInstantiate(i, startZ - 1, cellType.wall, node_index);
            CheckCellAndInstantiate(i, endZ + 1, cellType.wall, node_index);


        }


        for (int j = startZ - 1; j < endZ + 1; j++)
        {
            CheckCellAndInstantiate(startX - 1, j, cellType.wall, node_index);
            CheckCellAndInstantiate(endX + 1, j, cellType.wall, node_index);


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
                CheckCellAndInstantiate(i, j, cellType.hall, node_index);

            }

            //hall walls
            CheckCellAndInstantiate(startX - 1, j, cellType.wall, node_index);
            CheckCellAndInstantiate(endX + 1, j, cellType.wall, node_index);

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

                CheckCellAndInstantiate(i, j, cellType.hall, node_index);

            }

            CheckCellAndInstantiate(startX - 1, j, cellType.wall, node_index);
            CheckCellAndInstantiate(endX + 1, j, cellType.wall, node_index);

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

                CheckCellAndInstantiate(i, j, cellType.hall, node_index);

                CheckCellAndInstantiate(i, startZ + 1, cellType.wall, node_index);
                CheckCellAndInstantiate(i, endZ - 1, cellType.wall, node_index);


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
                CheckCellAndInstantiate(i, j, cellType.hall, node_index);


                CheckCellAndInstantiate(i, startZ + 1, cellType.wall, node_index);
                CheckCellAndInstantiate(i, endZ - 1, cellType.wall, node_index);


            }


        }


    }

    private void CheckCellAndInstantiate(int i, int j, cellType thisCellType, int nodeIndex)
    {
        if (cellMatrix[i, j].cell_obj == null)
        {
            Vector3 gridPoint = new Vector3(i - grid_halfWidth, 0f, j - grid_halfHeight);
            GameObject currentCell = Instantiate(grid_prefab, gridPoint, Quaternion.identity);
            currentCell.transform.SetParent(gridContainer.transform);

            cellMatrix[i, j] = new cellData(thisCellType, currentCell);

        }
        else if ((thisCellType != cellType.wall) && cellMatrix[i, j].type == cellType.wall)
        {
            cellMatrix[i, j].type = thisCellType;
        }


        UpdateCellPrefabParameters(i, j, nodeIndex);
    }

    private void DestroyGridObjects()
    {
        foreach (Transform child in gridContainer.transform)
        {
            Destroy(child.gameObject);

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

    private void UpdateCellPrefabParameters(int i, int j, int nodeIndex)
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
                    text = nodeIndex.ToString();
                    materialToUse = grid_whiteMat;
                    break;
                case cellType.node:
                    text = nodeIndex.ToString();
                    materialToUse = grid_blackMat;
                    break;
                case cellType.hall:
                    text = nodeIndex.ToString();
                    materialToUse = grid_greyMat;
                    break;
                case cellType.start:
                    text = "S";
                    materialToUse = grid_startMat;
                    break;
                case cellType.end:
                    text = "E";
                    materialToUse = grid_endMat;
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

        yield return new WaitForSeconds(.1f);

        Debug.Log("Creating Main Path...");
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

        int gen1_startIndex = endNode_index;

        Debug.Log("Creating gen1 nodes from Main Path...");
        for (int i = 0; i < mainPath_nodeCount; i++) //step through mainpath nodes, adding halls and nodes.
        {
            SelectAndAddRandomHallsToNode(i, neutral_hProb, 0, 3);
            AddNodesToParentHalls(i);

            yield return null; // Lets Unity process a frame
        }
        current_gen++;
        int gen1_endIndex = endNode_index - 1;


        Debug.Log("gen1 nodes generation complete. gen1 Start Index=" + gen1_startIndex + " End Index=" + gen1_endIndex);
        Debug.Log("node_array endNode_index:" + endNode_index);


        gen1_complete = true;

        StartCoroutine(InstantiateAllNodesCoroutine());
        yield return null; // Lets Unity process a frame

    }


    IEnumerator InstantiateAllNodesCoroutine()
    {
        for (int i = 0; i < endNode_index; i++)
        {
            InstantiateNodeLayout(i);
            yield return null;
        }
        currentlyGenerating = false;


    }

}
