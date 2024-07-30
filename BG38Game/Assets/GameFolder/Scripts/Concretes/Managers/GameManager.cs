using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BG38Game.Controllers;
using BG38Game.Managers;
using TMPro;
using Unity.Mathematics;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BG38Game
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private GameObject[] levelPrefabs;
        [SerializeField] private Button startButton;
        [SerializeField] private Transform[] spawnPoint;
        [SerializeField] private GameObject lastLevel;
        [SerializeField] private int levelCount;
        [SerializeField] private GameObject pointUI;
        [SerializeField] private GameObject pointPanel;
        [SerializeField] private List<GameObject> panelIUs;
        public List<PointController> pointControllers;
        public List<GameObject> players;

        [SerializeField] private float waitAfterFinish = 3f;
        //[SerializeField] private float countdownTime = 3f;

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

        private void Start()
        {
            levelCount = 0;
        }

        public void StartGame()
        {
            StartCoroutine(LevelTransition());
        }

        private IEnumerator LevelTransition()
        {
            yield return new WaitForSeconds(waitAfterFinish);

            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                var playerObject = client.Value.PlayerObject;
                var playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.DisableCharacter();
                }
            }

            StartNewLevel();

            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                var playerObject = client.Value.PlayerObject;
                var playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Debug.Log("enable çaðýrýldý");
                    playerController.EnableCharacter();
                }
            }
        }

        public void StartNewLevel()
        {
            if (levelCount > levelPrefabs.Length - 1)
            {
                levelCount = 0;
            }
            if (lastLevel is not null)
            {
                Destroy(lastLevel);
            }
            var obj = Instantiate(levelPrefabs[levelCount], Vector3.zero, Quaternion.identity);
            lastLevel = obj;
            obj.GetComponent<NetworkObject>().Spawn();
            
            for (int i = 0; i < players.Count; i++)
            {
                RequestTeleportAllPlayersServerRpc(spawnPoint[i].position);
            }
            levelCount++;
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

        public void CreatePointUI()
        {
            CreatePointUIClientRPC();
        }
        [ClientRpc]
        public void CreatePointUIClientRPC()
        {
            if (panelIUs is not null)
            {
                foreach (var panelui in panelIUs)
                {
                    Destroy(panelui);
                }
                panelIUs.Clear();
            }

            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                var playerObject = client.Value.PlayerObject;
                var pointController = playerObject.GetComponent<PointController>();
                
                var obj = Instantiate(pointUI,Vector3.zero, Quaternion.identity);
                obj.GetComponent<NetworkObject>().Spawn();
                UpdateClientScore(playerObject.name, pointController.currentPoints.Value, obj.GetComponent<NetworkObject>().NetworkObjectId);
                obj.GetComponent<RectTransform>().SetParent(pointPanel.transform);
                panelIUs.Add(obj);
            }
            
            // paneluis = paneluis.OrderByDescending(child =>
            // {
            //     var scoreText = child.transform.Find("PlayerPointText").GetComponent<TextMeshProUGUI>();
            //     int score;
            //     if (int.TryParse(scoreText.text, out score))
            //     {
            //         return score;
            //     }
            //
            //     return 0;
            // }).ToList();
            //
            // for (int i = 0; i < paneluis.Count; i++)
            // {
            //     paneluis[i].transform.SetSiblingIndex(i);
            // }
        }
        
        [ClientRpc]
        void UpdateClientScoreClientRPC(string name, int score, ulong networkObjectId)
        {
            string playerName = name;
            var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (networkObject != null)
            {
                GameObject obj2 = networkObject.gameObject;
                obj2.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = name;
                obj2.transform.Find("PlayerPointText").GetComponent<TextMeshProUGUI>().text = score.ToString();
            }
            
        }
        
        void UpdateClientScore(string name, int score, ulong networkObjectId)
        {
            UpdateClientScoreClientRPC(name,score,networkObjectId);
        }
    }
}
