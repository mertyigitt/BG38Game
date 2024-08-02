using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BG38Game
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject objectToSpawn; // The prefab to be spawned
        [SerializeField] private float spawnInterval = 5f; // Time interval between spawns
        [SerializeField] private Transform spawnLocation; // The specific location to spawn objects

        private void Start()
        {
            StartCoroutine(SpawnObjects());
        }

        private IEnumerator SpawnObjects()
        {
            while (true)
            {
                Instantiate(objectToSpawn, spawnLocation.position, spawnLocation.rotation);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
