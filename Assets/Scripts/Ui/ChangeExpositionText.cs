
using System;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Ui
{
    public class ChangeExpositionText : MonoBehaviour
    {
        private GameObject thisText;
        [SerializeField] [CanBeNull] private GameObject nextText;
        [SerializeField] private float timer;
        private float timerBuffer;
        [SerializeField] private string sceneName;

        private void Start()
        {
            timerBuffer = timer;
        }

        private void Update()
        {
            timerBuffer -= Time.deltaTime;

            if (timerBuffer > 0) return;
            
            if (nextText) nextText.SetActive(true);
            gameObject.SetActive(false);
            if (sceneName != "") SceneManager.LoadScene(sceneName);
        }
    }
}
