using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavUtils 
{
    public static Vector3 GetRandomPoint()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int t = Random.Range(0, navMeshData.indices.Length - 3);

        var point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        point = Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;
    }

    // public static Vector3 GetRandomPointInRange(Vector3 center, float range)
    // {
    //     NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
    //     List<int> list = new List<int>();
    //     for (int i = 0; i < navMeshData.indices.Length - 3; i++)
    //     {
    //         list.Add(i);
    //     }
    //     List<Vector3> triangle = new List<Vector3>();

    //     for (int i = 0; i < list.Count; i++)
    //     {
    //         int index = Random.Range(0, list.Count);
    //         if (Vector3.Distance(center, ))
    //     }
    // }
}