using BG38Game.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace BG38Game
{
    public class SpikeKill : NetworkBehaviour
    {
        [SerializeField] private int totalPlayer;
        //[SerializeField] private int finishedPlayers = 0;
        private int[] pointsForPositions;

        private HashSet<GameObject> finishedPlayerSet = new HashSet<GameObject>();

        private void Start()
        {
            totalPlayer = NetworkManager.Singleton.ConnectedClientsIds.Count;
            GameManager.Instance.managerFinishedPlayers = 0;
            pointsForPositions = new int[totalPlayer];

            for (int i = 0; i < totalPlayer; i++)
            {
                pointsForPositions[i] = (i+1) * 100;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !finishedPlayerSet.Contains(other.gameObject))
            {
                PointController pointController = other.GetComponent<PointController>();

                if (pointController != null && GameManager.Instance.managerFinishedPlayers < totalPlayer)
                {
                    GameManager.Instance.managerFinishedPlayers++;
                    int playerRank = GameManager.Instance.managerFinishedPlayers - 1;
                    
                    if (!IsServer) return;

                    pointController.AddPointsServerRpc(pointsForPositions[playerRank]);
                    Debug.Log($"{other.gameObject.name} finished in position {playerRank + 1} and received {pointsForPositions[playerRank]} points.");

                    DisablePlayer(other.gameObject);
                    finishedPlayerSet.Add(other.gameObject);
                }

                if (GameManager.Instance.managerFinishedPlayers == totalPlayer)
                {
                    GameManager.Instance.StartGame();
                    ResetFinishedPlayers();
                }
            }
        }

        private void DisablePlayer(GameObject player)
        {
            var playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.DisableCharacter();
            }
        }

        public void ResetFinishedPlayers()
        {
            GameManager.Instance.resetFinished();
            finishedPlayerSet.Clear();
        }
    }
}
