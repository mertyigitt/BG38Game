using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BG38Game
{
    public class GhostAI : MonoBehaviour
    {
        public List<Transform> patrolPoints;
        public float detectionRadius = 10f;
        public LayerMask playerLayerMask;

        private NavMeshAgent agent;
        private bool chasingPlayer = false;
        private Transform closestPlayer;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            GoToNextPatrolPoint();
        }

        void Update()
        {
            if (chasingPlayer)
            {
                if (closestPlayer != null)
                {
                    agent.SetDestination(closestPlayer.position);
                }
                else
                {
                    chasingPlayer = false;
                    GoToNextPatrolPoint();
                }
            }
            else
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    GoToNextPatrolPoint();
                }
            }

            DetectPlayers();
        }

        void GoToNextPatrolPoint()
        {
            if (patrolPoints.Count == 0)
                return;

            int randomIndex = Random.Range(0, patrolPoints.Count);
            agent.destination = patrolPoints[randomIndex].position;
        }

        void DetectPlayers()
        {
            Collider[] playersInRange = Physics.OverlapSphere(transform.position, detectionRadius, playerLayerMask);
            if (playersInRange.Length > 0)
            {
                closestPlayer = GetClosestPlayer(playersInRange);
                chasingPlayer = true;
            }
            else
            {
                closestPlayer = null;
                chasingPlayer = false;
            }
        }

        Transform GetClosestPlayer(Collider[] playersInRange)
        {
            Transform closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider player in playersInRange)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = player.transform;
                }
            }

            return closest;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
