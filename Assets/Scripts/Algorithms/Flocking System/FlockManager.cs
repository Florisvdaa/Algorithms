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

    [SerializeField] private Material greenMat, orangeMat;

    private List<FlockAgent> agents = new List<FlockAgent>();

    public float DriveFactor => driveFactor;
    public float NeighborRadius => neighborRadius;
    public float AvoidanceRadius => avoidanceRadius;
    public FlockBehavior Behavior => behavior;
    public List<FlockAgent> Agents => agents;

    void Start()
    {
        for (int i = 0; i < flockSize; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset2D.x, offset2D.y, 0f);

            FlockAgent agent = Instantiate(agentPrefab, spawnPos, Quaternion.identity, transform);
            agent.name = $"Agent {i}";
            agent.Initialize(this);

            // Set correct team ID and material
            int teamID = i < flockSize / 2 ? 0 : 1;
            Material mat = GetTeamMaterial(teamID);
            agent.SetTeam(teamID, mat); // ← this overrides the prefab value

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
        agents.Add(agent);
    }

    public Material GetTeamMaterial(int id)
    {
        return id == 0 ? greenMat : orangeMat;
    }
    void OnDrawGizmosSelected()
    {
        if (behavior == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, behavior.boundsRadius);
    }
}
