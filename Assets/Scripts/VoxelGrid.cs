using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class VoxelGrid
{
    private Vector3Int _gridDimensions;
    private float _voxelSize;
    private float _margin;

    public int CurrentLayer = 0;
    private Voxel[,,] _grid;

    public VoxelGrid(Vector3Int gridDimensions, float voxelSize, float margin)
    {
        _gridDimensions = gridDimensions;
        _voxelSize = voxelSize;
        _margin = margin;

        InitialiseGrid();
    }

    private void InitialiseGrid()
    {
        _grid = new Voxel[_gridDimensions.x, _gridDimensions.y, _gridDimensions.z];

        //Create grid
        for (int x = 0; x < _gridDimensions.x; x++)
            for (int y = 0; y < _gridDimensions.y; y++)
                for (int z = 0; z < _gridDimensions.z; z++)
                    _grid[x, y, z] = new Voxel(new Vector3Int(x, y, z), _voxelSize, _margin, _gridDimensions);
    }

    public void SetRandomAlive(int PercentageAlive)
    {
        int numberAlive = _gridDimensions.x * _gridDimensions.z * PercentageAlive / 100;

        for (int i = 0; i < numberAlive; i++)
        {
            int x = Random.Range(0, _gridDimensions.x);
            int z = Random.Range(0, _gridDimensions.z);

            if (_grid[x, 0, z].Status.Alive) i--;
            _grid[x, 0, z].Status.Alive = true;
        }
    }

    public void ResetGrid(bool alive)
    {
        foreach (var voxel in _grid)
        {
            voxel.Status.Alive = alive;
            voxel.Status.NextStatus = false;
        }
        CurrentLayer = 0;
    }

    public void InvertGrid()
    {
        foreach (var voxel in _grid)
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
                neighbours.Add(_grid[neighbourIndex.x, neighbourIndex.y, neighbourIndex.z]);
        }

        return neighbours;
    }

    private bool CheckIndex(Vector3Int index)
    {
        if (index.x < 0) return false;
        if (index.y < 0) return false;
        if (index.z < 0) return false;
        if (index.x > _gridDimensions.x - 1) return false;
        if (index.y > _gridDimensions.y - 1) return false;
        if (index.z > _gridDimensions.z - 1) return false;

        return true;
    }

    public void UpdateGrid()
    {

        /*foreach (var voxel in _grid)
        {
            GameOfLifeRules(voxel);
        }*/

        for (int x = 0; x < _gridDimensions.x; x++)
        {
            for (int z = 0; z < _gridDimensions.z; z++)
            {
                //only run game of life on the first layer
                GameOfLifeRules(_grid[x, 0, z]);

                for (int i = CurrentLayer; i > 0; i--)
                {
                    _grid[x, i, z].Status.NextStatus = _grid[x, i - 1, z].Status.Alive;
                }
            }
        }
        CurrentLayer++;


        foreach (var voxel in _grid) voxel.Status.Alive = voxel.Status.NextStatus;


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
}
