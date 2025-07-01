using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance { get; private set; }

    [SerializeField] private Boid boidPrefab;
    [SerializeField] private int boidCount = 100;
    [SerializeField] private float spawnDelay = 0.1f;
    [SerializeField] private Transform spawnPoint;

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

    private void Start()
    {
        StartCoroutine(SpawnBoidsSequentially());
    }

    private IEnumerator SpawnBoidsSequentially()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            Boid newBoid = Instantiate(boidPrefab, spawnPos, Quaternion.identity, transform);
            boids.Add(newBoid);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public List<Boid> GetBoids() => boids;
}