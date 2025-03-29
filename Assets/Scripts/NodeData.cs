using UnityEngine;

public struct WallsOffset
{
    public int north;
    public int south;
    public int east;
    public int west;

    public WallsOffset(int n, int s, int e, int w)
    {
        north = n; 
        south = s; 
        east = e; 
        west = w;
    }

}

public struct HallStats
{
    public int length;
    public int width;
    public int offsetFromNodeCenter;
    public bool isConnected;

    public HallStats(int l, int w, int offset, bool connected)
    {
        length = l;
        width = w;
        offsetFromNodeCenter = offset;
        isConnected = connected;
    }
}

public struct HallsData
{
    /*
    public Vector3 north;  //Vector 3 represents (length, width, x offset from node center). Hall start is determined by node center and wall offset.
    public Vector3 south;  //Vector 3 represents (length, width, x offset from node center)
    public Vector3 east;   //Vector 3 represents (length, width, z offset from node center)
    public Vector3 west;   //Vector 3 represents (length, width, z offset from node center)
    */

    public HallStats north;
    public HallStats south;
    public HallStats east;
    public HallStats west;

    public HallsData(HallStats n, HallStats s, HallStats e, HallStats w)
    {
        north = n;
        south = s;
        east = e;
        west = w;
    }
}

public class NodeData
{
    public int parentNode_index;

    public Vector2 offsetFromParentNode;
    public int directionIndexFromParent;
    public WallsOffset walls_offset;

    public HallsData halls_data;

    public int genIndex;

    public NodeData(int parent, Vector2 offset, int direction, WallsOffset walls, HallsData halls, int gen )
    {
        this.parentNode_index = parent;
        this.offsetFromParentNode = offset;
        this.directionIndexFromParent = direction;
        this.walls_offset = walls;
        this.halls_data = halls;
        this.genIndex = gen;
    }
     
}
