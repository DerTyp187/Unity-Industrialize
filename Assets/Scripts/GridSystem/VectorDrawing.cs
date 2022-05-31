using System.Collections.Generic;
using UnityEngine;

public class VectorDrawing : MonoBehaviour
{
    public static float DiagonalDistance(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        float dx = endWorldPosition.x - startWorldPosition.x;
        float dy = endWorldPosition.y - startWorldPosition.y;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static List<Vector2Int> FindVectorPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        List<Vector2Int> points = new List<Vector2Int>();
        Grid<PathNode> grid = Pathfinding.instance.GetGrid();
        float N = DiagonalDistance(startWorldPosition, endWorldPosition);

        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        points.Add(new Vector2Int(startX, startY));

        for (int i = 0; i < N + 1; i++) // N+1  for endWorldPosition
        {
            int x;
            int y;

            if (i < N)
            {
                float t = (float)i / N;
                Vector3 point = Vector3.Lerp(startWorldPosition, endWorldPosition, t);

                grid.GetXY(point, out x, out y);
            }
            else // is endWorldPosition
            {
                x = endX;
                y = endY;
            }



            // Sample points -> no diagonal connection
            if (i != 0)
            {
                if (x != points[points.Count - 1].x && y != points[points.Count - 1].y)
                {
                    points.Add(new Vector2Int(x, points[points.Count - 1].y));
                }
            }
            points.Add(new Vector2Int(x, y));
        }

        return points;
    }
}
