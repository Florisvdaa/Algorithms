using UnityEngine;

public class Cell
{
    public bool IsAlive { get; set; }      // Allow setting IsAlive
    public bool NextState { get; set; }

    private GameObject whiteVisual;
    private GameObject blackVisual;
    private Collider collider;

    public Cell(bool isAlive, GameObject white, GameObject black, Collider col)
    {
        IsAlive = isAlive;
        whiteVisual = white;
        blackVisual = black;
        collider = col;
    }

    public void SetAlive(bool alive)
    {
        IsAlive = alive;
        UpdateVisual();
    }

    public void Toggle()
    {
        IsAlive = !IsAlive;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (whiteVisual != null) whiteVisual.SetActive(IsAlive);
        if (blackVisual != null) blackVisual.SetActive(!IsAlive);
    }
}
