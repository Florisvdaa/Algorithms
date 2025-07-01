using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    private FlockManager manager;
    private Collider agentCollider;
    private Vector3 velocity;

    [Header("Obstacle Avoidance")]
    public float lookaheadDistance = 3f;
    public float lookaheadRadius = 0.5f;
    public LayerMask obstacleLayer;
    public float obstacleAvoidanceStrength = 5f;

    [Header("Flock Team Settings")]
    [HideInInspector] public int teamID = -1; // Don't assign manually

    public Collider Collider => agentCollider;

    void Awake()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Initialize(FlockManager manager)
    {
        this.manager = manager;
        velocity = transform.up;
    }
    public void SetTeam(int newTeamID, Material mat)
    {
        teamID = newTeamID;
        GetComponentInChildren<Renderer>().material = mat;
    }

    void Update()
    {
        HandleTeamConversion(); // <-- moved from LateUpdate()

        List<Transform> context = GetNearbyObjects();
        Vector3 move = manager.Behavior.CalculateMove(this, context, manager);

        move = Vector3.ClampMagnitude(move, 1f);
        velocity += move * manager.DriveFactor * Time.deltaTime;

        // Circular bounds
        Vector3 center = manager.transform.position;
        float distanceToCenter = Vector3.Distance(transform.position, center);
        if (distanceToCenter > manager.Behavior.boundsRadius)
        {
            Vector3 toCenter = (center - transform.position).normalized;
            velocity += toCenter * manager.Behavior.edgeSteerWeight * Time.deltaTime;
        }

        // Obstacle avoidance
        Ray ray = new Ray(transform.position, velocity.normalized);
        if (Physics.SphereCast(ray, lookaheadRadius, out RaycastHit hit, lookaheadDistance, obstacleLayer))
        {
            Vector3 avoidDir = Vector3.Cross(hit.normal, Vector3.forward).normalized;
            float proximityFactor = 1f - (hit.distance / lookaheadDistance);
            velocity += avoidDir * obstacleAvoidanceStrength * (1f + proximityFactor) * Time.deltaTime;
        }

        // Final move
        velocity = Vector3.ClampMagnitude(velocity, 5f);
        velocity.z = 0f;
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    void HandleTeamConversion()
    {
        int[] teamCounts = new int[2];

        foreach (FlockAgent other in manager.Agents)
        {
            if (other == this) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < manager.NeighborRadius)
            {
                teamCounts[other.teamID]++;
            }
        }

        int myTeam = teamID;
        int otherTeam = 1 - myTeam;

        bool outnumbered = teamCounts[otherTeam] > teamCounts[myTeam] + 2;
        bool conversionThreshold = teamCounts[otherTeam] >= 3;

        if (outnumbered && conversionThreshold)
        {
            ConvertTo(otherTeam);
            velocity = Vector3.zero;
        }
    }

    void ConvertTo(int newTeamID)
    {
        if (newTeamID == teamID) return;

        teamID = newTeamID;
        Material mat = manager.GetTeamMaterial(teamID);
        GetComponentInChildren<Renderer>().material = mat;
        velocity = Vector3.zero;
    }

    List<Transform> GetNearbyObjects()
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(transform.position, manager.NeighborRadius);
        foreach (Collider c in contextColliders)
        {
            if (c == agentCollider) continue;

            FlockAgent other = c.GetComponent<FlockAgent>();
            if (other != null && other.teamID == this.teamID)
            {
                context.Add(other.transform);
            }
        }
        return context;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            // Notify manager to respawn
            manager.RespawnAgent(this);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 lookAhead = velocity.normalized * lookaheadDistance;
        Gizmos.DrawRay(transform.position, lookAhead);
        Gizmos.DrawWireSphere(transform.position + lookAhead, lookaheadRadius);
        Gizmos.color = teamID == 0 ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, manager.NeighborRadius);
    }
}
