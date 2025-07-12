using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeGrid : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private ObjectPool cellPool;
    [SerializeField] private float spacing = 1.1f;

    private int sizeX = 10;
    private int sizeY = 10;
    public int SizeX => sizeX;
    public int SizeY => sizeY;

    public Cell[,] Grid { get; private set; }

    private List<GameObject> activeCellObjects = new List<GameObject>();

    public void RebuildGrid(int x, int y)
    {
        sizeX = Mathf.Clamp(x, 1, 100);
        sizeY = Mathf.Clamp(y, 1, 100);

        // Return previous to pool
        foreach (GameObject obj in activeCellObjects)
        {
            cellPool.Return(obj);
        }
        activeCellObjects.Clear();

        Grid = new Cell[sizeX, sizeY];

        for (int xi = 0; xi < sizeX; xi++)
        {
            for (int yi = 0; yi < sizeY; yi++)
            {
                Vector3 pos = new Vector3(xi, yi, 0f) * spacing;

                GameObject cellObj = cellPool.Get(pos, transform);

                var white = cellObj.transform.Find("WhiteVisual")?.gameObject;
                var black = cellObj.transform.Find("BlackVisual")?.gameObject;
                var colliderObj = cellObj.transform.Find("Collider")?.GetComponent<Collider>();

                if (white == null || black == null || colliderObj == null)
                {
                    Debug.LogWarning($"Cell prefab is missing required parts at [{xi}, {yi}]!");
                }

                Cell cell = new Cell(false, white, black, colliderObj);
                Grid[xi, yi] = cell;
                cell.UpdateVisual();

                activeCellObjects.Add(cellObj);
            }
        }
    }

    public Vector2Int GetMaxGridSizeFromCamera(Camera cam)
    {
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));

        float width = Mathf.Abs(topRight.x - bottomLeft.x);
        float height = Mathf.Abs(topRight.y - bottomLeft.y);

        int cellsX = Mathf.FloorToInt(width / spacing);
        int cellsY = Mathf.FloorToInt(height / spacing);

        return new Vector2Int(Mathf.Min(cellsX, 100), Mathf.Min(cellsY, 100));
    }
}
