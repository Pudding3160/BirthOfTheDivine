    using UnityEngine;

namespace Events
{
    public class GameEventManager : MonoBehaviour
    {
        // Singleton
        public static GameEventManager Instance { get; private set; }

        public InputEvents inputEvents;
        public UIEvents uiEvents;
        public SceneEvents sceneEvents;
        public ResourceEvents resourceEvents;
        public LevelEvents levelEvents;
        public PlayerStatEvents playerStatEvents;
        public MiscEvents miscEvents;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple instances of GameEventManager detected!");
                Destroy(gameObject);
            }
            Instance = this;
            
            
            // Initialize events
            inputEvents = new InputEvents();
            uiEvents = new UIEvents();
            sceneEvents = new SceneEvents();
            resourceEvents = new ResourceEvents();
            levelEvents = new LevelEvents();
            playerStatEvents = new PlayerStatEvents();
            miscEvents = new MiscEvents();
        }
    }
}
