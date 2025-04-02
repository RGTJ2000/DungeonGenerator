using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GridMeshGenerator : MonoBehaviour
{
    [Header("Cell Materials")]
    [SerializeField] private Material undefinedMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material nodeMaterial;
    [SerializeField] private Material hallMaterial;
    [SerializeField] private Material startMaterial;
    [SerializeField] private Material endMaterial;

    [Header("Mesh Parameters")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float heightOffset = 0.05f; // Small height difference between different cell types
    [SerializeField] private float wallHeight = 0.5f; // Height for wall cells

    private Dictionary<cellType, Material> materialMap;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();

        InitializeMaterialMap();
    }

    private void InitializeMaterialMap()
    {
        materialMap = new Dictionary<cellType, Material>
        {
            { cellType.undef, undefinedMaterial },
            { cellType.wall, wallMaterial },
            { cellType.node, nodeMaterial },
            { cellType.hall, hallMaterial },
            { cellType.start, startMaterial },
            { cellType.end, endMaterial }
        };
    }

    public void GenerateMesh(cellType[,] cellTypeMatrix, Vector2 position)
    {
        if (cellTypeMatrix == null || cellTypeMatrix.GetLength(0) == 0 || cellTypeMatrix.GetLength(1) == 0)
        {
            Debug.LogError("Invalid cellTypeMatrix provided to GridMeshGenerator.");
            return;
        }

        int width = cellTypeMatrix.GetLength(0);
        int height = cellTypeMatrix.GetLength(1);

        // Create a game object to hold the mesh
        transform.position = new Vector3(position.x, 0, position.y);

        // Create submeshes for each cell type
        Dictionary<cellType, List<Vector3>> verticesByCellType = new Dictionary<cellType, List<Vector3>>();
        Dictionary<cellType, List<int>> trianglesByCellType = new Dictionary<cellType, List<int>>();
        Dictionary<cellType, List<Vector2>> uvsByCellType = new Dictionary<cellType, List<Vector2>>();

        foreach (cellType type in System.Enum.GetValues(typeof(cellType)))
        {
            verticesByCellType[type] = new List<Vector3>();
            trianglesByCellType[type] = new List<int>();
            uvsByCellType[type] = new List<Vector2>();
        }

        // Build vertices and triangles for each cell
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                cellType currentCellType = cellTypeMatrix[x, z];
                if (currentCellType == cellType.undef)
                    continue;

                // Calculate cell position - adjusted for origin at lower left
                float posX = x * cellSize;
                float posZ = z * cellSize;

                AddCellToMesh(currentCellType, posX, posZ, verticesByCellType, trianglesByCellType, uvsByCellType);
            }
        }

        // Combine all submeshes
        CombineMeshes(verticesByCellType, trianglesByCellType, uvsByCellType);
    }

    private void AddCellToMesh(cellType type, float x, float z,
        Dictionary<cellType, List<Vector3>> verticesByCellType,
        Dictionary<cellType, List<int>> trianglesByCellType,
        Dictionary<cellType, List<Vector2>> uvsByCellType)
    {
        float y = 0;

        // Set different heights for different cell types
        switch (type)
        {
            case cellType.wall:
                y = wallHeight;
                break;
            case cellType.node:
                y = heightOffset;
                break;
            case cellType.hall:
                y = heightOffset * 0.5f;
                break;
            case cellType.start:
                y = heightOffset * 2f;
                break;
            case cellType.end:
                y = heightOffset * 2f;
                break;
            default:
                y = 0;
                break;
        }

        var vertices = verticesByCellType[type];
        var triangles = trianglesByCellType[type];
        var uvs = uvsByCellType[type];

        int vertexCount = vertices.Count;

        // Top face vertices
        vertices.Add(new Vector3(x, y, z)); // Bottom-left
        vertices.Add(new Vector3(x + cellSize, y, z)); // Bottom-right
        vertices.Add(new Vector3(x, y, z + cellSize)); // Top-left
        vertices.Add(new Vector3(x + cellSize, y, z + cellSize)); // Top-right

        // UVs for top face
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));

        // Top face triangles
        triangles.Add(vertexCount + 0);
        triangles.Add(vertexCount + 2);
        triangles.Add(vertexCount + 1);
        triangles.Add(vertexCount + 1);
        triangles.Add(vertexCount + 2);
        triangles.Add(vertexCount + 3);

        // For walls, add side faces
        if (type == cellType.wall)
        {
            // Add side faces (we need additional vertices for proper UVs)
            // Each side adds 4 vertices and 6 triangles

            // Front face (Z+)
            vertices.Add(new Vector3(x, 0, z + cellSize));
            vertices.Add(new Vector3(x + cellSize, 0, z + cellSize));
            vertices.Add(new Vector3(x, y, z + cellSize));
            vertices.Add(new Vector3(x + cellSize, y, z + cellSize));

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            triangles.Add(vertexCount + 4);
            triangles.Add(vertexCount + 6);
            triangles.Add(vertexCount + 5);
            triangles.Add(vertexCount + 5);
            triangles.Add(vertexCount + 6);
            triangles.Add(vertexCount + 7);

            // Back face (Z-)
            vertices.Add(new Vector3(x + cellSize, 0, z));
            vertices.Add(new Vector3(x, 0, z));
            vertices.Add(new Vector3(x + cellSize, y, z));
            vertices.Add(new Vector3(x, y, z));

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            triangles.Add(vertexCount + 8);
            triangles.Add(vertexCount + 10);
            triangles.Add(vertexCount + 9);
            triangles.Add(vertexCount + 9);
            triangles.Add(vertexCount + 10);
            triangles.Add(vertexCount + 11);

            // Left face (X-)
            vertices.Add(new Vector3(x, 0, z));
            vertices.Add(new Vector3(x, 0, z + cellSize));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + cellSize));

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            triangles.Add(vertexCount + 12);
            triangles.Add(vertexCount + 14);
            triangles.Add(vertexCount + 13);
            triangles.Add(vertexCount + 13);
            triangles.Add(vertexCount + 14);
            triangles.Add(vertexCount + 15);

            // Right face (X+)
            vertices.Add(new Vector3(x + cellSize, 0, z + cellSize));
            vertices.Add(new Vector3(x + cellSize, 0, z));
            vertices.Add(new Vector3(x + cellSize, y, z + cellSize));
            vertices.Add(new Vector3(x + cellSize, y, z));

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            triangles.Add(vertexCount + 16);
            triangles.Add(vertexCount + 18);
            triangles.Add(vertexCount + 17);
            triangles.Add(vertexCount + 17);
            triangles.Add(vertexCount + 18);
            triangles.Add(vertexCount + 19);
        }
    }

    private void CombineMeshes(
       Dictionary<cellType, List<Vector3>> verticesByCellType,
       Dictionary<cellType, List<int>> trianglesByCellType,
       Dictionary<cellType, List<Vector2>> uvsByCellType)
    {
        // First count valid submeshes and collect only non-empty cell types
        List<cellType> validCellTypes = new List<cellType>();
        foreach (cellType type in System.Enum.GetValues(typeof(cellType)))
        {
            if (verticesByCellType[type].Count > 0)
            {
                validCellTypes.Add(type);
            }
        }

        int submeshCount = validCellTypes.Count;
        if (submeshCount == 0)
        {
            Debug.LogWarning("No mesh data to generate.");
            return;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Support for large meshes

        List<Vector3> combinedVertices = new List<Vector3>();
        List<Vector2> combinedUVs = new List<Vector2>();
        List<Material> materials = new List<Material>();

        // First collect all vertices and UVs, but only for valid cell types
        foreach (cellType type in validCellTypes)
        {
            var vertices = verticesByCellType[type];
            var uvs = uvsByCellType[type];

            combinedVertices.AddRange(vertices);
            combinedUVs.AddRange(uvs);
            materials.Add(materialMap[type]);
        }

        // Assign vertices and UVs to the mesh BEFORE setting triangles
        mesh.vertices = combinedVertices.ToArray();
        mesh.uv = combinedUVs.ToArray();

        // Now set the submesh count
        mesh.subMeshCount = submeshCount;

        // Process triangles with proper offsets
        int vertexOffset = 0;

        for (int submeshIndex = 0; submeshIndex < validCellTypes.Count; submeshIndex++)
        {
            cellType type = validCellTypes[submeshIndex];
            var vertices = verticesByCellType[type];
            var triangles = trianglesByCellType[type];

            // Adjust triangle indices with the correct offset
            int[] adjustedTriangles = new int[triangles.Count];
            for (int i = 0; i < triangles.Count; i++)
            {
                adjustedTriangles[i] = triangles[i] + vertexOffset;
            }

            // Set submesh triangles
            mesh.SetTriangles(adjustedTriangles, submeshIndex);

            vertexOffset += vertices.Count;
        }

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        meshRenderer.materials = materials.ToArray();
        meshCollider.sharedMesh = mesh;
    }



    // Helper method to clear the mesh if needed
    public void ClearMesh()
    {
        if (meshFilter != null && meshFilter.mesh != null)
        {
            meshFilter.mesh.Clear();
        }
    }
}