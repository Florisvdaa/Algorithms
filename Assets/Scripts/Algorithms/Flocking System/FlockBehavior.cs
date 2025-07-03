using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Flock/Behavior")]
public class FlockBehavior : ScriptableObject
{
    [Header("Flocking Weights")]
    public float alignmentWeight = 0.5f;
    public float cohesionWeight = 0.5f;
    public float avoidanceWeight = 4f;

    [Header("Bounds")]
    public float boundsRadius = 30f;
    public float edgeSteerWeight = 10f;

    public Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager manager)
    {
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 avoidance = Vector3.zero;

        if (context.Count == 0)
            return Vector3.zero;

        int count = 0;
        foreach (Transform item in context)
        {
            Vector3 toNeighbor = item.position - agent.transform.position;
            toNeighbor.z = 0f;

            alignment += item.forward;
            cohesion += item.position;

            if (toNeighbor.magnitude < manager.AvoidanceRadius)
            {
                avoidance -= toNeighbor.normalized / toNeighbor.magnitude;
            }

            count++;
        }

        if (count > 0)
        {
            alignment /= count;
            cohesion = (cohesion / count - agent.transform.position);
            avoidance /= count;
        }

        alignment.z = cohesion.z = avoidance.z = 0f;

        Vector3 move = alignment * alignmentWeight +
                       cohesion * cohesionWeight +
                       avoidance * avoidanceWeight;

        return Vector3.ClampMagnitude(move, 1f);
    }
}
