using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private VoxelGrid _grid;
    [SerializeField]
    private Vector3Int _gridDimension;
    [SerializeField]
    private float _voxelSize;
    [SerializeField]
    private float _margin;
    [SerializeField]
    private float _speed = 0.1f;
    [SerializeField]
    private Material[] _materials = new Material[6];
    [SerializeField]
    private Mesh _voxelMesh;
    [SerializeField]
    private Material _highlightMat;

    private bool _running = false;
    private bool _shortestPath = false;
    private bool _toggleDirty = false;
    private IEnumerator _runGameOfLife;
    private Voxel _startVox, _endVox;

    // Start is called before the first frame update
    void Start()
    {
        _grid = new VoxelGrid(_gridDimension, _voxelSize, _margin, _materials, _voxelMesh);
        _runGameOfLife = RunGameOfLife();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Voxel")
                {
                    GameObject hitObject = hit.transform.gameObject;
                    var status = hitObject.GetComponent<VoxelStatus>();

                    if (_shortestPath)
                    {

                        _grid.ColorVoxels(status.Daddy, _highlightMat);

                        if (_startVox == null)
                            _startVox = status.Daddy;
                        else
                            _endVox = status.Daddy;
                    }
                    else
                    {
                        status.Alive = !status.Alive;
                    }
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

        if (GUI.Button(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
            buttonWidth, buttonHeight), "Generate Random Alive"))
        {
            _grid.ResetGrid(false);
            _grid.SetRandomAlive(50);

        }
        if (GUI.Button(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
            buttonWidth, buttonHeight), "Invert grid"))
        {
            _grid.InvertGrid();
        }

        if (GUI.Button(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
            buttonWidth, buttonHeight),
            _running ? "Stop game of life" : "Start game of life"))
        {
            if (_running)
            {
                StopCoroutine(_runGameOfLife);
            }
            else
            {
                StartCoroutine(_runGameOfLife);
            }
            _running = !_running;
        }
        _speed = GUI.HorizontalSlider(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
            buttonWidth, buttonHeight), _speed, 0.5f, 0.001f);

        if (_grid != null)
            GUI.Label(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
                buttonWidth, buttonHeight), (_grid.CurrentLayer).ToString());

        if (_grid != null && GUI.Button(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
        buttonWidth, buttonHeight),
         "Remove single voxels"))
        {
            _grid.RemoveSingleVoxels();
        }

        if (_grid != null && !_shortestPath && _startVox == null && _endVox == null &&
        GUI.Button(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
        buttonWidth, buttonHeight), "Select voxel"))
        {
            _shortestPath = true;
            _grid.ToggleColliderInactive(false);
        }

        if (_grid != null && _shortestPath && _startVox != null && _endVox != null &&
            GUI.Button(new Rect(padding, padding + ((buttonHeight + padding) * buttonCounter++),
            buttonWidth, buttonHeight), "Generate shortest path"))
        {
            List<Voxel> shortestPath = PathFinding.CalculateShortestPath(_grid.Graph, _startVox, _endVox);
            _grid.ColorVoxels(shortestPath, _highlightMat);
        }


    }

    IEnumerator RunGameOfLife()
    {
        while (true)
        {
            if (_grid.CurrentLayer == _gridDimension.y - 1)
            {
                _running = false;
                _grid.CreateGraph();
                StopCoroutine(_runGameOfLife);
            }

            _grid.UpdateGrid();



            yield return new WaitForSeconds(_speed);
        }
    }
}
