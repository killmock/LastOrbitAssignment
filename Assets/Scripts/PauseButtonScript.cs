using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class PauseButtonScript : MonoBehaviour
    {
        public static bool Paused = false;
        public GameObject PauseMenuCanvas;

        void Start()
        {
            Time.timeScale = 1f;

        }

        
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (Paused)
                {
                    Play();
                }
                else
                {
                    Stop();
                }
            }
        }


        void Stop()
        {
            PauseMenuCanvas.SetActive(true);
            Time.timeScale = 0f;
            Paused = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Play()
        {
            PauseMenuCanvas.SetActive(false);
            Time.timeScale = 1f;
            Paused = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void MainMenuButton()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

}
