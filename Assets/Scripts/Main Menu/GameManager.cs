using UnityEngine;

public enum Algorithms {MainMenu ,GameOfLife, FlockingSystem, LSystem, Astar, EcoSystem }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Algorithms algorithmState = Algorithms.MainMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && algorithmState != Algorithms.MainMenu)
        {
            UIManager.Instance.SetPause();
        }
    }

    public void ChangeAlgorithmState(Algorithms newState)
    {
        algorithmState = newState;
    }
}
