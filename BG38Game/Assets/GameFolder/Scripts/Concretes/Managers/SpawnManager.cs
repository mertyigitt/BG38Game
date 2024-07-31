using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BG38Game.Managers
{
    public class SpawnManager : NetworkBehaviour
    {
        public static SpawnManager Instance;
        [SerializeField] GameObject[] characterPrefabs;
        
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
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            SpawnPlayer(clientId);
        }

        public void SpawnPlayer(ulong clientId)
        {
            int characterIndex = GetCharacterIndex(clientId);
            GameObject characterPrefab = characterPrefabs[characterIndex];
            GameObject playerInstance = Instantiate(characterPrefab, GetSpawnPosition(), Quaternion.identity);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            GameManager.Instance.players.Add(playerInstance.gameObject);
            GameManager.Instance.pointControllers.Add(playerInstance.GetComponent<PointController>());
            //GameManager.Instance.CreatePointUI();
        }

        private int GetCharacterIndex(ulong clientId)
        {
            // Oyuncu ID'sine göre bir indeks hesaplama mantığı
            // Örneğin, clientId mod prefab sayısı
            return (int)(clientId % (ulong)characterPrefabs.Length);
        }

        private Vector3 GetSpawnPosition()
        {
            // Oyuncuların spawn pozisyonunu belirleme mantığı
            return new Vector3(0, 1, 0); // Basit bir örnek
        }
    }
}
