using MoreMountains.Feedbacks;
using UnityEngine;

public class FlockFeedbackManager : MonoBehaviour
{
    public static FlockFeedbackManager Instance { get; private set; }

    [Header("Flock Feedbacks")]
    [SerializeField] private MMF_Player flockDeathFeedback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    //public void FlockDeath(Vector3 worldPosition)
    //{
    //    flockDeathFeedback.transform.position = worldPosition;

    //    flockDeathFeedback.PlayFeedbacks();
    //}

    // References
    public MMF_Player GetFlockDeathFeedback() => flockDeathFeedback;
}
