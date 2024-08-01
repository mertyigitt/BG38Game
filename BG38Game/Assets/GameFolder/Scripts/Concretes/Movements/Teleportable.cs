using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace BG38Game
{
    public class Teleportable : NetworkBehaviour
    {
        public Vector3 checkPointPosition;

        private void Start()
        {
            checkPointPosition = transform.position;
        }

        [ClientRpc]
        public void TeleportClientRpc(Vector3 position)
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false; // CharacterController'ı geçici olarak devre dışı bırak
                transform.position = position;
                controller.enabled = true; // CharacterController'ı tekrar etkinleştir
            }
            else
            {
                // Eğer CharacterController yoksa doğrudan pozisyonu ayarla
                transform.position = position;
            }
        }
    }
}
