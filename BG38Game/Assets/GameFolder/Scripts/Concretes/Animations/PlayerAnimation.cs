using System.Collections;
using System.Collections.Generic;
using BG38Game.Abstracts.Controllers;
using UnityEngine;

namespace BG38Game.Animations
{
    public class PlayerAnimation : MonoBehaviour
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
    }
}
