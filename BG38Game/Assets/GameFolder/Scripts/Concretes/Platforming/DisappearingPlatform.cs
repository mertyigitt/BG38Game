using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class DisappearingPlatform : MonoBehaviour
    {
        [SerializeField] private float disappearingTime = 2f; // Time before the platform disappears after being triggered
        [SerializeField] private float waitTime = 3f; // Time before the platform reappears after disappearing (0 means it doesn't reappear)

        private MeshRenderer meshRenderer;
        private Collider platformCollider;
        private bool isTriggered = false;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            platformCollider = GetComponent<Collider>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!isTriggered && other.CompareTag("Player"))
            {
                isTriggered = true;
                StartCoroutine(Disappear());
            }
        }

        private IEnumerator Disappear()
        {
            yield return new WaitForSeconds(disappearingTime);

            meshRenderer.enabled = false;
            platformCollider.enabled = false;

            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
                Reappear();
            }
        }

        private void Reappear()
        {
            meshRenderer.enabled = true;
            platformCollider.enabled = true;
            isTriggered = false;
        }
    }
}
