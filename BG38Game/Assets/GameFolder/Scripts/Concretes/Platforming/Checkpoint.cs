using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Teleportable teleportable = other.GetComponent<Teleportable>();
                if (teleportable != null)
                {
                    teleportable.checkPointPosition = transform.position;
                }
            }
        }
    }
}
