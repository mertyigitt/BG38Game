using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace BG38Game
{
    public class TriggerEnterExit : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Girdi");
                other.transform.position = transform.position;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            /*if (other.CompareTag("Player"))
            {
                other.transform.SetParent(null);
            }*/
        }
    }
}
