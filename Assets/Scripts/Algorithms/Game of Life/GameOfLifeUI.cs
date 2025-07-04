using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOfLifeUI : MonoBehaviour
{
    [SerializeField] private GameOfLifeGrid gameOfLifeGrid;
    [SerializeField] private GameOfLifeManager gameOfLifeManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Slider xSlider;
    [SerializeField] private Slider ySlider;
    [SerializeField] private Slider tickSlider;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button runPauseButton;
    [SerializeField] private TextMeshProUGUI runButtonText;

    private bool isRunning = false;

    private void Start()
    {
        buildButton.onClick.AddListener(OnBuild);
        runPauseButton.onClick.AddListener(OnToggleRun);
        tickSlider.onValueChanged.AddListener(gameOfLifeManager.SetTickRate);
        OnBuild(); // build once at start
    }

    private void OnBuild()
    {
        gameOfLifeManager.SetRunning(false);
        isRunning = false;
        runButtonText.text = "Start";
        gameOfLifeGrid.RebuildGrid((int)xSlider.value, (int)ySlider.value);
    }

    private void OnToggleRun()
    {
        isRunning = !isRunning;
        gameOfLifeManager.SetRunning(isRunning);
        runButtonText.text = isRunning ? "Pause" : "Start";
    }
    public void FitGridToScreen()
    {
        Vector2Int size = gameOfLifeGrid.GetMaxGridSizeFromCamera(mainCamera);
        xSlider.value = size.x;
        ySlider.value = size.y;
    }
}
