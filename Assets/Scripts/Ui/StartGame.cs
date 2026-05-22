using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField]
        private string sceneName;

        public void ChangeScene()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
