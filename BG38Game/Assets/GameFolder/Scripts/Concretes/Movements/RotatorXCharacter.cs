using System.Collections;
using System.Collections.Generic;
using BG38Game.Abstracts.Movements;
using BG38Game.Controllers;
using UnityEngine;

namespace BG38Game.Movements
{
    public class RotatorXCharacter : IRotator
    {
        private PlayerController _playerController;

        public RotatorXCharacter(PlayerController playerController)
        {
            _playerController = playerController;
        }
        public void RotationAction(float direction, float speed)
        {
            _playerController.transform.Rotate(Vector3.up * direction * speed * Time.deltaTime);
        }
    }
}
