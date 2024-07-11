using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BG38Game.Multiplayers
{
    public class CreateLobby : MonoBehaviour
    {
        [SerializeField] private TMP_InputField lobbyName;
        [SerializeField] private TMP_Dropdown maxPlayer;
        [SerializeField] private Toggle isLobbyPrivate;

        public async void CreateLobbyMetod()
        {
            string lobbyName = this.lobbyName.text;
            int maxPlayer = Convert.ToInt32(this.maxPlayer.options[this.maxPlayer.value].text);
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = isLobbyPrivate.isOn;

            options.Player = new Player(AuthenticationService.Instance.PlayerId);
            options.Player.Data = new Dictionary<string, PlayerDataObject>()
            {
                
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);
            GetComponent<CurrentLobby>().currentLobby = lobby;
            DontDestroyOnLoad(this);
            Debug.LogWarning("create lobby: " + lobby.Name);
            LogPlayersInLobby(lobby);
            StartCoroutine(HeartBeatLobbyCoroutine(lobby.Id, 15f));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        IEnumerator HeartBeatLobbyCoroutine(string lobbyId, float waitForSeconds)
        {
            var delay = new WaitForSeconds(waitForSeconds);
            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            }
        }

        void LogPlayersInLobby(Lobby lobby)
        {
            foreach (Player player in lobby.Players)
            {
                Debug.LogWarning("Player Id: " + player.Id);
            }
        }
    }
}
