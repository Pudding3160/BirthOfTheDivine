using Events;
using UnityEngine;

public class EnablePlayerControls : MonoBehaviour
{
    [SerializeField] private int levelTimeSeconds;
    
    private void Start()
    {
        GameEventManager.Instance.sceneEvents.OnSceneLoaded();
        GameEventManager.Instance.levelEvents.OnLevelTimerStarted(levelTimeSeconds);
    }
}
