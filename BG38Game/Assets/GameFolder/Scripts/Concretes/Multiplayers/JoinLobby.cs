using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BG38Game.Multiplayers
{
    public class JoinLobby : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        

        public async void JoinLobbyWithLobbyCode(string lobbycode)
        {
            var code = inputField.text;
            try
            {
                JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
                options.Player = new Player(AuthenticationService.Instance.PlayerId);
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code,options);
                Debug.LogWarning("Joined lobby: " + code);
                DontDestroyOnLoad(this);
                GetComponent<CurrentLobby>().currentLobby = lobby;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        public async void JoinLobbyWithLobbyId(string lobbyId)
        {
            try
            {
                JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
                options.Player = new Player(AuthenticationService.Instance.PlayerId);
                Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
                DontDestroyOnLoad(this);
                GetComponent<CurrentLobby>().currentLobby = lobby;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        public async void QuickJoin()
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                GetComponent<CurrentLobby>().currentLobby = lobby;
                DontDestroyOnLoad(this);
                Debug.LogWarning("Joined lobby: " + lobby.LobbyCode);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
