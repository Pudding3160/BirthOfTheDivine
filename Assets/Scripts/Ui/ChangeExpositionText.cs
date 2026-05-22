
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Ui
{
    public class ChangeExpositionText : MonoBehaviour
    {
        private GameObject thisText;
        [SerializeField] [CanBeNull] private GameObject nextText;
        [SerializeField] private float timer;
        private float timerBuffer;

        private void Start()
        {
            timerBuffer = timer;
        }

        private void Update()
        {
            timerBuffer -= Time.deltaTime;

            if (timerBuffer > 0) return;
            
            nextText?.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
