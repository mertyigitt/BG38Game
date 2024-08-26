using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class RotatingPlatform : MonoBehaviour
    {
        [SerializeField] private Vector3 rotateDirection = Vector3.right;
        [SerializeField] private float rotationSpeed = 3f;

        void FixedUpdate()
        {
            transform.Rotate(rotateDirection*rotationSpeed);
        }
    }
}
