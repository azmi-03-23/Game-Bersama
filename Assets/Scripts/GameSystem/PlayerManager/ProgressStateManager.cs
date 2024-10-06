using UnityEngine;

public class ProgressStateManager : MonoBehaviour
{
    void Awake()
    {
        PlayerDataManager.Init();
        PlayerDataManager.LoadHighestGameAchievedInit();
    }
}
