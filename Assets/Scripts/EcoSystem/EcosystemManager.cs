using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20);
    [SerializeField] private float tickRate = 1f;
    [SerializeField] private GameObject cellPrefab; // Optional: for visuals
    [SerializeField] private GameObject herbivorePrefab;
    
    private List<Herbivore> herbivores = new();
    private Dictionary<Vector2Int, AgentBase> occupiedTiles = new();


    private float timer;
    private GridCell[,] grid;

    private void Start()
    {
        InitGrid();

        int amount = 20;
        int tries = 0;

        while (amount > 0 && tries < 500)
        {
            Vector2Int randomPos = new Vector2Int(
                Random.Range(0, gridSize.x),
                Random.Range(0, gridSize.y)
            );

            if (SpawnHerbivore(randomPos))
                amount--;

            tries++;
        }

        Debug.Log("Herbivores spawned: " + (20 - amount));
    }



    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tickRate)
        {
            timer = 0f;
            Tick();
        }
    }

    private void InitGrid()
    {
        grid = new GridCell[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = new GridCell();

                Vector3 pos = new Vector3(x, 0f, y);
                GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity);
                cellObj.name = $"Cell ({x},{y})";

                var view = cellObj.GetComponent<CellView>();
                grid[x, y].SetView(view); // You’ll add this method below
            }
        }
    }

    private void Tick()
    {
        // Grid logic
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
                grid[x, y].Tick();

        // Agents
        for (int i = herbivores.Count - 1; i >= 0; i--)
        {
            if (herbivores[i] == null)
            {
                herbivores.RemoveAt(i);
                continue;
            }

            herbivores[i].Tick();
        }
    }

    public bool SpawnHerbivore(Vector2Int position)
    {
        if (occupiedTiles.ContainsKey(position))
            return false; // tile already taken

        GameObject obj = Instantiate(herbivorePrefab);
        Herbivore h = obj.GetComponent<Herbivore>();
        h.Initialize(position, this);

        herbivores.Add(h);
        occupiedTiles[position] = h;
        return true;
    }
    public void UnregisterAgent(Vector2Int position)
    {
        if (occupiedTiles.ContainsKey(position))
        {
            occupiedTiles.Remove(position);
        }
    }
    public void RegisterAgent(Vector2Int pos, AgentBase agent)
    {
        occupiedTiles[pos] = agent;
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return occupiedTiles.ContainsKey(pos);
    }


    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x &&
               pos.y >= 0 && pos.y < gridSize.y;
    }

    public GridCell GetCell(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }


}
