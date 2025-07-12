using UnityEngine;

public class CellView : MonoBehaviour
{
    [SerializeField] private GameObject grassVisual;

    public void SetGrassVisible(bool hasGrass)
    {
        if (grassVisual != null)
            grassVisual.SetActive(hasGrass);
    }
}
