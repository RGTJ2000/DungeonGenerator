using UnityEngine;

public class Instantiator : MonoBehaviour
{

    [SerializeField]private GameObject wallUnit_prefab;
    [SerializeField]private GameObject nodeFloor;
    [SerializeField] private GameObject hallFloor;
    [SerializeField] private GameObject startFloor;
    [SerializeField] private GameObject endFloor;

    private Vector3 center_position = Vector3.zero;
    [SerializeField] float floorElevation = 1f;

    private GameObject PrefabContainer;
    private GameObject floor;

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

    public void DestroyFloorAndWalls()
    {
        foreach (Transform child in PrefabContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Destroy(floor);

        isInstantiated = false;
    }
}
