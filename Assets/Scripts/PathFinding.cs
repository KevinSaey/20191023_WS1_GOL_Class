using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using QuickGraph;
using QuickGraph.Algorithms;
using System.Linq;

public static class PathFinding
{
    public static List<Voxel> CalculateShortestPath(UndirectedGraph<Voxel, Edge<Voxel>> graph, Voxel start, Voxel end)
    {
        //Run the dijkstra shortest path algorithm
        TryFunc<Voxel, IEnumerable<Edge<Voxel>>> shortest = graph.ShortestPathsDijkstra(e => 1.0, start);
        shortest(end, out IEnumerable<Edge<Voxel>> endPath);

        List<Voxel> shortestPathVoxels = new List<Voxel>();

        //Extract ordered list of voxels in the shortest path
        foreach (Edge<Voxel> edge in endPath)
        {
            shortestPathVoxels.Add(edge.Source);
        }
        shortestPathVoxels.Add(endPath.Last().Target);

        return shortestPathVoxels;
    }

}
