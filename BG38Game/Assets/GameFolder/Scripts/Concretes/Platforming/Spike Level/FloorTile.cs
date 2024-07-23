using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class FloorTile : MonoBehaviour
    {
        public GameObject spikes; 
        [SerializeField] private Color flashColor = Color.red; 
        public float flashDuration = 0.2f; 
        public int flashCount = 3; 
        public float spikeUpTime = 2f; 

        private Color originalColor;
        private Renderer tileRenderer;

        void Start()
        {
            tileRenderer = GetComponent<Renderer>();
            originalColor = tileRenderer.material.color;
        }

        public IEnumerator ActivateTile()
        {
            // Flash the tile
            for (int i = 0; i < flashCount; i++)
            {
                tileRenderer.material.color = flashColor;
                yield return new WaitForSeconds(flashDuration);
                tileRenderer.material.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }

            // Activate the spikes
            if (spikes != null)
            {
                Spikes spikesScript = spikes.GetComponent<Spikes>();
                if (spikesScript != null)
                {
                    spikesScript.ActivateSpike(spikeUpTime);
                }
            }
        }
    }
}
