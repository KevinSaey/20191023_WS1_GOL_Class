using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public Vector3Int Index;
    public VoxelStatus Status;
    GameObject _goVoxel;

    public Voxel(Vector3Int index, float voxelSize, float margin, Vector3Int gridDimension)
    {
        Index = index;
        _goVoxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _goVoxel.name = $"Voxel {Index}";
        _goVoxel.tag = "Voxel";
        _goVoxel.transform.localScale = Vector3.one * voxelSize;

        //move the grid to the centre of the camera
        Vector3 startingPoint = -(Vector3)gridDimension * (voxelSize + margin) / 2;
        _goVoxel.transform.position = startingPoint + (Vector3)Index * (voxelSize + margin);
        Status = _goVoxel.AddComponent<VoxelStatus>();
        Status.Alive = false;
    }
}

