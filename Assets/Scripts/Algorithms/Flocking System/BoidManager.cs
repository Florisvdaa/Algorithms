using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance { get; private set; }

    [SerializeField] private Boid boidPrefab;
    [SerializeField] private int boidCount = 100;
    [SerializeField] private List<Boid> boids = new List<Boid>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPos = Random.insideUnitSphere * 10f;
            Boid newBoid = Instantiate(boidPrefab, spawnPos, Quaternion.identity);
            boids.Add(newBoid);
        }
    }
    public List<Boid> GetBoids() => boids;
}
