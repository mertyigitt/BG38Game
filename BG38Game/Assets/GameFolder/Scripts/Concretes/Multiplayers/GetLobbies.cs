using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

namespace BG38Game.Multiplayers
{
    public class GetLobbies : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject buttonsContainer;
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void GetLobbiesTest()
        {
            ClearContainer();
            try
            {
                QueryLobbiesOptions options = new();
                Debug.LogWarning("QueryLobbiesTest");
                options.Count = 25;

                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0" , QueryFilter.OpOptions.GT)
                };

                options.Order = new List<QueryOrder>()
                {
                    new QueryOrder(true, QueryOrder.FieldOptions.Created)
                };

                QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
                Debug.LogWarning("Get Lobbies Done Count: "+ lobbies.Results.Count);
                foreach (Lobby lobby in lobbies.Results)
                {
                    CreateLobbyButton(lobby);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        void CreateLobbyButton(Lobby lobby)
        {
            var button = Instantiate(buttonPrefab, Vector3.zero, quaternion.identity);
            button.name = lobby.Name + "_Button";
            button.GetComponentInChildren<TextMeshProUGUI>().text = lobby.Name;
            var recTronsform = button.GetComponent<RectTransform>();
            recTronsform.SetParent(buttonsContainer.transform);
            button.GetComponent<Button>().onClick.AddListener(delegate() {LobbyOnClick(lobby);});
        }
        
        public void LobbyOnClick(Lobby lobby)
        {
            Debug.Log("Clicked Lobby: " + lobby.Name);
            GetComponent<JoinLobby>().JoinLobbyWithLobbyId(lobby.Id);
        }

        void ClearContainer()
        {
            if (buttonsContainer is not null && buttonsContainer.transform.childCount > 0)
            {
                foreach (Transform child in buttonsContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
