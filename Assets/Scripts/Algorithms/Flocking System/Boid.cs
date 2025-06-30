using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Boid Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float neighborRadius = 3f;
    [SerializeField] private float separationRadius = 1f;
    [SerializeField] private float separationWeight = 2.5f;

    [Header("World Bounds")]
    [SerializeField] private float xMin = -20f, xMax = 20f;
    [SerializeField] private float yMin = -10f, yMax = 10f;

    [Header("Edge Steering")]
    [SerializeField] private float edgeBuffer = 3f;
    [SerializeField] private float steerStrength = 1f;

    private Vector3 velocity;

    private void Start()
    {
        velocity = transform.forward * speed;
    }

    private void Update()
    {
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (var other in BoidManager.Instance.GetBoids())
        {
            if (other == this) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance < neighborRadius)
            {
                alignment += other.velocity;
                cohesion += other.transform.position;

                if (distance < separationRadius)
                {
                    Vector3 away = transform.position - other.transform.position;
                    if (away != Vector3.zero)
                    {
                        separation += away.normalized / away.magnitude; // stronger push if closer
                    }
                }

                count++;
            }
        }

        if (count > 0)
        {
            alignment = (alignment / count).normalized * speed;
            cohesion = ((cohesion / count) - transform.position).normalized * speed;
            separation = separation.normalized * speed;
        }

        // Combine weighted forces
        Vector3 force = alignment + cohesion + separation * separationWeight;
        velocity += force * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, speed);

        // Soft edge steering
        Vector3 steer = Vector3.zero;
        Vector3 pos = transform.position;

        if (pos.x > xMax - edgeBuffer) steer += Vector3.left;
        if (pos.x < xMin + edgeBuffer) steer += Vector3.right;
        if (pos.y > yMax - edgeBuffer) steer += Vector3.down;
        if (pos.y < yMin + edgeBuffer) steer += Vector3.up;

        if (steer != Vector3.zero)
        {
            velocity += steer.normalized * steerStrength * Time.deltaTime;
        }

        // Apply movement
        pos += velocity * Time.deltaTime;
        pos.z = 0f;
        transform.position = pos;

        // Smooth facing
        if (velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public Vector3 Velocity => velocity;
}