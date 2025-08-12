using UnityEngine;
using UnityEngine.UI;

public class FlockUIManager : MonoBehaviour
{
    [SerializeField] private FlockManager flockManager;

    // Buttons
    [SerializeField] private Button startSystemButton;
    [SerializeField] private Slider flockAgentsSlider;

    // Canvas
    [SerializeField] private GameObject flockingCanvas;


    private void Start()
    {
        startSystemButton.onClick.AddListener(() => StartSystem());

        if(flockingCanvas != null )
            flockingCanvas.SetActive(true);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(flockingCanvas != null)
                flockingCanvas.SetActive(true);
        }
    }

    private void StartSystem()
    {
        if(flockingCanvas != null)
            flockingCanvas.SetActive(false);

        flockManager.SpawnAgents((int)flockAgentsSlider.value);
    }
}
