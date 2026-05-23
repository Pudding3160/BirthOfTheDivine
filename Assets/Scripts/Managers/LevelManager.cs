using Events;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] 
        private float levelTimerSeconds;
        [SerializeField]
        private float timerBuffer;
        private bool _finishedLevel;

        private void Start()
        {
            // GameEventManager.Instance.levelEvents.LevelTimerFinished += () => Debug.Log("Level Timer Finished");
            
            
            timerBuffer = levelTimerSeconds;
        }

        private void Update()
        {
            if (timerBuffer > 0)
            {
                timerBuffer -= Time.deltaTime;
                return;
            }
            if (_finishedLevel) return; // This exists so the bellow invocation only occurs once
            GameEventManager.Instance.levelEvents.OnLevelTimerFinished();
            _finishedLevel = true;
        }
        
        
    }
}
