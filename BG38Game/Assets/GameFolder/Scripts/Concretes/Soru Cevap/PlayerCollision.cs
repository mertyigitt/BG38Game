using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class PlayerCollision : MonoBehaviour
    {
        public DataManager datamanager;

        void OnTriggerEnter(Collider other)
        {
            Platform platform = other.GetComponent<Platform>();
            if (platform != null)
            {
                if (platform.index == datamanager.correctIndex)
                {
                    Debug.Log("Correct");
                }
                else
                {
                    Debug.Log("Wrong");
                }
            }
        }
    }
}
