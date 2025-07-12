using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentBase : MonoBehaviour
{
    public Vector2Int GridPosition { get; protected set; }
    public AgentStats Stats { get; protected set; }

    protected EcosystemManager ecosystem;
    private Coroutine moveRoutine;

    protected void MoveTo(Vector2Int newPos)
    {
        if (ecosystem.IsOccupied(newPos))
            return; // Don't move if occupied

        ecosystem.UnregisterAgent(GridPosition); // clear old tile
        GridPosition = newPos;
        ecosystem.RegisterAgent(GridPosition, this); // register new tile

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(SmoothMove(new Vector3(newPos.x, 0f, newPos.y), 0.25f));
    }


    public virtual void Initialize(Vector2Int startPos, EcosystemManager eco)
    {
        GridPosition = startPos;
        ecosystem = eco;
        transform.position = new Vector3(startPos.x, 0f, startPos.y); //  now Z instead of Y
    }


    public virtual void Tick()
    {
        Stats.LoseEnergy(Stats.moveCost);
        if (Stats.IsDead)
        {
            Die();
            return;
        }

        Act();
    }
    private IEnumerator SmoothMove(Vector3 targetPosition, float duration)
    {
        Vector3 start = transform.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.position = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }


    protected abstract void Act();

    protected void Die()
    {
        Destroy(gameObject); // basic cleanup
    }

}
