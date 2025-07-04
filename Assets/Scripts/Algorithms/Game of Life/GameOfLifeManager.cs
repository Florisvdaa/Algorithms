using UnityEngine;

public class GameOfLifeManager : MonoBehaviour
{
    [SerializeField] private GameOfLifeGrid visualizer;
    [SerializeField] private float tickRate = 0.5f;

    private Cell[,] grid;
    private float timer;

    private void Start()
    {
        grid = visualizer.CreateGrid();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tickRate)
        {
            timer = 0f;
            StepSimulation();
        }
    }

    private void StepSimulation()
    {
        int sizeX = visualizer.SizeX;
        int sizeY = visualizer.SizeY;

        // Calculate next state
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                var cell = grid[x, y];

                // Conway's rules:
                if (cell.IsAlive)
                    cell.NextState = aliveNeighbors == 2 || aliveNeighbors == 3;
                else
                    cell.NextState = aliveNeighbors == 3;
            }
        }

        // Apply state
        foreach (var cell in grid)
        {
            cell.IsAlive = cell.NextState;
            cell.UpdateVisual();
        }
    }

    private int CountAliveNeighbors(int cx, int cy)
    {
        int count = 0;

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int x = cx + dx;
                int y = cy + dy;

                if (x >= 0 && x < visualizer.SizeX &&
                    y >= 0 && y < visualizer.SizeY &&
                    grid[x, y].IsAlive)
                {
                    count++;
                }
            }

        return count;
    }
}
