using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class MovingPlatform : MonoBehaviour
    {
        public Vector3 moveDirection = Vector3.right; 
        public float moveDistance = 5f; 
        public float speed = 2f; 
        public float delayTime = 1f; 

        private Vector3 startPosition; 
        private bool movingForward = true; 
        private bool isWaiting = false; 

        void Start()
        {
            startPosition = transform.position;
        }

        void FixedUpdate()
        {
            if (!isWaiting)
            {
                MovePlatform();
            }
        }

        private void MovePlatform()
        {
            Vector3 targetPosition = startPosition + (moveDirection.normalized * moveDistance);

            if (movingForward)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    StartCoroutine(WaitAtEnd());
                    movingForward = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                {
                    StartCoroutine(WaitAtEnd());
                    movingForward = true;
                }
            }
        }

        private IEnumerator WaitAtEnd()
        {
            isWaiting = true;
            yield return new WaitForSeconds(delayTime);
            isWaiting = false;
        }
    }
}
