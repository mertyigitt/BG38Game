using System.Collections;
using System.Collections.Generic;
using BG38Game.Abstracts.Movements;
using BG38Game.Controllers;
using UnityEngine;

namespace BG38Game.Movements
{
    public class RotatorYCharacter : IRotator
    {
        private Transform _transform;
        private float _tilt;

        public RotatorYCharacter(PlayerController playerController)
        {
            _transform = playerController.TurnTransform;
        }
        public void RotationAction(float direction, float speed)
        {
            direction *= speed * Time.deltaTime;
            _tilt = Mathf.Clamp(_tilt - direction, -30, 30);
            _transform.localRotation = Quaternion.Euler(_tilt,0f,0f);
        }
    }
}
