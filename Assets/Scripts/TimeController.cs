using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class TimeController : MonoBehaviour
    {
        [Header("Component")]
        public TextMeshProUGUI timerText;

        [Header("Timer Settings")]
        public float currentTime;
        public bool countDown;

        [Header("Limit Settings")]
        public bool hasLimit;
        public float timerLimit;


        private void Update()
        {
            currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;
            timerText.text = currentTime.ToString();

            if(hasLimit && countDown && currentTime <= timerLimit)
            {
                currentTime = timerLimit;
                SetTimerText();
                timerText.color = Color.red;
                enabled = false;
            }
            SetTimerText();

            if (currentTime <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        private void SetTimerText()
        {
            timerText.text = currentTime.ToString("0.0");
        }

        public void AddTime(float timeToAdd)
        {
            currentTime += timeToAdd;
            SetTimerText();
        }
    }
}
