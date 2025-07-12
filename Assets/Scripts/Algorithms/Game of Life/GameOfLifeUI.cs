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
    [SerializeField] private TextMeshProUGUI xSliderText;
    [SerializeField] private TextMeshProUGUI ySliderText;
    [SerializeField] private TextMeshProUGUI tickSliderText;

    private bool isRunning = false;

    private void Start()
    {
        buildButton.onClick.AddListener(OnBuild);
        runPauseButton.onClick.AddListener(OnToggleRun);
        tickSlider.onValueChanged.AddListener(gameOfLifeManager.SetTickRate);

        xSlider.onValueChanged.AddListener(OnDimensionChanged);
        ySlider.onValueChanged.AddListener(OnDimensionChanged);

        OnBuild(); // build once at start
    }

    private void Update()
    {
        xSliderText.text = $"X Axis: {(int)xSlider.value} ({FormatSliderPercentage(xSlider)})";
        ySliderText.text = $"Y Axis: {(int)ySlider.value} ({FormatSliderPercentage(ySlider)})";
        tickSliderText.text = $"Tick speed: {tickSlider.value:0.00}";
    }


    /// <summary>
    /// This function generates the Grid based on the slider current value.
    /// We don't need the float param, but it needs to be there because the slider value.
    /// </summary>
    /// <param name="_"></param>
    private void OnDimensionChanged(float _)
    {
        if (!isRunning)
        {
            gameOfLifeGrid.RebuildGrid((int)xSlider.value, (int)ySlider.value);
        }
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
    private string FormatSliderPercentage(Slider slider)
    {
        float percent = (slider.value / slider.maxValue) * 100f;
        return $"{Mathf.RoundToInt(percent)}%";
    }

}
