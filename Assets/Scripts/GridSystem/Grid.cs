using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject>
{
    int width, height;
    float cellSize;
    Vector3 originPosition;
    public TGridObject[,] gridArray;

    bool showDebug = true;

    public int GetWidth() => width;
    public int GetHeight() => height;
    public float GetCellSize() => cellSize;

    public Grid(int _width, int _height, float _cellSize, Vector3 _originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;
        originPosition = _originPosition;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 1000f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 1000f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 1000f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 1000f);
        }

    }

    public List<TGridObject> GetGridObjectsAround(Vector3 position)
    {
        List<TGridObject> gridObjects = new List<TGridObject>();

        GetXY(position, out int x, out int y);

        if (x - 1 >= 0)
        {
            gridObjects.Add(gridArray[x - 1, y]);
        }
        if (x + 1 < width)
        {
            gridObjects.Add(gridArray[x + 1, y]);
        }
        if (y - 1 >= 0)
        {
            gridObjects.Add(gridArray[x, y - 1]);
        }
        if (y + 1 < height)
        {
            gridObjects.Add(gridArray[x, y + 1]);
        }

        return gridObjects;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            Debug.Log(x.ToString() + " " + y.ToString() + " -> " + value.ToString());
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }
}