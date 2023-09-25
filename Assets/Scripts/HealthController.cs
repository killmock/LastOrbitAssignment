using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


namespace Platformer
{
    public class HealthController : MonoBehaviour
    {
        [Header("Health Parameters")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float smoothDecreaseDuration = 0.5f;
       

        [Header("UI Parameters")]
        [SerializeField] private TMP_Text healthText;

        private float currentHealth = 100f;

        private void Start()
        {
            currentHealth = maxHealth;
            UpdateHealthText();
        }



        public void TakeDamage(float damage)
        {
            StartCoroutine(SmoothDecreaseHealth(damage));
        }
        void UpdateHealthText()
        {
            healthText.text = currentHealth.ToString("0");
        }

        private IEnumerator SmoothDecreaseHealth(float damage)
        {
            float damagePerTick = damage / smoothDecreaseDuration;
            float elapsedTime = 0f;

            while (elapsedTime < smoothDecreaseDuration)
            {
                float currentDamage = damagePerTick * Time.deltaTime;
                currentHealth -= currentDamage;
  
                elapsedTime += Time.deltaTime;

                UpdateHealthText();

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                }
                yield return null;
            }
        }

        public void GainHealth(float healAmount)
        {
          
            StartCoroutine(SmoothIncreaseHealth(healAmount));

        }



        private IEnumerator SmoothIncreaseHealth(float healAmount)
        {
          

            float healingPerTick = healAmount / smoothDecreaseDuration;
            float elapsedTime = 0f;
           
            while (elapsedTime < smoothDecreaseDuration)
            {
                float currentHeal = healingPerTick * Time.deltaTime;
                currentHealth += currentHeal;
                elapsedTime += Time.deltaTime;

                UpdateHealthText();

                yield return null;
            }
        }
    }
}
