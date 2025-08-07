using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject pauseCanvas;

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

    public void SetPause()
    {
        if(pauseCanvas != null) 
            pauseCanvas.SetActive(true);
    }
    public void UnsetPause()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
    }
}
