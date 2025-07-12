using UnityEngine;

[System.Serializable]
public class AgentStats
{
    public float energy = 10f;
    public float moveCost = 1f;
    public float eatAmount = 5f;
    public float maxEnergy = 15f;

    public void AddEnergy(float amount)
    {
        energy = Mathf.Min(energy + amount, maxEnergy);
    }

    public void LoseEnergy(float amount)
    {
        energy -= amount;
    }

    public bool IsDead => energy <= 0f;
}
