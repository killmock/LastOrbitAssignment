using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class CollectableItem : MonoBehaviour
    {
        public float timeToAdd = 20.0f; 

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) 
            {
                TimeController timeController = FindObjectOfType<TimeController>();
                if (timeController != null)
                {
                    timeController.AddTime(timeToAdd);
                }

                Destroy(gameObject); 
            }
        }
    }
}
