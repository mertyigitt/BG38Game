using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BG38Game.UIs
{
    public class StartNetworkButtonAction : MonoBehaviour
    {
        private NetworkManager _networkManager;

        private void Awake()
        {
            _networkManager = GetComponentInParent<NetworkManager>();
        }
        
        public void StartHost()
        {
            _networkManager.StartHost();
        }
        
        public void StartClient()
        {
            _networkManager.StartClient();
        }
    }
}
