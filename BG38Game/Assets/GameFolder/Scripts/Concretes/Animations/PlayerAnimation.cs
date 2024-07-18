using System.Collections;
using System.Collections.Generic;
using BG38Game.Abstracts.Controllers;
using UnityEngine;

namespace BG38Game.Animations
{
    public class PlayerAnimation
    {
        private Animator _animator;

        public PlayerAnimation(IEntityController entityController)
        {
            _animator = entityController.transform.GetComponentInChildren<Animator>();
        }

        //1-Animation fonksiyonları burada tanımlanır.
        public void JumpAnimation(bool canJump)
        {
            _animator.SetBool("isJumped", canJump);
        }
        
        public void Grounded(bool isGrounded)
        {
            _animator.SetBool("isGrounded", isGrounded);
        }

        public void KickAnimation(bool isKicked)
        {
            _animator.SetBool("isKicked", isKicked);
        }

        public void BasicMoveAnimation(float velocityX, float velocityZ)
        {
            //0.7071

            _animator.SetFloat("X Velocity", velocityX);
            _animator.SetFloat("Z Velocity", velocityZ);
        }
    }
}
