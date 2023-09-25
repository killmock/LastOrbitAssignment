using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class DamageController : MonoBehaviour
    {
        [SerializeField] private int damage = 10;

        [SerializeField] private HealthController healthController;

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Player"))
            {
                healthController.TakeDamage(damage);
            }
        }

    }
}
