using System;
using UnityEngine;

namespace Pentagram
{
    public class Fire : MonoBehaviour
    {
        private Collider2D detectionArea;
        [SerializeField]
        private Canvas upgradeCanvas;
        
        private void Awake()
        {
            detectionArea = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            upgradeCanvas.gameObject.SetActive(true);
        }
        
        private void OnTriggerExit2D(Collider2D other) 
        {
            if (!other.gameObject.CompareTag("Player")) return;
            upgradeCanvas.gameObject.SetActive(false);
        }
    }
}
