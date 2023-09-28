using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class WinningScript : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
        }
    }
}
