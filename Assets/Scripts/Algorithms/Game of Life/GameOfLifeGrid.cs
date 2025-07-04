using UnityEngine;

public class GameOfLifeGrid : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int sizeX = 20;
    [SerializeField] private int sizeY = 20;
    [SerializeField] private float spacing = 1.1f;

    public int SizeX => sizeX;
    public int SizeY => sizeY;

    public Cell[,] CreateGrid()
    {
        var grid = new Cell[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3 pos = new Vector3(x, y, 0f) * spacing;
                GameObject cellInstance = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                var white = cellInstance.transform.Find("WhiteVisual").gameObject;
                var black = cellInstance.transform.Find("BlackVisual").gameObject;

                bool alive = Random.value > 0.85f;

                var cell = new Cell(alive, white, black);
                grid[x, y] = cell;
                cell.UpdateVisual();
            }
        }

        return grid;
    }
}
