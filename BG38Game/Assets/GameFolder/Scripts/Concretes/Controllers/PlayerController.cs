using BG38Game.Abstracts.Controllers;
using BG38Game.Abstracts.Inputs;
using BG38Game.Abstracts.Movements;
using BG38Game.Movements;
using UnityEngine;

namespace BG38Game.Controllers
{
    public class PlayerController : MonoBehaviour, IEntityController
    {
        #region Self Variables

        #region Serialized Variables

        [Header("Movement Informations")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float turnSpeed = 10f;
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private Transform turnTransform;

        #endregion

        #region Private Variables

        private IInputReader _input;
        private IMover _mover;
        private IRotator _xRotator;
        private IRotator _yRotator;
        private IJumper _jumper;
        private Vector3 _direction;
        private Vector2 _rotation;

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
        }

        private void Update()
        {
            _direction = _input.Direction;
            _xRotator.RotationAction(_input.Rotation.x, turnSpeed);
            _yRotator.RotationAction(_input.Rotation.y,turnSpeed);
        }

        private void FixedUpdate()
        {
            _mover.MoveAction(_direction , moveSpeed);
            if (_input.IsJump)
            {
                _jumper.JumpAction(jumpSpeed);
            }
        }
    }
}
