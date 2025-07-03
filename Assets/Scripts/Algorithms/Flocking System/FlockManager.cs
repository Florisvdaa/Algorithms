using UnityEngine;
using System.Collections.Generic;

public class FlockManager : MonoBehaviour
{
    public FlockAgent agentPrefab;
    public int flockSize = 50;
    public float spawnRadius = 10f;

    [Range(1f, 100f)] public float driveFactor = 10f;
    [Range(1f, 10f)] public float neighborRadius = 3f;
    [Range(0.1f, 5f)] public float avoidanceRadius = 1f;

    public FlockBehavior behavior;

    [SerializeField] private Material agentMaterial; // Shared material for all agents

    private List<FlockAgent> agents = new List<FlockAgent>();

    // Accessors for shared values
    public float DriveFactor => driveFactor;
    public float NeighborRadius => neighborRadius;
    public float AvoidanceRadius => avoidanceRadius;
    public FlockBehavior Behavior => behavior;
    public List<FlockAgent> Agents => agents;

    void Start()
    {
        // Spawn and initialize all agents
        for (int i = 0; i < flockSize; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset2D.x, offset2D.y, 0f);

            FlockAgent agent = Instantiate(agentPrefab, spawnPos, Quaternion.identity, transform);
            agent.name = $"Agent {i}";
            agent.Initialize(this);
            agent.ApplyMaterial(agentMaterial);

            agents.Add(agent);
        }
    }

    public void RespawnAgent(FlockAgent deadAgent)
    {
        agents.Remove(deadAgent);

        Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(offset2D.x, offset2D.y, 0f);

        FlockAgent agent = Instantiate(agentPrefab, spawnPos, Quaternion.identity, transform);
        agent.name = $"Respawned Agent {Random.Range(1000, 9999)}";
        agent.Initialize(this);
        agent.ApplyMaterial(agentMaterial);

        agents.Add(agent);
    }

    // Optional bounds gizmo for debugging
    void OnDrawGizmosSelected()
    {
        if (behavior == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, behavior.boundsRadius);
    }
}
