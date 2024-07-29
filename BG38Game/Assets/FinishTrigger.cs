using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BG38Game
{
    public class FinishTrigger : MonoBehaviour
    {
        [SerializeField] private int totalPlayer;
        [SerializeField] private int finishedPlayers;

        private void Start()
        {
            totalPlayer = NetworkManager.Singleton.ConnectedClientsIds.Count;
            finishedPlayers = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                finishedPlayers++;
                if (finishedPlayers == totalPlayer)
                {
                    GameManager.Instance.StartGame();
                }
            }
        }
    }
}
