using System.Collections;
using System.Collections.Generic;
using BG38Game.Abstracts.Controllers;
using BG38Game.Abstracts.Movements;
using UnityEngine;

namespace BG38Game.Movements
{
    public class MoveWithCharacterController : IMover
    {
        private CharacterController _characterController;

        public MoveWithCharacterController(IEntityController entityController)
        {
            _characterController = entityController.transform.GetComponent<CharacterController>();
        }
        public void MoveAction(Vector3 direction, float moveSpeed)
        {
            if(direction.magnitude == 0f) return;
            
            Vector3 worldPosition = _characterController.transform.TransformDirection(direction);
            Vector3 movement = worldPosition * moveSpeed * Time.deltaTime;
            
            _characterController.Move(movement);
        }
    }
}
