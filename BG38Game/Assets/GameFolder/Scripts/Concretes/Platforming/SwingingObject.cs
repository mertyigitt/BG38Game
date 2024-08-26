using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class SwingingObject : MonoBehaviour
    {
        [SerializeField] private Vector3 rotateDirection = Vector3.forward;
        [SerializeField] private float swingAngle = 45f; // Max angle
        [SerializeField] private float swingSpeed = 2f;

        private Quaternion startRotation;
        private float x = 0f,y = 0f,z = 0f;
        private float currentTime = 0f;

        void Start()
        {
            startRotation = transform.rotation;
        }

        void FixedUpdate()
        {
            currentTime += Time.deltaTime * swingSpeed;
            float angle = Mathf.Sin(currentTime) * swingAngle;

            if(rotateDirection.x > 0) x= rotateDirection.x * angle;
            if(rotateDirection.y > 0) y= rotateDirection.y * angle;
            if(rotateDirection.z > 0) z= rotateDirection.z * angle;
            
            transform.rotation = startRotation * Quaternion.Euler(x, y, z);
        }
    }
}
