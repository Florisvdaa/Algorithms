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

    public Collider Collider => agentCollider;

    void Awake()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Initialize(FlockManager manager)
    {
        this.manager = manager;
        velocity = transform.up; // Initial direction
    }

    // Apply material to the agent's visual
    public void ApplyMaterial(Material mat)
    {
        GetComponentInChildren<Renderer>().material = mat;
    }

    void Update()
    {
        // Get nearby agents for alignment, cohesion, avoidance
        List<Transform> context = GetNearbyObjects();

        // Compute move vector from behavior logic
        Vector3 move = manager.Behavior.CalculateMove(this, context, manager);
        move = Vector3.ClampMagnitude(move, 1f); // Limit influence strength
        velocity += move * manager.DriveFactor * Time.deltaTime;

        // Stay within circular bounds
        Vector3 center = manager.transform.position;
        float distanceToCenter = Vector3.Distance(transform.position, center);
        if (distanceToCenter > manager.Behavior.boundsRadius)
        {
            Vector3 toCenter = (center - transform.position).normalized;
            velocity += toCenter * manager.Behavior.edgeSteerWeight * Time.deltaTime;
        }

        // Obstacle avoidance using a forward sphere cast
        Ray ray = new Ray(transform.position, velocity.normalized);
        if (Physics.SphereCast(ray, lookaheadRadius, out RaycastHit hit, lookaheadDistance, obstacleLayer))
        {
            Vector3 avoidDir = Vector3.Cross(hit.normal, Vector3.forward).normalized;
            float proximityFactor = 1f - (hit.distance / lookaheadDistance);
            velocity += avoidDir * obstacleAvoidanceStrength * (1f + proximityFactor) * Time.deltaTime;
        }

        // Apply movement
        velocity = Vector3.ClampMagnitude(velocity, 5f);
        velocity.z = 0f;
        transform.position += velocity * Time.deltaTime;

        // Smoothly rotate toward velocity
        if (velocity != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    private List<Transform> GetNearbyObjects()
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(transform.position, manager.NeighborRadius);
        foreach (Collider c in contextColliders)
        {
            if (c != agentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    // If agent hits an obstacle, destroy and respawn
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            manager.RespawnAgent(this);
            Destroy(gameObject);
            //FlockFeedbackManager.Instance.FlockDeath(transform.position);
        }
    }

    // Visualize direction and avoidance radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 lookAhead = velocity.normalized * lookaheadDistance;
        Gizmos.DrawRay(transform.position, lookAhead);
        Gizmos.DrawWireSphere(transform.position + lookAhead, lookaheadRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, manager.NeighborRadius);
    }
}
