using UnityEngine;

public class Instantiator : MonoBehaviour
{
    [SerializeField] private GameObject player_prefab;
    [SerializeField]private GameObject wallUnit_prefab;
    [SerializeField]private GameObject nodeFloor;
    [SerializeField] private GameObject hallFloor;
    [SerializeField] private GameObject startFloor;
    [SerializeField] private GameObject endFloor;

    private Vector3 center_position = Vector3.zero;
    [SerializeField] float floorElevation = 1f;

    private GameObject PrefabContainer;
    private GameObject floor;

    private GameObject player;

    public bool isInstantiated = false;

    private void Start()
    {
        PrefabContainer = new GameObject("PrefabContainer");
    }

    public void InstantiateDungeon(cellType[,] cell_matrix)
    {
        int matrixWidth = cell_matrix.GetLength(0);
        int matrixHeight = cell_matrix.GetLength(1);

        //instantiate floor
        //floor = Instantiate(floorUnit_prefab, new Vector3(center_position.x, floorElevation, center_position.z), Quaternion.identity);
        //floor.transform.localScale = new Vector3(matrixWidth, floor.transform.localScale.y, matrixHeight);

        //instantiate walls
        Vector3 start_pos = new Vector3(center_position.x - (float)matrixWidth / 2, 0, center_position.z - (float)matrixHeight / 2);

        for (int j = 0; j < matrixHeight; j++)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                Vector3 spawnPosition = new Vector3(start_pos.x + i, floorElevation, start_pos.z + j);

                switch (cell_matrix[i, j])
                {
                    case cellType.wall:
                        GameObject newWall = Instantiate(wallUnit_prefab, spawnPosition + wallUnit_prefab.transform.position, Quaternion.identity);
                        newWall.transform.SetParent(PrefabContainer.transform, true);

                        break;
                    case cellType.start:
                        GameObject newStartFloor = Instantiate(startFloor, spawnPosition, Quaternion.identity);
                        newStartFloor.transform.SetParent(PrefabContainer.transform, true);
                        break;
                    case cellType.end:
                        GameObject newEndFloor = Instantiate(endFloor, spawnPosition, Quaternion.identity);
                        newEndFloor.transform.SetParent(PrefabContainer.transform, true);
                        break;
                    case cellType.node:
                        GameObject newNodeFloor = Instantiate(nodeFloor, spawnPosition, Quaternion.identity);
                        newNodeFloor.transform.SetParent(PrefabContainer.transform, true);
                        break;
                    case cellType.hall:
                        GameObject newHallFloor = Instantiate(hallFloor, spawnPosition, Quaternion.identity);
                        newHallFloor.transform.SetParent(PrefabContainer.transform, true);
                        break;


                    default:
                        break;

                }
            }


        }

        isInstantiated = true;
    }

    public void DestroyFloorAndWallsAndPlayer()
    {
        foreach (Transform child in PrefabContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Destroy(floor);

        if (player != null)
        {
            Destroy(player);
        }

        isInstantiated = false;
    }

    public GameObject PlacePlayer(cellType[,] cell_matrix)
    {
        Vector3 worldStartPosition = GetStartNodePosition(cell_matrix);

        player = Instantiate(player_prefab, worldStartPosition, Quaternion.identity);

        return player;
    }

    private Vector3 GetStartNodePosition(cellType[,] cell_matrix)
    {
        Vector3 worldStart = Vector3.zero;

        int minX = cell_matrix.GetLength(0)-1;
        int maxX = 0;
        int minY = cell_matrix.GetLength(1)-1;
        int maxY = 0;

        for (int j=0; j<cell_matrix.GetLength(1); j++)
        {
            for (int i=0; i<cell_matrix.GetLength(0); i++)
            {
                if (cell_matrix[i,j] == cellType.start)
                {
                    if (i < minX) minX = i;
                    if (i > maxX) maxX = i;
                    
                    if (j < minY) minY = j;
                    if(j > maxY) maxY = j;


                }

            }
        }

        int centerX = (minX + maxX) / 2;
        int centerY = (minY + maxY) / 2;

        worldStart = new Vector3(center_position.x - cell_matrix.GetLength(0)/2 + centerX, floorElevation+1, center_position.z - cell_matrix.GetLength(1)/2 + centerY);

        Debug.Log($"minX/maxX= {minX}/{maxX} | minY/maxY= {minY}/{maxY} | center pos ({centerX},{centerY})");
        return worldStart;


    }
}
