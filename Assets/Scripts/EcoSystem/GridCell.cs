using System.Diagnostics;

public class GridCell
{
    public bool HasGrass { get; private set; } = false;

    private float growthChance = 0.1f; // 5% chance per tick

    private CellView view;

    public void SetView(CellView cellView)
    {
        view = cellView;
    }

    public void Tick()
    {
        if (!HasGrass && UnityEngine.Random.value < 0.05f)
        {
            HasGrass = true;
            view?.SetGrassVisible(true);
        }
    }

    public void RemoveGrass()
    {
        HasGrass = false;
        view?.SetGrassVisible(false); // Hide the visual
    }

}
