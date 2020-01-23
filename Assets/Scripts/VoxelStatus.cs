using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelStatus : MonoBehaviour
{
    public bool Alive
    {
        get
        {
            Debug.Log("Getting the status");
            return _alive;
        }
        set
        {
            Debug.Log("Setting the status");
            this.GetComponent<MeshRenderer>().enabled = value;
            _alive = value;
        }
    }

    private bool _alive = true;
    public bool NextStatus = false;
}
