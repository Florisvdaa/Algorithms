using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    // Boid movement and interaction settings
    [Header("Boid Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float neighborRadius = 3f;
    [SerializeField] private float separationRadius = 1f;
    [SerializeField] private float separationWeight = 2.5f;

    // Playable bounds and edge behavior
    [Header("World Bounds")]
    [SerializeField] private float xMin = -20f, xMax = 20f;
    [SerializeField] private float yMin = -10f, yMax = 10f;

    [Header("Edge Steering")]
    [SerializeField] private float edgeBuffer = 3f;
    [SerializeField] private float boundarySteerStrength = 1f;

    // Obstacle avoidance on collision or proximity
    [Header("Obstacle Avoidance")]
    [SerializeField] private float obstacleAvoidanceSteerStrength = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    // Forward-looking obstacle detection
    [Header("Proactive Avoidance")]
    [SerializeField] private float lookaheadDistance = 3f;
    [SerializeField] private float lookaheadRadius = 0.5f;

    private Vector3 velocity;

    private void Start()
    {
        // Initialize velocity in "up" direction
        velocity = transform.up * speed;

        // Setup physics components
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = separationRadius;
    }

    private void Update()
    {
        // Flocking forces
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int count = 0;

        // Find neighbors and compute steering components
        foreach (var other in BoidManager.Instance.GetBoids())
        {
            if (other == this) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance < neighborRadius)
            {
                alignment += other.Velocity;
                cohesion += other.transform.position;

                if (distance < separationRadius)
                {
                    // Push away, biased slightly toward a side tangent for smoother gliding
                    Vector3 away = transform.position - other.transform.position;
                    if (away != Vector3.zero)
                    {
                        Vector3 tangent = Vector3.Cross(Vector3.forward, away).normalized;
                        float angleBias = 0.4f;
                        separation += (1 - angleBias) * away.normalized / away.magnitude + angleBias * tangent;
                    }
                }

                count++;
            }
        }

        // Normalize forces if neighbors found
        if (count > 0)
        {
            alignment = (alignment / count).normalized * speed;
            cohesion = ((cohesion / count) - transform.position).normalized * speed;
            separation = separation.normalized * speed;
        }

        // Combine steering vectors into overall motion
        Vector3 force = alignment + cohesion + separation * separationWeight;
        velocity += force * Time.deltaTime;

        // Proactive obstacle avoidance with a forward SphereCast
        Ray ray = new Ray(transform.position, velocity.normalized);
        if (Physics.SphereCast(ray, lookaheadRadius, out RaycastHit hit, lookaheadDistance, obstacleLayer))
        {
            Vector3 avoidDir = Vector3.Reflect(velocity.normalized, hit.normal);
            velocity += avoidDir * obstacleAvoidanceSteerStrength * Time.deltaTime;
        }

        // World boundary avoidance
        Vector3 steer = Vector3.zero;
        Vector3 pos = transform.position;

        if (pos.x > xMax - edgeBuffer) steer += Vector3.left;
        if (pos.x < xMin + edgeBuffer) steer += Vector3.right;
        if (pos.y > yMax - edgeBuffer) steer += Vector3.down;
        if (pos.y < yMin + edgeBuffer) steer += Vector3.up;

        if (steer != Vector3.zero)
            velocity += steer.normalized * boundarySteerStrength * Time.deltaTime;

        // Clamp velocity, lock Z, and apply movement
        velocity.z = 0f;
        velocity = Vector3.ClampMagnitude(velocity, speed);
        pos += velocity * Time.deltaTime;
        pos.z = 0f;
        transform.position = pos;

        // Smoothly rotate to match velocity direction
        if (velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // Reactive avoidance upon entering trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            Vector3 away = transform.position - other.ClosestPoint(transform.position);
            if (away != Vector3.zero)
            {
                velocity += away.normalized * obstacleAvoidanceSteerStrength;
                velocity = Vector3.ClampMagnitude(velocity, speed);
                velocity.z = 0f;
            }
        }
    }

    // Provide velocity to other boids
    public Vector3 Velocity => velocity;

    // Editor visualization of influence zones and lookahead ray
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector3 origin = transform.position;

        Gizmos.color = new Color(1f, 0.5f, 1f); // velocity
        Gizmos.DrawLine(origin, origin + velocity);

        Gizmos.color = Color.green; // neighbor detection
        Gizmos.DrawWireSphere(origin, neighborRadius);

        Gizmos.color = Color.yellow; // separation zone
        Gizmos.DrawWireSphere(origin, separationRadius);

        Gizmos.color = Color.cyan; // edge buffer box
        Vector3 min = new Vector3(xMin + edgeBuffer, yMin + edgeBuffer, 0f);
        Vector3 max = new Vector3(xMax - edgeBuffer, yMax - edgeBuffer, 0f);
        Vector3 center = (min + max) / 2f;
        Vector3 size = new Vector3(xMax - xMin - edgeBuffer * 2, yMax - yMin - edgeBuffer * 2, 0f);
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.red; // lookahead cast
        Vector3 lookahead = velocity.normalized * lookaheadDistance;
        Gizmos.DrawRay(origin, lookahead);
        Gizmos.DrawWireSphere(origin + lookahead, lookaheadRadius);
    }
}