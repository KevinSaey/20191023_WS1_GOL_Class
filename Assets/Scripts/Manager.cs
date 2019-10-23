using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    Vector3Int _gridDimensions;
    [SerializeField]
    float _voxelSize;
    [SerializeField]
    float _margin;

    VoxelGrid _grid;
    // Start is called before the first frame update
    void Start()
    {
        _grid = new VoxelGrid(_gridDimensions, _voxelSize, _margin);
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // the object identified by hit.transform was clicked
                // do whatever you want
                if (hit.transform.tag == "Voxel")
                {
                    VoxelStatus status = hit.transform.gameObject.GetComponent<VoxelStatus>();
                    status.Alive = !status.Alive;
                }
            }
        }
    }

    private void OnGUI()
    {
        int padding = 5;
        int buttonHeight = 30;
        int buttonWidth = 170;
        int buttonCounter = 0;

        if (GUI.Button(new Rect(padding, padding + ((buttonHeight+padding) * buttonCounter++), buttonWidth, buttonHeight), "Generate Random Alive"))
        {
            _grid.SetRandomAlive(50);
        }

        if (GUI.Button(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
            buttonWidth, buttonHeight), "Start Game of Life"))
        {
            StartCoroutine(RunGameOfLife(0.1f));
        }
    }

    IEnumerator RunGameOfLife(float time)
    {
        while (true)
        {
            _grid.UpdateGrid();

            yield return new WaitForSeconds(time);
        }
    }
}
