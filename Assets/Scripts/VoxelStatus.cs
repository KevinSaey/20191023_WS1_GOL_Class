using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelStatus : MonoBehaviour
{
    public Voxel Daddy;
    public bool Alive
    {
        get
        {
            return _alive;
        }
        set
        {
            this.GetComponent<MeshRenderer>().enabled = value;
            _alive = value;
        }
    }

    public bool Collider
    {
        set
        {
            this.GetComponent<BoxCollider>().enabled = value;
        }
    }

    public Material GOMaterial
    {
        set
        {
            this.GetComponent<MeshRenderer>().material = value;
        }
    }

    public Mesh VoxelMesh
    {
        set
        {
            this.GetComponent<MeshFilter>().mesh = value;
        }
    }


    private bool _alive = true;
    public bool NextStatus = false;
}
