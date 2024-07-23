using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class SpikeManager : MonoBehaviour
    {
        public List<FloorTile> floorTiles; // List of all floor tiles
        [SerializeField] private float waitBetweenRounds = 2f; // Time to wait before starting the next round
        [SerializeField] private int tilesToActivate = 22; // Number of tiles to activate each round
        [SerializeField] private int tileIncrease = 2;
        [SerializeField] private int maxTile = 61;

        void Start()
        {
            StartCoroutine(StartRounds());
        }

        private IEnumerator StartRounds()
        {
            yield return new WaitForSeconds(waitBetweenRounds);

            while (true)
            {
                List<FloorTile> selectedTiles = SelectRandomTiles(tilesToActivate);

                foreach (FloorTile tile in selectedTiles)
                {
                    StartCoroutine(tile.ActivateTile());
                }

                // Calculate the total duration of the current round
                float totalRoundTime = waitBetweenRounds;
                if (selectedTiles.Count > 0)
                {
                    totalRoundTime += (selectedTiles[0].flashDuration * selectedTiles[0].flashCount * 2) + selectedTiles[0].spikeUpTime;
                }

                // Wait until the round is over and increase the number of tiles for the next round
                yield return new WaitForSeconds(totalRoundTime);
                
                if(tilesToActivate < maxTile)
                {
                    tilesToActivate = tilesToActivate + tileIncrease;
                }
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
