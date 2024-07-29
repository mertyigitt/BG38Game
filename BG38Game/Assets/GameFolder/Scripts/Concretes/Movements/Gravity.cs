using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BG38Game.Movements
{
    [RequireComponent(typeof(CharacterController))]
    public class Gravity : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] float _gravity = -9.81f;

        #endregion

        #region Private Variables

        private CharacterController _characterController;
        private Vector3 _velocity;
        private bool _isGrounded;

        #endregion

        #region Public Variables

        public Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
        public float GravityValue => _gravity;
        public bool IsGroundedValue => _isGrounded;

        public float GroundedOffset = -0.14f;
        public LayerMask GroundLayers;

        #endregion

        #endregion

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            _isGrounded = IsGrounded();
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            else
            {
                _velocity.y += _gravity * Time.deltaTime;
            }
            

            _characterController.Move(_velocity * Time.deltaTime);
        }
        
        private bool IsGrounded()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);

            if(Physics.CheckSphere(spherePosition, _characterController.radius, GroundLayers, QueryTriggerInteraction.Ignore)){
                return true;
            }

            return false;



            /*RaycastHit hit;
            float rayLength = _characterController.height * 0.001f + _characterController.skinWidth; 
            
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
            {
                return true;
            }

            return false;*/
        }
    }
}
