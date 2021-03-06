﻿using System.Collections.Generic;
using UnityEngine;


    public class Corner
    {
        public Vector3 Position;
        public Vector3Int Index;
        protected VoxelGrid _grid;

        public Corner(Vector3Int index, VoxelGrid grid)
        {
            _grid = grid;
            Index = index;
            Position = grid.Corner + new Vector3(index.x, index.y, index.z) * (grid.VoxelSize+grid.Margin);
        }

        public Corner(Corner corner) : this(corner.Index, corner._grid)
        {
            _grid.Corners[corner.Index.x, corner.Index.y, corner.Index.z] = this;
        }

        public IEnumerable<Voxel> GetConnectedVoxels()
        {
            for (int zi = -1; zi <= 0; zi++)
            {
                int z = zi + Index.z;
                if (z == -1 || z == _grid.GridDimensions.z) continue;

                for (int yi = -1; yi <= 0; yi++)
                {
                    int y = yi + Index.y;
                    if (y == -1 || y == _grid.GridDimensions.y) continue;

                    for (int xi = -1; xi <= 0; xi++)
                    {
                        int x = xi + Index.x;
                        if (x == -1 || x == _grid.GridDimensions.x) continue;

                        var i = new Vector3Int(x, y, z);

                        yield return _grid.Voxels[x, y, z];
                    }
                }
            }
        }
    
}