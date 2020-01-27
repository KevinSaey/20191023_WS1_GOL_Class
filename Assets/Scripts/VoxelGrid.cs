using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

using System.Linq;
using QuickGraph;

public class VoxelGrid
{
    public Vector3Int GridDimensions;
    public float VoxelSize;
    public float Margin;

    public int CurrentLayer = 0;
    private UndirectedGraph<Voxel, Edge<Voxel>> _graph = new UndirectedGraph<Voxel, Edge<Voxel>>();
    private Material[] _materials;
    private Mesh _voxelMesh;


    public Voxel[,,] Voxels;
    public Corner[,,] Corners;
    public Face[][,,] Faces = new Face[3][,,];
    public Edge[][,,] Edges = new Edge[3][,,];

    public Vector3 Corner;

    public VoxelGrid(Vector3Int gridDimensions, float voxelSize, float margin, Material[] materials, Mesh voxelMesh)
    {
        _materials = materials;
        _voxelMesh = voxelMesh;
        GridDimensions = gridDimensions;
        VoxelSize = voxelSize;
        Margin = margin;
        

        Corner = -new Vector3(gridDimensions.x, -voxelSize / 2, gridDimensions.y) * (VoxelSize + Margin) / 2;

        InitialiseGrid();
    }

    private void InitialiseGrid()
    {
        var watch = Stopwatch.StartNew();
        Voxels = new Voxel[GridDimensions.x, GridDimensions.y, GridDimensions.z];

        MakeVoxels();
        MakeCorners();
        MakeFaces();
        MakeEdges();

        Debug.Log($"Grid took: {watch.ElapsedMilliseconds} ms to create.\r\nGrid GridDimensions: {GridDimensions}, {GridDimensions.x * GridDimensions.y * GridDimensions.z} voxels.");
    }

    private void MakeVoxels()
    {
        //Create grid
        for (int x = 0; x < GridDimensions.x; x++)
            for (int y = 0; y < GridDimensions.y; y++)
                for (int z = 0; z < GridDimensions.z; z++)
                    Voxels[x, y, z] = new Voxel(new Vector3Int(x, y, z), VoxelSize, Margin, GridDimensions, Corner);
    }

    private void MakeCorners()
    {
        // make corners
        Corners = new Corner[GridDimensions.x + 1, GridDimensions.y + 1, GridDimensions.z + 1];

        for (int x = 0; x < GridDimensions.x + 1; x++)
            for (int y = 0; y < GridDimensions.y + 1; y++)
                for (int z = 0; z < GridDimensions.z + 1; z++)
                {
                    Corners[x, y, z] = new Corner(new Vector3Int(x, y, z), this);
                }
    }

    private void MakeFaces()
    {
        // make faces
        Faces[0] = new Face[GridDimensions.x + 1, GridDimensions.y, GridDimensions.z];

        for (int x = 0; x < GridDimensions.x + 1; x++)
            for (int y = 0; y < GridDimensions.y; y++)
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    Faces[0][x, y, z] = new Face(x, y, z, Axis.X, this);
                }

        Faces[1] = new Face[GridDimensions.x, GridDimensions.y + 1, GridDimensions.z];

