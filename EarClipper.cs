using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public static List<int> triangulateWithHoles(List<Vector2> points, List<Vector2> hole)
    {
        List<Vector2> newPoints = new List<Vector2>(points);

        hole.Reverse();
        getMutuallyVisiblePoints(newPoints, hole, out int n, out int m);
        cutOpenPolygon(newPoints, hole, n, m);

        Debug.Log("n: " + n + " m: " + m);

        for (int i = 0; i < newPoints.Count; i ++)
        {
            Debug.Log(i + " : " + newPoints[i]);
        }

        //triangulate(newPoints);

        return null;
        
        

    }

    static void cutOpenPolygon(List<Vector2> points, List<Vector2> hole, int n, int m)
    {
        List<Vector2> insertionPoints = new List<Vector2>();

        for(int i = 0; i < hole.Count; i++)
        {
            insertionPoints.Add(hole[(m + i) % hole.Count]);
        }
        insertionPoints.Add(hole[m]);
        insertionPoints.Add(points[n]);

        points.InsertRange(n + 1, insertionPoints);
    }

    //Identify Mutually Visible Points
    static void getMutuallyVisiblePoints(List<Vector2> points, List<Vector2> hole, out int n, out int m)
    {
        n = -1;
        m = hole.IndexOf(hole.OrderByDescending(p => p.x).First());
        
        getClosestPoint(hole[m], points, out Vector2 i, out int p, out int mv);
        if(mv != -1){
            n = mv;
            return;
        }

        List<int> reflexes = getReflexVerticesInsideMIP(hole[m], i, p, points);
        if (reflexes.Count == 0){
            n = p;
            return;
        }

        n = getMinimumAngle(hole[m], i, reflexes, points);
    }
    static void getClosestPoint(Vector2 m, List<Vector2> points, out Vector2 closest, out int p, out int mv)
    {
        closest = new Vector2(Mathf.Infinity, 0);
        p = -1;
        mv = -1;

        for(int i = 0; i < points.Count; i++)
        {
            Vector2 v0 = points[i];
            Vector2 v1 = points[(i + 1) % points.Count];

            if (m.x > v0.x && m.x > v1.x)
                continue;

            if (m.y > v0.y || m.y < v1.y)
                continue;

            
            if(m.y == v0.y){
                mv = i;
                return;
            }

            if(m.y == v1.y){
                mv = (i + 1) % points.Count;
                return;
            }

            if (LineIntersector.intersectionPoint(m, m + new Vector2(100000, 0), v0, v1, out Vector2 intersection))
            {
                if (closest.x > intersection.x)
                {
                    closest = intersection;
                    p = (v0.x >= v1.x) ? i : (i + 1) % points.Count;
                }
            }
        }
    }
    static List<int> getReflexVerticesInsideMIP(Vector2 a, Vector2 b, int cI, List<Vector2> points)
    {
        List<int> reflexes = new List<int>();

        for(int i = 0; i < points.Count; i++)
        {
            if (i == cI)
                continue;

            Vector2 beforeVertex = points[mod(i - 1, points.Count)];
            Vector2 thisVertex = points[mod(i, points.Count)];
            Vector2 nextVertex = points[mod(i + 1, points.Count)];

            if(cross(beforeVertex - thisVertex, nextVertex - thisVertex) < 0 && pointInTriangle(points[i], a, b, points[cI]))
                reflexes.Add(i);
        }

        return reflexes;
    }
    static int getMinimumAngle(Vector2 M, Vector2 I, List<int> reflexes, List<Vector2> points)
    {
        return reflexes.OrderBy(i => Vector2.Angle(I - M, points[i] - M)).First();
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
