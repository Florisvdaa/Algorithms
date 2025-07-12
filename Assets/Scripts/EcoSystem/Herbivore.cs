using UnityEngine;

public class Herbivore : AgentBase
{
    public override void Initialize(Vector2Int startPos, EcosystemManager eco)
    {
        base.Initialize(startPos, eco);
        Stats = new AgentStats(); // Use defaults for now
    }

    protected override void Act()
    {
        Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        Vector2Int? grassTarget = null;

        // Check surroundings for grass
        foreach (var dir in directions)
        {
            Vector2Int checkPos = GridPosition + dir;
            if (!ecosystem.IsInBounds(checkPos))
                continue;

            var checkCell = ecosystem.GetCell(checkPos);
            if (checkCell.HasGrass)
            {
                grassTarget = checkPos;
                break;
            }
        }

        // Move to grass if found
        if (grassTarget.HasValue)
        {
            MoveTo(grassTarget.Value);
            var targetCell = ecosystem.GetCell(grassTarget.Value);
            targetCell.RemoveGrass();
            Stats.AddEnergy(Stats.eatAmount);
        }
        else
        {
            // No grass nearby, move randomly
            Vector2Int targetPos = GridPosition + directions[Random.Range(0, directions.Length)];

            if (!ecosystem.IsInBounds(targetPos))
                return;

            MoveTo(targetPos);
        }
    }


    private void MoveTo(Vector2Int newPos)
    {
        GridPosition = newPos;
        transform.position = new Vector3(newPos.x, 0f, newPos.y); //  use Z
    }
}
