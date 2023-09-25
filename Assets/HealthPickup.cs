using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class HealthPickup : MonoBehaviour
    {
        [SerializeField] private float healthAmount; 
        [SerializeField] private HealthController healthController; 

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                healthController.GainHealth(healthAmount);
                Destroy(gameObject);
            }
        }
    }
}
