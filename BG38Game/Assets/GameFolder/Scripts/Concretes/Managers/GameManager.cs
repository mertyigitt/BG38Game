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
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject lastLevel;
        public int levelCount;
        [SerializeField] private GameObject pointUI;
        [SerializeField] private GameObject pointPanel;
        [SerializeField] private List<GameObject> panelIUs;
        public List<PointController> pointControllers;
        public List<GameObject> players;

        [SerializeField] private float waitAfterFinish = 3f;
        public float[] levelTimes;
        [SerializeField] private TextMeshProUGUI levelTimeText;
        [SerializeField] private bool isStartLevel;
        [SerializeField] private GameObject finishObject;

        [SerializeField] private float time;
        [SerializeField] private GameObject timeCanvas;
        private GameObject existingCanvas;

        private float startTime;
        private bool isTiming;

        private void Awake()
        {
            if (Instance == null)
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
            if (IsServer)
            {
                levelCount = 0;
                UpdateLevelCountClientRpc(levelCount);
            }
        }

        public void StartGame()
        {
            StartCoroutine(LevelTransition());
        }

        private IEnumerator LevelTransition()
        {
            if (isTiming)
            {
                StopCoroutine("LevelStartTimerCoroutine");
                isTiming = false;
            }
            
            if (existingCanvas != null)
            {
                Destroy(existingCanvas);
                existingCanvas = null;
            }
            
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
                    Debug.Log("enable çağrıldı");
                    playerController.EnableCharacter();
                }
            }
        }

        void StartNewLevel()
        {
            StartNewLevelClientRpc();
        }

        [ClientRpc]
        public void StartNewLevelClientRpc()
        {
            if (IsServer)
            {
                if (levelCount > levelPrefabs.Length - 1)
                {
                    levelCount = 0;
                }
                if (lastLevel != null)
                {
                    lastLevel.GetComponent<NetworkObject>().Despawn();
                    //Destroy(lastLevel);
                }
                var obj = Instantiate(levelPrefabs[levelCount], Vector3.zero, Quaternion.identity);
                lastLevel = obj;
                obj.GetComponent<NetworkObject>().Spawn();

                for (int i = 0; i < players.Count && i < spawnPoints.Length; i++)
                {
                    RequestTeleportAllPlayersServerRpc(spawnPoints[i].position);
                }

                levelCount++;
                UpdateLevelCountClientRpc(levelCount);
                CreateTimeCanvas();

                // Zamanlayıcıyı başlat
            }
        }
    
        [ClientRpc]
        void UpdateLevelCountClientRpc(int newLevelCount)
        {
            levelCount = newLevelCount;
        }

        public void CreateTimeCanvas()
        {
            if (IsServer)
            {
                CreateTimeCanvasClientRpc();
            }
            else
            {
                CreateTimeCanvasServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreateTimeCanvasServerRpc()
        {
            CreateTimeCanvasClientRpc();
        }
    
        [ClientRpc]
        private void CreateTimeCanvasClientRpc()
        {
            if (existingCanvas != null)
            {
                Destroy(existingCanvas);
            }

            var canvas = Instantiate(timeCanvas, Vector3.zero, Quaternion.identity);
            existingCanvas = canvas;

            var levelTimeTextTransform = canvas.transform.Find("LevelTimeText (TMP)");
            if (levelTimeTextTransform == null)
            {
                Debug.LogError("LevelTimeText (TMP) not found in the canvas.");
                return;
            }
        
            levelTimeText = levelTimeTextTransform.GetComponent<TextMeshProUGUI>();
            if (levelTimeText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on LevelTimeText (TMP).");
                return;
            }

            levelTimeText.enabled = true;
        }

        public void StartTimer(float duration)
        {
            if (isTiming)
            {
                StopCoroutine("LevelTimerCoroutine");
            }

            startTime = Time.time;
            time = duration;
            isTiming = true;
            StartCoroutine(LevelTimerCoroutine());
        }
        private IEnumerator LevelTimerCoroutine()
        {
            while (time > 0)
            {
                float elapsed = Time.time - startTime;
                time = Mathf.Max(0, time - elapsed);
                startTime = Time.time;
                UpdateClientTimerClientRpc(time);
                yield return null;
            }


            FinishTrigger finishTrigger = finishObject.GetComponent<FinishTrigger>();
            finishTrigger.ResetFinishedPlayers();
            StartCoroutine(LevelTransition());
            CreatePointUI();
            isTiming = false;
            time = 0;
        }

        [ClientRpc]
        void UpdateClientTimerClientRpc(float time)
        {
            if (levelTimeText != null)
            {
                levelTimeText.text = time.ToString("0");
            }
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
            CreatePointUIClientRpc();
        }

        [ClientRpc]
        public void CreatePointUIClientRpc()
        {
            if (IsServer)
            {
                if (panelIUs != null)
                {
                    foreach (var panelui in panelIUs)
                    {
                        Destroy(panelui);
                    }
                    panelIUs.Clear();
                }

                pointPanel.GetComponent<Image>().enabled = true;

                foreach (var client in NetworkManager.Singleton.ConnectedClients)
                {
                    var playerObject = client.Value.PlayerObject;
                    var pointController = playerObject.GetComponent<PointController>();

                    var obj = Instantiate(pointUI, Vector3.zero, Quaternion.identity);
                    obj.GetComponent<NetworkObject>().Spawn();
                    UpdateClientScore(playerObject.name, pointController.currentPoints.Value, obj.GetComponent<NetworkObject>().NetworkObjectId);
                    obj.GetComponent<RectTransform>().SetParent(pointPanel.transform);
                    panelIUs.Add(obj);
                }
            }
        }

        [ClientRpc]
        void UpdateClientScoreClientRpc(string name, int score, ulong networkObjectId)
        {
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
            UpdateClientScoreClientRpc(name, score, networkObjectId);
        }
    }
}