using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace BG38Game.Managers
{
    public class RelayManager : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown playerCount;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI joinCode;
        private RelayHostData _hostData;
        private RelayJoinData _joinData;
        private string _playerID;
        
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            SignIn();
        }
        
        async void SignIn()
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _playerID = AuthenticationService.Instance.PlayerId;
        }
        
        public async void OnHostClick()
        {
            int maxPlayerCount = Convert.ToInt32(playerCount.options[playerCount.value].text);
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayerCount);
            _hostData = new RelayHostData()
            {
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                Key = allocation.Key
            };
            _hostData.JoinCode = await RelayService.Instance.GetJoinCodeAsync(_hostData.AllocationID);
            joinCode.text = "JoinCode: " + _hostData.JoinCode;
            Debug.LogWarning("Join Code: " + _hostData.JoinCode);
            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            transport.SetRelayServerData(_hostData.IPv4Address, _hostData.Port, _hostData.AllocationIDBytes, _hostData.Key, _hostData.ConnectionData);
            NetworkManager.Singleton.StartHost();
            
        }

        public async void OnJoinClick()
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(inputField.text);
            _joinData = new RelayJoinData()
            {
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                Key = allocation.Key
            };
            Debug.LogWarning("Join Successful" + _joinData.AllocationID);
            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            transport.SetRelayServerData(_joinData.IPv4Address, _joinData.Port, _joinData.AllocationIDBytes, 
                _joinData.Key, _joinData.ConnectionData, _joinData.HostConnectionData);
            NetworkManager.Singleton.StartClient();
        }
    }

    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }
    
    public struct RelayJoinData
    {
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }
}
