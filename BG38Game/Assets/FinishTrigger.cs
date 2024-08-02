using BG38Game.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace BG38Game
{
    public class FinishTrigger : NetworkBehaviour
{
    [SerializeField] private int totalPlayer;
    [SerializeField] private int finishedPlayers = 0;
    private int[] pointsForPositions;

    private HashSet<GameObject> finishedPlayerSet = new HashSet<GameObject>();

    private void Start()
    {
        totalPlayer = NetworkManager.Singleton.ConnectedClientsIds.Count;
        finishedPlayers = 0;
        pointsForPositions = new int[totalPlayer];

        for (int i = 0; i < totalPlayer; i++)
        {
            pointsForPositions[i] = (totalPlayer - i) * 100;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !finishedPlayerSet.Contains(other.gameObject))
        {
            PointController pointController = other.GetComponent<PointController>();

            if (pointController != null && finishedPlayers < totalPlayer)
            {
                finishedPlayers++;
                int playerRank = finishedPlayers - 1;

                // Ensure that the method is called only once per player
                if (!IsServer) return;

                pointController.AddPointsServerRpc(pointsForPositions[playerRank]);
                Debug.Log($"{other.gameObject.name} finished in position {playerRank + 1} and received {pointsForPositions[playerRank]} points.");

                DisablePlayer(other.gameObject);
                finishedPlayerSet.Add(other.gameObject);
            }

            if (finishedPlayers == totalPlayer)
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
        finishedPlayers = 0;
        finishedPlayerSet.Clear();
    }
}
    // public class FinishTrigger : MonoBehaviour
    // {
    //     [SerializeField] private int totalPlayer;
    //     [SerializeField] private int finishedPlayers = 0;
    //     private int[] pointsForPositions;
    //
    //     private void Start()
    //     {
    //         totalPlayer = NetworkManager.Singleton.ConnectedClientsIds.Count;
    //         finishedPlayers = 0;
    //         pointsForPositions = new int[totalPlayer];
    //
    //         for (int i = 0; i < totalPlayer; i++)
    //         {
    //             pointsForPositions[i] = (totalPlayer - i) * 100;
    //         }
    //     }
    //
    //     private void OnTriggerEnter(Collider other)
    //     {
    //         if(other.CompareTag("Player"))
    //         {
    //             PointController pointController = other.GetComponent<PointController>();
    //
    //             if (pointController != null && finishedPlayers < totalPlayer)
    //             {
    //                 finishedPlayers++;
    //                 int playerRank = finishedPlayers - 1;
    //
    //                 pointController.AddPoints(pointsForPositions[playerRank]);
    //                 Debug.Log($"{other.gameObject.name} finished in position {playerRank + 1} and received {pointsForPositions[playerRank]} points.");
    //
    //                 DisablePlayer(other.gameObject);
    //             }
    //             
    //             if (finishedPlayers == totalPlayer)
    //             {
    //                 GameManager.Instance.StartGame();
    //                 GameManager.Instance.CreatePointUI();
    //                 resetFinishedPlayers();
    //             }
    //         }
    //     }
    //
    //     private void DisablePlayer(GameObject player)
    //     {
    //         var playerController = player.GetComponent<PlayerController>();
    //         if (playerController != null)
    //         {
    //             playerController.DisableCharacter();
    //         }
    //     }
    //
    //     public void resetFinishedPlayers()
    //     {
    //         finishedPlayers = 0;
    //     }
    // }
}
