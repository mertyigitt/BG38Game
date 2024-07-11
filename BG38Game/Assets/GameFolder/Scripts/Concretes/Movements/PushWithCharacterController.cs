using System;
using System.Collections;
using System.Collections.Generic;
using BG38Game.Abstracts.Controllers;
using BG38Game.Abstracts.Movements;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

namespace BG38Game.Movements
{
    public class PushWithCharacterController : NetworkBehaviour , IPusher
    {
        [SerializeField] float pushPower;
        [SerializeField] float pushDuration;
        [SerializeField] float liftPower;
        private CharacterController _characterController;
        private bool _isPushing;

        public bool IsPushing 
        { 
            get { return _isPushing; }
            set { _isPushing = value; }
        }

        private void Awake()
        {
            _characterController = transform.GetComponent<CharacterController>();
        }

        
        public void PushAction()
        {
            _isPushing = true;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!IsOwner) return;
            if (_isPushing)
            {
                CharacterController otherCharacterController = hit.collider.GetComponent<CharacterController>();

                if (otherCharacterController != null && otherCharacterController != _characterController)
                {
                    Vector3 pushDir = new Vector3(hit.moveDirection.x, liftPower, hit.moveDirection.z);

                    StartCoroutine(ApplyPush(otherCharacterController, pushDir));
                }
            }
        }

        private IEnumerator ApplyPush(CharacterController otherCharacterController, Vector3 pushDir)
        {
            float elapsedTime = 0f;
            Vector3 finalPush = new Vector3(pushDir.x * pushPower, pushDir.y * liftPower, pushDir.z * pushPower);

            while (elapsedTime < pushDuration)
            {
                Vector3 pushStep = finalPush * (Time.deltaTime / pushDuration);
                otherCharacterController.Move(pushStep);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
