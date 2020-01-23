using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid
{
    Vector3Int _gridDimensions;
    float _voxelSize;
    float _margin;
    bool _twoD = true;

    Voxel[,,] _grid;

    public VoxelGrid(Vector3Int gridDimensions, float voxelSize, float margin)
    {
        _gridDimensions = gridDimensions;
        _voxelSize = voxelSize;
        _margin = margin;

        InitialiseGrid();
    }

    private void InitialiseGrid()
    {
        //Here we will create our grid

        _grid = new Voxel[_gridDimensions.x, _gridDimensions.y, _gridDimensions.z];

        for (int x = 0; x < _gridDimensions.x; x++)
        {
            for (int y = 0; y < _gridDimensions.y; y++)
            {
                for (int z = 0; z < _gridDimensions.z; z++)
                {
                    _grid[x, y, z] = new Voxel(new Vector3Int(x, y, z), _voxelSize, _margin, _gridDimensions);
                }
            }
        }
    }

    public void SetRandomAlive(int PercentageAlive)
    {
        int numberAlive = _gridDimensions.x * _gridDimensions.y * _gridDimensions.z * PercentageAlive / 100;

        for (int i = 0; i < numberAlive; i++)
        {
            int x = Random.Range(0, _gridDimensions.x);
            int y = Random.Range(0, _gridDimensions.y);
            int z = Random.Range(0, _gridDimensions.z);

            if (_grid[x, y, z].Status.Alive) i--;
            else _grid[x, y, z].Status.Alive = true;
        }
    }

    List<Voxel> GetNeighbours(Vector3Int index)
    {
        List<Voxel> neighbours = new List<Voxel>();
        List<Vector3Int> directions = new List<Vector3Int>
        {
            new Vector3Int(1, 0, 0),//East
            new Vector3Int(1, 0, -1),//SouthEast
            new Vector3Int(0, 0, -1),//South
            new Vector3Int(-1, 0, -1),//SouthWest
            new Vector3Int(-1, 0, 0),//West
            new Vector3Int(-1, 0, 1),//NorthWest
            new Vector3Int(0, 0, 1),//North
            new Vector3Int(1, 0, 1)//NorthEast
        };
        //up
        if (!_twoD)
        {
            directions.AddRange(new List<Vector3Int>{
            new Vector3Int(1, 1, 0),//East
            new Vector3Int(1, 1, -1),//SouthEast
            new Vector3Int(0, 1, -1),//South
            new Vector3Int(-1, 1, -1),//SouthWest
            new Vector3Int(-1, 1, 0),//West
            new Vector3Int(-1, 1, 1),//NorthWest
            new Vector3Int(0, 1, 1),//North
            new Vector3Int(1, 1, 1),//NorthEast
            //down
            new Vector3Int(1, -1, 0),//East
            new Vector3Int(1, -1, -1),//SouthEast
            new Vector3Int(0, -1, -1),//South
            new Vector3Int(-1, -1, -1),//SouthWest
            new Vector3Int(-1, -1, 0),//West
            new Vector3Int(-1, -1, 1),//NorthWest
            new Vector3Int(0, -1, 1),//North
            new Vector3Int(1, -1, 1)//NorthEast
            });
        }


        foreach (var direction in directions)
        {
            Vector3Int neighbourIndex = index + direction;

            if (CheckIndex(neighbourIndex))
            {
                neighbours.Add(_grid[neighbourIndex.x, neighbourIndex.y, neighbourIndex.z]);
            }
        }

        return neighbours;
    }

    bool CheckIndex(Vector3Int neighbourIndex)
    {
        if (neighbourIndex.x < 0) return false;
        if (neighbourIndex.y < 0) return false;
        if (neighbourIndex.z < 0) return false;
        if (neighbourIndex.x > _gridDimensions.x - 1) return false;
        if (neighbourIndex.y > _gridDimensions.y - 1) return false;
        if (neighbourIndex.z > _gridDimensions.z - 1) return false;
        return true;
    }

    int GetNumberOfAliveNeighbours(List<Voxel> neighbours)
    {
        int numberOfAliveNeighbours = 0;

        foreach (var neighbour in neighbours)
        {
            if (neighbour.Status.Alive) numberOfAliveNeighbours++;
        }

        return numberOfAliveNeighbours;
    }

    public void UpdateGrid()
    {
        foreach (var voxel in _grid)
        {
            int nrOfALiveNeighbours;

            nrOfALiveNeighbours = GetNumberOfAliveNeighbours(GetNeighbours(voxel.Index));

            if (_twoD)
            {
                if (nrOfALiveNeighbours < 2) voxel.Status.NextState = false;
                else if (nrOfALiveNeighbours > 3) voxel.Status.NextState = false;
                else if (nrOfALiveNeighbours == 3) voxel.Status.NextState = true;
                else voxel.Status.NextState = voxel.Status.Alive;
            }
            else
            {
                if (nrOfALiveNeighbours < 4) voxel.Status.NextState = false;
                else if (nrOfALiveNeighbours > 5) voxel.Status.NextState = false;
                else if (nrOfALiveNeighbours == 5) voxel.Status.NextState = true;
                else voxel.Status.NextState = voxel.Status.Alive;
            }
        }

        foreach (var voxel in _grid) voxel.Status.Alive = voxel.Status.NextState;

    }
}
