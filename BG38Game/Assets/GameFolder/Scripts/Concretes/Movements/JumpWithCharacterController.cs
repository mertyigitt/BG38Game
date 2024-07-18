using System.Collections;
using System.Collections.Generic;
using BG38Game.Abstracts.Controllers;
using BG38Game.Abstracts.Movements;
using UnityEngine;

namespace BG38Game.Movements
{
    public class JumpWithCharacterController : IJumper
    {
        private CharacterController _characterController;
        private Gravity _gravity;
        


        public JumpWithCharacterController(IEntityController entityController)
        {
            _characterController = entityController.transform.GetComponent<CharacterController>();
            _gravity = entityController.transform.GetComponent<Gravity>();
        }
        public void JumpAction(float jumpSpeed)
        {
            if(!_gravity.IsGroundedValue) return;
            if (_gravity.IsGroundedValue)
            {
                Vector3 velocity = _gravity.Velocity;
                velocity.y = Mathf.Sqrt(jumpSpeed * -2f * _gravity.GravityValue);
                _gravity.Velocity = velocity;
            }
        }
    }
}
