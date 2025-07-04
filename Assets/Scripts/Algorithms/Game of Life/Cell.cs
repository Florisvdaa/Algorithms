using UnityEngine;

public class Cell
{
    public bool IsAlive;
    public bool NextState;

    private GameObject _whiteVisual;
    private GameObject _blackVisual;

    public Cell(bool isAlive, GameObject whiteVisual, GameObject blackVisual)
    {
        IsAlive = isAlive;
        _whiteVisual = whiteVisual;
        _blackVisual = blackVisual;
    }

    public void Toggle()
    {
        IsAlive = !IsAlive;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        _whiteVisual.SetActive(IsAlive);
        _blackVisual.SetActive(!IsAlive);
    }
}
