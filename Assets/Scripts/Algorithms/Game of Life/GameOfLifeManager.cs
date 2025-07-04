using UnityEngine;

public class GameOfLifeManager : MonoBehaviour
{
    [SerializeField] private GameOfLifeGrid visualizer;
    [SerializeField, Range(0.01f, 2f)] private float tickRate = 0.5f;
    [SerializeField] private Camera mainCamera;

    private float timer;
    private bool isRunning = false;

    public void SetRunning(bool state)
    {
        isRunning = state;

        if (isRunning)
        {
            RandomizeCells(); // add this line to populate randomly
        }
    }
    public void SetTickRate(float rate) => tickRate = rate;
    public void Step() => Simulate();

    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            if (timer >= tickRate)
            {
                timer = 0f;
                Simulate();
            }
        }
    }

    private void Simulate()
    {
        int xSize = visualizer.SizeX;
        int ySize = visualizer.SizeY;
        var grid = visualizer.Grid;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(grid, x, y);
                var cell = grid[x, y];

                cell.NextState = cell.IsAlive
                    ? (aliveNeighbors == 2 || aliveNeighbors == 3)
                    : (aliveNeighbors == 3);
            }
        }

        foreach (var cell in grid)
        {
            cell.IsAlive = cell.NextState;
            cell.UpdateVisual();
        }
    }

    private int CountAliveNeighbors(Cell[,] grid, int cx, int cy)
    {
        int count = 0;
        int w = visualizer.SizeX;
        int h = visualizer.SizeY;

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int x = cx + dx;
                int y = cy + dy;

                if (x >= 0 && x < w && y >= 0 && y < h && grid[x, y].IsAlive)
                    count++;
            }

        return count;
    }
    public void RandomizeCells(float aliveChance = 0.2f)
    {
        var grid = visualizer.Grid;
        int width = visualizer.SizeX;
        int height = visualizer.SizeY;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool willBeAlive = Random.value < aliveChance;
                var cell = grid[x, y];
                cell.IsAlive = willBeAlive;
                cell.UpdateVisual();
            }
        }
    }

}
