using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttonText;
    [SerializeField] private Loader.Scene targetAlgorithm;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        button.onClick.AddListener(ChangeToTargetScene);

        string original = targetAlgorithm.ToString();
        if (original.Contains("Scene"))
            original = original.Replace("Scene", "");

        buttonText.text = original.ToString();
    }

    private void ChangeToTargetScene()
    {
        if (targetAlgorithm == Loader.Scene.MainMenuScene)
            UIManager.Instance.UnsetPause();

        GameManager.Instance.ChangeAlgorithmState(MapSceneToAlgorithm(targetAlgorithm));

        Loader.Load(targetAlgorithm);
    }

    private Algorithms MapSceneToAlgorithm(Loader.Scene scene)
    {
        switch (scene)
        {
            case Loader.Scene.GameOfLifeScene: return Algorithms.GameOfLife;
            case Loader.Scene.FlockingSystemScene: return Algorithms.FlockingSystem;
            case Loader.Scene.LStyleScene: return Algorithms.LSystem;
            case Loader.Scene.AStarScene: return Algorithms.Astar;
            case Loader.Scene.EcoSystemScene: return Algorithms.EcoSystem;
            case Loader.Scene.MainMenuScene:
            case Loader.Scene.LoadingScene:
            default: return Algorithms.MainMenu;
        }
    }
}
