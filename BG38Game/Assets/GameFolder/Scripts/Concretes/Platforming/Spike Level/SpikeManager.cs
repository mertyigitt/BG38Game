using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BG38Game
{
    public class SpikeManager : NetworkBehaviour
    {
        public List<FloorTile> floorTiles; 
        [SerializeField] private float waitBetweenRounds = 2f; 
        [SerializeField] private int tilesToActivate = 22; 
        [SerializeField] private int tileIncrease = 2;
        [SerializeField] private int maxTile = 61;

        void Start()
        {
            if (IsServer)
            {
                StartCoroutine(StartRounds());
            }
        }

        private IEnumerator StartRounds()
        {
            yield return new WaitForSeconds(6f);

            while (true)
            {
                List<FloorTile> selectedTiles = SelectRandomTiles(tilesToActivate);

                foreach (FloorTile tile in selectedTiles)
                {
                    tile.ActivateTileServerRpc();
                }

                float totalRoundTime = waitBetweenRounds;
                if (selectedTiles.Count > 0)
                {
                    totalRoundTime += (selectedTiles[0].flashDuration * selectedTiles[0].flashCount * 2) + selectedTiles[0].spikeUpTime;
                }

                yield return new WaitForSeconds(totalRoundTime);

                if (tilesToActivate < maxTile)
                {
                    tilesToActivate = Mathf.Min(tilesToActivate + tileIncrease, maxTile);
                }

                Debug.Log($"Next round will have {tilesToActivate} tiles to activate.");
            }
        }

        private List<FloorTile> SelectRandomTiles(int number)
        {
            List<FloorTile> selectedTiles = new List<FloorTile>();
            List<FloorTile> availableTiles = new List<FloorTile>(floorTiles);
            int randomIndex;

            for (int i = 0; i < number; i++)
            {
                if (availableTiles.Count == 0) break;

                randomIndex = Random.Range(0, availableTiles.Count);
                selectedTiles.Add(availableTiles[randomIndex]);
                availableTiles.RemoveAt(randomIndex);
            }

            return selectedTiles;
        }
    }
}
