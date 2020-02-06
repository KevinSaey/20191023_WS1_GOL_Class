using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Voxel
{
    public Vector3Int Index;
    public bool VoxelStatus;
    GameObject _goVoxel;
    public VoxelStatus Status;
    public List<Face> Faces = new List<Face>(6);

    public int NumberOfAliveFaces => Faces.Where(f => f.IsActive).Count();

    public bool IsSkin => Status.Alive && Faces.Any(f => f.IsSkin);

    public Voxel(Vector3Int index, float voxelSize, float margin, Vector3Int gridDimension, Vector3 startingPoint)
    {
        Index = index;
        CreateVoxelGameObject(voxelSize, margin, gridDimension, startingPoint);
    }

    public void CreateVoxelGameObject(float voxelSize, float margin, Vector3Int gridDimension, Vector3 startingPoint)
    {
        _goVoxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _goVoxel.name = $"Voxel {Index}";
        _goVoxel.tag = "Voxel";
        _goVoxel.transform.localScale = Vector3.one * voxelSize;

        _goVoxel.transform.position = startingPoint + (Vector3)Index * (voxelSize + margin);
        Status = _goVoxel.AddComponent<VoxelStatus>();
        Status.Alive = false;
        Status.Daddy = this;
    }


}
