using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class TriggerTrap : MonoBehaviour
    {
        [SerializeField] private Vector3 moveDirection = Vector3.forward; // Direction to move the trap
        [SerializeField] private float moveDistance = 5f; // Distance the trap will move
        [SerializeField] private float speed = 2f; // Speed of the trap movement
        [SerializeField] private float triggerDelay = 1f;
        [SerializeField] private float waitTime = 3f; // Delay time before the trap returns to the starting position

        private Vector3 startPosition; // Initial position of the trap
        private bool isTriggered = false; // Whether the trap has been triggered

        void Start()
        {
            // Save the starting position of the trap
            startPosition = transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            // Trigger the trap if a player enters the trigger zone and the trap has not been triggered yet
            if (!isTriggered && other.CompareTag("Player"))
            {
                isTriggered = true;
                StartCoroutine(ActivateTrap());
            }
        }

        private IEnumerator ActivateTrap()
        {
            yield return new WaitForSeconds(triggerDelay);

            // Calculate the target position based on the direction and distance
            Vector3 targetPosition = startPosition + (moveDirection.normalized * moveDistance);

            // Move towards the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            // Wait for the specified delay time
            yield return new WaitForSeconds(waitTime);

            // Only return to the starting position if delay time is 10 seconds or less
            if (waitTime <= 10f)
            {
                // Move back to the starting position
                while (Vector3.Distance(transform.position, startPosition) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
                    yield return null;
                }
            }

            isTriggered = false;
        }
    }
}
