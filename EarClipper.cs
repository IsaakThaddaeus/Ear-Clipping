using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class EarClipper
{
    public static List<int> triangulate(List<Vector2> points)
    {
        List<Vector2> vertices = new List<Vector2>(points);
        List<int> triangles = new List<int>();


        while (vertices.Count > 3)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 x = vertices[mod(i - 1, vertices.Count)];
                Vector2 a = vertices[mod(i, vertices.Count)];
                Vector2 y = vertices[mod(i + 1, vertices.Count)];

                if (cross(x - a, y - a) > 0 && !pointInEar(x, a, y, vertices))
                {
                    triangles.Add(points.IndexOf(x));
                    triangles.Add(points.IndexOf(a));
                    triangles.Add(points.IndexOf(y));
                    vertices.RemoveAt(i);
                    break;
                }
            }
        }

        triangles.Add(points.IndexOf(vertices[0]));
        triangles.Add(points.IndexOf(vertices[1]));
        triangles.Add(points.IndexOf(vertices[2]));

        return triangles;
    }

    static bool pointInEar(Vector2 x, Vector2 a, Vector2 y, List<Vector2> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (i == vertices.IndexOf(x) || i == vertices.IndexOf(a) || i == vertices.IndexOf(y))
                continue;
               
            if (pointInTriangle(vertices[i], x, a, y))
                return true;
        }

        return false;
    }
    static bool pointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c){

        float cross1 = cross(b - a, p - a);
        float cross2 = cross(c - b, p - b);
        float cross3 = cross(a - c, p - c);

        if (cross1 > 0f || cross2 > 0f || cross3 > 0f)
            return false;

        return true;
    }
    static float cross(Vector2 a, Vector2 b){
        return a.x * b.y - a.y * b.x;
    }
    static int mod(int x, int m)
    {
        return ((x % m) + m) % m;
    }
}
