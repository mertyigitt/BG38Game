using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private float moveDistance = 1f; // Distance the spikes move upwards
        [SerializeField] private float moveSpeed = 2f; // Speed of the spikes moving upwards

        private Vector3 originalPosition;
        private Vector3 targetPosition;

        void Start()
        {
            originalPosition = transform.localPosition;
            targetPosition = originalPosition + new Vector3(0, moveDistance, 0);
        }

        public void ActivateSpike(float spikeUpTime)
        {
            StartCoroutine(MoveSpikes(spikeUpTime));
        }

        private IEnumerator MoveSpikes(float spikeUpTime)
        {
            // Move spikes upwards
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Wait for the spikes to stay up
            yield return new WaitForSeconds(spikeUpTime);

            // Move spikes back down
            while (Vector3.Distance(transform.localPosition, originalPosition) > 0.01f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
