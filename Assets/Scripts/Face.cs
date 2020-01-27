using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using QuickGraph;

public class Face
{
    public enum BoundaryType { Inside = 0, Left = -1, Right = 1, Outside = 2 }

    public Voxel[] Voxels;
    public Vector3Int Index;
    public Vector3 Center;
    public Axis Direction;

    public Edge[] Edges => GetEdges();
    public Corner[] Corners => GetCorners();

    VoxelGrid _grid;

    public bool IsActive => Voxels.Count(v => v != null && v.Status.Alive) == 2;

    public BoundaryType Boundary
    {
        get
        {
            bool left = Voxels[0]?.Status.Alive == true;
            bool right = Voxels[1]?.Status.Alive == true;

            if (!left && right) return BoundaryType.Left;
            if (left && !right) return BoundaryType.Right;
            if (left && right) return BoundaryType.Inside;
            return BoundaryType.Outside;
        }
    }

    public Vector3 Normal
    {
        get
        {
            int f = (int)Boundary;
            if (Boundary == BoundaryType.Outside) f = 0;

            if (Index.y == 0 && Direction == Axis.Y)
            {
                f = Boundary == BoundaryType.Outside ? 1 : 0;
            }

            switch (Direction)
            {
                case Axis.X:
                    return Vector3.right * f;
                case Axis.Y:
                    return Vector3.up * f;
                case Axis.Z:
                    return Vector3.forward * f;
                default:
                    throw new Exception("Wrong direction.");
            }
        }
    }

    public bool IsSkin
    {
        get
        {
            if (Index.y == 0 && Direction == Axis.Y)
            {
                return Boundary == BoundaryType.Outside;
            }

            return Boundary == BoundaryType.Left || Boundary == BoundaryType.Right;
        }

    }

    public Face(int x, int y, int z, Axis direction, VoxelGrid grid)
    {
        _grid = grid;
        Index = new Vector3Int(x, y, z);
        Direction = direction;
        Voxels = GetVoxels();

        /*foreach (var v in Voxels)
        {
            if (v != null) v.Faces.Add(this);
        }*/

        foreach (var v in Voxels.Where(v => v != null))//This is using Linq
            v.Faces.Add(this);

        Center = GetCenter();
    }

    Vector3 GetCenter()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        switch (Direction)
        {
            case Axis.X:
                return _grid.Corner + new Vector3(x, y + 0.5f, z + 0.5f) * _grid.VoxelSize;
            case Axis.Y:
                return _grid.Corner + new Vector3(x + 0.5f, y, z + 0.5f) * _grid.VoxelSize;
            case Axis.Z:
                return _grid.Corner + new Vector3(x + 0.5f, y + 0.5f, z) * _grid.VoxelSize;
            default:
                throw new Exception("Wrong direction.");
        }
    }

    Voxel[] GetVoxels()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        int test = 3;

        if (test == 0)
        {
            Debug.Log("test is 0");
        }
        else if (test == 2)
        {
            Debug.Log("test is 2");
        }
        else if (test == 7)
        {
            Debug.Log("test is 7");
        }


        switch (Direction)
        {
            case Axis.X:
                return new[]
                {
                   x == 0 ? null : _grid.Voxels[x - 1, y, z],
                   x == _grid.GridDimensions.x ? null : _grid.Voxels[x, y, z]
                };
            case Axis.Y:
                return new[]
                {
                   y == 0 ? null : _grid.Voxels[x, y - 1, z],
                   y == _grid.GridDimensions.y ? null : _grid.Voxels[x, y, z]
                };
            case Axis.Z:
                /*Voxel[] voxels = new Voxel[2];//This is exactly the same as the code below

                if (z == 0) voxels[0] = null;
                else voxels[0] = _grid.Voxels[x, y, z - 1];

                if (z == _grid.GridDimensions.z) voxels[1] = null;
                else voxels[1] = _grid.Voxels[x, y, z];

                return voxels;*/

                return new[]
                    {

                   z == 0 ? null : _grid.Voxels[x, y, z - 1],
                   z == _grid.GridDimensions.z ? null : _grid.Voxels[x, y, z]
                };
            default:
                throw new Exception("Wrong direction.");
        }
    }

    Edge[] GetEdges()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        switch (Direction)
        {
            case Axis.X:
                return new[]
                {
                  _grid.Edges[1][x, y, z],
                  _grid.Edges[1][x, y, z + 1],
                  _grid.Edges[2][x, y, z],
                  _grid.Edges[2][x, y + 1, z]
                };
            case Axis.Y:
                return new[]
                {
                  _grid.Edges[0][x, y, z],
                  _grid.Edges[0][x, y, z + 1],
                  _grid.Edges[2][x, y, z],
                  _grid.Edges[2][x + 1, y, z]
                };
            case Axis.Z:
                return new[]
                {
                  _grid.Edges[0][x, y, z],
                  _grid.Edges[0][x, y + 1, z],
                  _grid.Edges[1][x, y, z],
                  _grid.Edges[1][x + 1, y, z]
                };
            default:
                throw new Exception("Wrong direction.");
        }
    }

    Corner[] GetCorners()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;

        switch (Direction)
        {
            case Axis.X:
                return new[]
                {
                 _grid.Corners[x, y, z],
                 _grid.Corners[x, y + 1, z],
                 _grid.Corners[x, y, z + 1],
                 _grid.Corners[x, y + 1, z + 1]
                };
            case Axis.Y:
                return new[]
                {
                 _grid.Corners[x, y, z],
                 _grid.Corners[x + 1, y, z],
                 _grid.Corners[x, y, z + 1],
                 _grid.Corners[x + 1, y, z + 1]
                };
            case Axis.Z:
                return new[]
                {
                 _grid.Corners[x, y, z],
                 _grid.Corners[x + 1, y, z],
                 _grid.Corners[x, y + 1, z],
                 _grid.Corners[x + 1, y + 1, z]
                };
            default:
                throw new Exception("Wrong direction.");
        }
    }
}
