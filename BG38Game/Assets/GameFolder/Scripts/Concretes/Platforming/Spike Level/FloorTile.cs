using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BG38Game
{
    public class FloorTile : NetworkBehaviour
    {
        public GameObject spikes; 
        [SerializeField] private Color flashColor = Color.red; 
        public float flashDuration = 0.2f; 
        public int flashCount = 3; 
        public float spikeUpTime = 2f; 

        private Color originalColor;
        private Renderer tileRenderer;
        private NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false);

        void Start()
        {
            tileRenderer = GetComponent<Renderer>();
            originalColor = tileRenderer.material.color;
            isActivated.OnValueChanged += HandleActivationChanged;
        }

        [ServerRpc]
        public void ActivateTileServerRpc()
        {
            isActivated.Value = true;
        }

        private void HandleActivationChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                StartCoroutine(ActivateTileCoroutine());
            }
        }

        private IEnumerator ActivateTileCoroutine()
        {
            for (int i = 0; i < flashCount; i++)
            {
                tileRenderer.material.color = flashColor;
                yield return new WaitForSeconds(flashDuration);
                tileRenderer.material.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }

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
