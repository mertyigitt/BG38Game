using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BG38Game.Multiplayers
{
    public class PopulateUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyName;
        [SerializeField] private TextMeshProUGUI lobbyCode;
        [SerializeField] private GameObject playerInfoContainer;
        [SerializeField] private GameObject playerInfoPrefab;
        [SerializeField] private List<Player> players;
        private CurrentLobby _currentLobby;
        private string lobbyId;

        private void Start()
        {
            _currentLobby = GameObject.Find("LobbyManager").GetComponent<CurrentLobby>();
            PopulateUIElements();
            lobbyId = _currentLobby.currentLobby.Id;
            InvokeRepeating(nameof(PollForLobbyUpdate), 1.1f, 3f);
        }

        void PopulateUIElements()
        {
            lobbyName.text = _currentLobby.currentLobby.Name;
            lobbyCode.text = _currentLobby.currentLobby.LobbyCode;
            ClearContainer();
            foreach (Player player in _currentLobby.currentLobby.Players)
            {
                CreatePlayerInfoCard(player);
            }
        }

        void CreatePlayerInfoCard(Player player)
        {
            var text = Instantiate(playerInfoPrefab, Vector3.zero, quaternion.identity);
            text.name = player.Id;
            text.GetComponent<TextMeshProUGUI>().text = player.Id;
            var rectTransform = text.GetComponent<RectTransform>();
            rectTransform.SetParent(playerInfoContainer.transform);
        }

        async void PollForLobbyUpdate()
        {
            _currentLobby.currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
            PopulateUIElements();
        }
        void ClearContainer()
        {
            if (playerInfoContainer is not null && playerInfoContainer.transform.childCount > 0)
            {
                foreach (Transform child in playerInfoContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        
        public void NextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