        for (int x = 0; x < GridDimensions.x; x++)
            for (int y = 0; y < GridDimensions.y + 1; y++)
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    Faces[1][x, y, z] = new Face(x, y, z, Axis.Y, this);
                }

        Faces[2] = new Face[GridDimensions.x, GridDimensions.y, GridDimensions.z + 1];

        for (int x = 0; x < GridDimensions.x; x++)
            for (int y = 0; y < GridDimensions.y; y++)
                for (int z = 0; z < GridDimensions.z + 1; z++)
                {
                    Faces[2][x, y, z] = new Face(x, y, z, Axis.Z, this);
                }
    }

    private void MakeEdges()
    {
        Edges[2] = new Edge[GridDimensions.x + 1, GridDimensions.y + 1, GridDimensions.z];

        for (int x = 0; x < GridDimensions.x + 1; x++)
            for (int y = 0; y < GridDimensions.y + 1; y++)
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    Edges[2][x, y, z] = new Edge(x, y, z, Axis.Z, this);
                }

        Edges[0] = new Edge[GridDimensions.x, GridDimensions.y + 1, GridDimensions.z + 1];

        for (int x = 0; x < GridDimensions.x; x++)
            for (int y = 0; y < GridDimensions.y + 1; y++)
                for (int z = 0; z < GridDimensions.z + 1; z++)
                {
                    Edges[0][x, y, z] = new Edge(x, y, z, Axis.X, this);
                }

        Edges[1] = new Edge[GridDimensions.x + 1, GridDimensions.y, GridDimensions.z + 1];

        for (int x = 0; x < GridDimensions.x + 1; x++)
            for (int y = 0; y < GridDimensions.y; y++)
                for (int z = 0; z < GridDimensions.z + 1; z++)
                {
                    Edges[1][x, y, z] = new Edge(x, y, z, Axis.Y, this);
                }
    }

    public void SetRandomAlive(int PercentageAlive)
    {
        int numberAlive = GridDimensions.x * GridDimensions.z * PercentageAlive / 100;

        for (int i = 0; i < numberAlive; i++)
        {
            int x = Random.Range(0, GridDimensions.x);
            int z = Random.Range(0, GridDimensions.z);

            if (Voxels[x, 0, z].Status.Alive) i--;
            Voxels[x, 0, z].Status.Alive = true;
        }
    }

    public void ResetGrid(bool alive)
    {
        foreach (var voxel in Voxels)
        {
            voxel.Status.Alive = alive;
            voxel.Status.NextStatus = false;
        }
        CurrentLayer = 0;
    }

    public void InvertGrid()
    {
        foreach (var voxel in Voxels)
        {
            voxel.Status.Alive = !voxel.Status.Alive;
        }
    }

    private List<Voxel> GetNeighbours(Vector3Int index)
    {
        List<Voxel> neighbours = new List<Voxel>();
        List<Vector3Int> directions = new List<Vector3Int>()
        {
            new Vector3Int (1,0,0),//East,
            new Vector3Int (1,0,-1),//SouthEast,
            new Vector3Int (0,0,-1),//South,
            new Vector3Int (-1,0,-1),//SouthWest,
            new Vector3Int (-1,0,0),//West,
            new Vector3Int (-1,0,1),//NorthWest,
            new Vector3Int (0,0,1),//North,
            new Vector3Int (1,0,1)//NorthEast,
        };

        foreach (var direction in directions)
        {
            Vector3Int neighbourIndex = index + direction;
            if (CheckIndex(neighbourIndex))
                neighbours.Add(Voxels[neighbourIndex.x, neighbourIndex.y, neighbourIndex.z]);
        }

        return neighbours;
    }

    private bool CheckIndex(Vector3Int index)
    {
        if (index.x < 0) return false;
        if (index.y < 0) return false;
        if (index.z < 0) return false;
        if (index.x > GridDimensions.x - 1) return false;
        if (index.y > GridDimensions.y - 1) return false;
        if (index.z > GridDimensions.z - 1) return false;

        return true;
    }

    public void UpdateGrid()
    {
        for (int x = 0; x < GridDimensions.x; x++)
        {
            for (int z = 0; z < GridDimensions.z; z++)
            {
                //only run game of life on the first layer
                GameOfLifeRules(Voxels[x, 0, z]);

                for (int i = CurrentLayer; i > 0; i--)
                {
                    Voxels[x, i, z].Status.NextStatus = Voxels[x, i - 1, z].Status.Alive;
                }
            }
        }
        CurrentLayer++;
        
        foreach (var voxel in Voxels) voxel.Status.Alive = voxel.Status.NextStatus;

        ColorVoxels();
    }

    private void GameOfLifeRules(Voxel voxel)
    {
        int nrOfAliveNeighbours;

        nrOfAliveNeighbours = GetNumberOfAliveNeighbours(GetNeighbours(voxel.Index));

        if (nrOfAliveNeighbours < 2) voxel.Status.NextStatus = false;
        else if (nrOfAliveNeighbours > 3) voxel.Status.NextStatus = false;
        else if (nrOfAliveNeighbours == 3) voxel.Status.NextStatus = true;
    }

    private int GetNumberOfAliveNeighbours(List<Voxel> neighbours)
    {
        int nrOfAliveNeighbours = 0;
        foreach (var vox in neighbours)
        {
            if (vox.Status.Alive) nrOfAliveNeighbours++;
        }

        return nrOfAliveNeighbours;
    }

    private IEnumerable<Voxel> GetVoxels()
    {
        for (int x = 0; x < GridDimensions.x; x++)
            for (int y = 0; y < GridDimensions.y; y++)
                for (int z = 0; z < GridDimensions.z; z++)
                    yield return Voxels[x, y, z];
    }

    private IEnumerable<Face> GetFaces()
    {
        for (int n = 0; n < 3; n++)
        {
            int xSize = Faces[n].GetLength(0);
            int ySize = Faces[n].GetLength(1);
            int zSize = Faces[n].GetLength(2);

            for (int x = 0; x < xSize; x++)
                for (int y = 0; y < ySize; y++)
                    for (int z = 0; z < zSize; z++)
                    {
                        yield return Faces[n][x, y, z];
                    }
        }
    }

    //Explain linq
    public void RemoveSingleVoxels()
    {
        var singleVoxels = GetVoxels().Where(v => v.Status.Alive && v.NumberOfAliveFaces == 0);
        foreach (var vox in singleVoxels) vox.Status.Alive = false;
    }


    //public void RemoveSingleVoxels()
    //{
    //    var singleVoxels = GetVoxels().Where(v =>v.Status.Alive&& v.Faces.Where(f => f.IsActive).Count() == 0);
    //    foreach (var vox in singleVoxels) vox.Status.Alive = false;


    //    /*foreach (var voxel in GetVoxels())
    //    {
    //        int activeFaces = 0;
    //        foreach (var face in voxel.Faces)
    //        {
    //            if (face.IsActive) activeFaces++;
    //        }

    //        if (activeFaces == 0) voxel.Status.Alive = false;
    //    }*/

    //}

    public void CreateGraph()
    {
        _graph.AddVertexRange(GetVoxels().Where(v => v.Status.Alive));
        _graph.AddEdgeRange(GetFaces().Where(f => f.IsActive).Select(f => new Edge<Voxel>(f.Voxels[0], f.Voxels[1])));
    }

    public void ColorVoxels()
    {
        foreach (var vox in Voxels)
        {
            vox.Status.GOMaterial = _materials[vox.NumberOfAliveFaces];
            vox.Status.VoxelMesh = _voxelMesh;
        }
    }
}
