using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class DamageController : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;

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
