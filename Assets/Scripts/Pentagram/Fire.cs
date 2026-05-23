using System;
using Events;
using UnityEngine;

namespace Pentagram
{
    public class Fire : MonoBehaviour
    {
        private Collider2D detectionArea;
        [SerializeField]
        private Canvas upgradeCanvas;
        [SerializeField]
        private bool hasOfferedBlood = false;
        
        private void Awake()
        {
            detectionArea = GetComponent<Collider2D>();
        }

        private void Start()
        {
            GameEventManager.Instance.resourceEvents.UnlockNextLevel += AllowUpgrades;
        }

        private void AllowUpgrades()
        {
            hasOfferedBlood = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!hasOfferedBlood) return;
            if (!other.gameObject.CompareTag("Player")) return;
            upgradeCanvas.gameObject.SetActive(true);
        }
        
        private void OnTriggerExit2D(Collider2D other) 
        {
            if (!hasOfferedBlood) return;
            if (!other.gameObject.CompareTag("Player")) return;
            upgradeCanvas.gameObject.SetActive(false);
        }
    }
}
