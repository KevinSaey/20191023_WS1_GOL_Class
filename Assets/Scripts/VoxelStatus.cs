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
            _alive = value;
            GetComponent<MeshRenderer>().enabled = _alive;
        }
    }

    bool _alive = true;
    public bool NextState;
}
