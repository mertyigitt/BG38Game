using System;
using System.Collections;
using System.Collections.Generic;
using BG38Game.Managers;
using Unity.Mathematics;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace BG38Game
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private Button startButton;
        [SerializeField] private Transform[] spawnPoint;
        public NetworkObject[] pl;
        public List<GameObject> players;

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        
        public void StartGame()
        {
            for (int i = 0; i < players.Count; i++)
            {
                RequestTeleportAllPlayersServerRpc(spawnPoint[i].position);
            }
            
            Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void RequestTeleportAllPlayersServerRpc(Vector3 position)
        {
            foreach (var player in players)
            {
                var teleportable = player.GetComponent<Teleportable>();
                if (teleportable != null)
                {
                    teleportable.TeleportClientRpc(position);
                }
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
