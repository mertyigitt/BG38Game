using System;
using System.Collections;
using BG38Game.Abstracts.Controllers;
using BG38Game.Abstracts.Inputs;
using BG38Game.Abstracts.Movements;
using BG38Game.Animations;
using BG38Game.Movements;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace BG38Game.Controllers
{
    public class PlayerController : NetworkBehaviour, IEntityController
    {
        #region Self Variables

        #region Serialized Variables

        [Header("Movement Informations")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float turnSpeed = 10f;
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private Transform turnTransform;

        [SerializeField] private float waitForKick = 0.86f;
        [SerializeField] private float animationAcceleration = 4f;
        [SerializeField] private float countdownTime = 5f;
        
        [SerializeField] private Camera cam;
        [SerializeField] private CinemachineVirtualCamera camController;

        //[SerializeField] private float knockbackForce = 20f; // The force with which to throw the player

        #endregion

        #region Private Variables

        private IInputReader _input;
        private IMover _mover;
        private IRotator _xRotator;
        private IRotator _yRotator;
        private IJumper _jumper;
        private IPusher _pusher;
        private Vector3 _direction;
        private Vector2 _rotation;
        private PlayerAnimation _animation;
        private Gravity _gravity;
        private bool myPush;
        private float velocityX;
        private float velocityZ;
        private AudioSource _audioSource;

        //private CharacterController characterController;

        private bool isKnockedBack = false;
        /*private float knockbackDuration = 0.5f; // How long the knockback lasts
        private float knockbackTimer = 0f;*/
        private float tempSpeed = 10f;

        #endregion

        #region Public Variables

        public Transform TurnTransform => turnTransform;

        #endregion

        #endregion
        

        private void Awake()
        {
            _input = GetComponent<IInputReader>();
            _mover = new MoveWithCharacterController(this);
            _xRotator = new RotatorXCharacter(this);
            _yRotator = new RotatorYCharacter(this);
            _jumper = new JumpWithCharacterController(this);
            _pusher = GetComponent<PushWithCharacterController>();
            _animation = new PlayerAnimation(this);
            _gravity = GetComponent<Gravity>();
            myPush = false;
            _audioSource = GetComponent<AudioSource>();

            //characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            if (!IsOwner)
            {
                cam.enabled = false;
                camController.enabled = false;
            }

            tempSpeed = moveSpeed;
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!isKnockedBack) _direction = _input.Direction;
            AnimationDirection();

            _xRotator.RotationAction(_input.Rotation.x, turnSpeed);
            _yRotator.RotationAction(_input.Rotation.y,turnSpeed);

            /*if (isKnockedBack)
            {
                knockbackTimer -= Time.deltaTime;
                if (knockbackTimer <= 0)
                {
                    moveSpeed = tempSpeed;
                    isKnockedBack = false;
                    //velocity = Vector3.zero;
                }
            }*/
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            if (myPush) return;

            Vector3 moveDirection = _direction * moveSpeed;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up);

            _mover.MoveAction(moveDirection.normalized, moveSpeed);

            if (_input.IsJump)
            {
                if(_gravity.IsGroundedValue) _audioSource.Play();

                _jumper.JumpAction(jumpSpeed);
            }

            if (_input.IsPush && _gravity.IsGroundedValue)
            {
                myPush = true;
                _pusher.PushAction();

                StartCoroutine(WaitForKick());
            }
        }

        private void LateUpdate()
        {
            //2-playerAnimation script dosyasında yapılmış olan fonksiyonlar burada çalıştırılır.
            if (!IsOwner) return;

            _animation.JumpAnimation(_input.IsJump);
            _animation.Grounded(_gravity.IsGroundedValue);
            _animation.KickAnimation(myPush);
            _animation.BasicMoveAnimation(velocityX, velocityZ);
        }


        /*void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Sledgehammer"))
            {
                //Debug.Log("We did it?");
                Vector3 forceDirection = transform.position - other.transform.position;
                
                forceDirection.y = 1f;
                

                forceDirection.Normalize();

                ApplyKnockback(forceDirection, knockbackForce);
            }
        }

        public void ApplyKnockback(Vector3 direction, float force)
        {
            Debug.Log("Yes sir");
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;
            _direction = direction;
            moveSpeed = force;
        }*/


        private IEnumerator WaitForKick()
        {
            yield return new WaitForSeconds(waitForKick);
            myPush = false;
        }

        private void AnimationDirection()
        {
            if (_direction.z > 0 && velocityZ <= 1)
            {
                velocityZ += Time.deltaTime * animationAcceleration;
            }
            if (_direction.x > 0 && velocityX <= 1)
            {
                velocityX += Time.deltaTime * animationAcceleration;
            }
            if (_direction.z < 0 && velocityZ >= -1)
            {
                velocityZ -= Time.deltaTime * animationAcceleration;
            }
            if (_direction.x < 0 && velocityX >= -1)
            {
                velocityX -= Time.deltaTime * animationAcceleration;
            }
            if (_direction.z == 0 && velocityZ > 0)
            {
                velocityZ -= Time.deltaTime * animationAcceleration;
            }
            if (_direction.x == 0 && velocityX > 0)
            {
                velocityX -= Time.deltaTime * animationAcceleration;
            }
            if (_direction.z == 0 && velocityZ < 0)
            {
                velocityZ += Time.deltaTime * animationAcceleration;
            }
            if (_direction.x == 0 && velocityX < 0)
            {
                velocityX += Time.deltaTime * animationAcceleration;
            }
        }

        // Disabling
        [ClientRpc]
        public void DisableVisibleBodyClientRpc()
        {
            Transform visibleBody = transform.GetChild(0); 
            if (visibleBody != null)
            {
                visibleBody.gameObject.SetActive(false);
            }
        }

        [ClientRpc]
        public void DisableCharacterControllerClientRpc()
        {
            var characterController = GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }
        }

        // Enabling
        [ClientRpc]
        public void EnableVisibleBodyClientRpc()
        {
            Transform visibleBody = transform.GetChild(0); 
            if (visibleBody != null)
            {
                visibleBody.gameObject.SetActive(true);
                DisableCharacterControllerClientRpc();
            }
        }

        [ClientRpc]
        public void EnableCharacterControllerClientRpc()
        {
            var characterController = GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }

        public void DisableCharacter()
        {
            DisableCharacterControllerClientRpc();
            DisableVisibleBodyClientRpc();
        }

        public void EnableCharacter()
        {
            EnableVisibleBodyClientRpc();
            StartCoroutine(EnableCharacterControllerAfterCountdown());
        }

        private IEnumerator EnableCharacterControllerAfterCountdown()
        {
            yield return new WaitForSeconds(countdownTime); 
            EnableCharacterControllerClientRpc();
            //var gameManager = GameManager.Instance;
            //gameManager.StartTimer(gameManager.levelTimes[gameManager.levelCount - 1]);
        }

    }
}
