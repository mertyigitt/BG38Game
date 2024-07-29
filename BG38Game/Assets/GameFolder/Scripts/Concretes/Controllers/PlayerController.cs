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
        
        [SerializeField] private Camera cam;
        [SerializeField] private CinemachineVirtualCamera camController;

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
        }

        private void Start()
        {
            if (!IsOwner)
            {
                cam.enabled = false;
                camController.enabled = false;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            _direction = _input.Direction;
            AnimationDirection();

            _xRotator.RotationAction(_input.Rotation.x, turnSpeed);
            _yRotator.RotationAction(_input.Rotation.y,turnSpeed);
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            if (myPush) return;

            _mover.MoveAction(_direction, moveSpeed);

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
        
    }
}
