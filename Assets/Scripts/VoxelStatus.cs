using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelStatus : MonoBehaviour
{
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

    private bool _alive = true;
    public bool NextStatus = false;
}
