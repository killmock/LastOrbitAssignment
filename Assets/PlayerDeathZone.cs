using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerDeathZone : MonoBehaviour
    {
        public GameObject gameOverScreen;

        private bool isDead = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("DeathZone"))
            {
                Die();
            }
        }

        private void Die()
        {
            if (!isDead)
            {
                isDead = true;
                // Deactivate character or perform any death animation
                gameObject.SetActive(false);

                // Show the Game Over screen
                gameOverScreen.SetActive(true);
            }
        }
    }

}