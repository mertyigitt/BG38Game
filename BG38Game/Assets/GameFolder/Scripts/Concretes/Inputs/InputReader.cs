using BG38Game.Abstracts.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BG38Game.Inputs
{
    public class InputReader : MonoBehaviour, IInputReader  
    {
        public Vector3 Direction { get; set; }
        public Vector2 Rotation { get; set; }
        public bool IsJump { get; set; }
        
        public bool IsPush { get; set; }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 oldDirection = context.ReadValue<Vector2>();
            Direction = new Vector3(oldDirection.x, 0f, oldDirection.y);
        }
        
        public void OnRotate(InputAction.CallbackContext context)
        {
            Rotation = context.ReadValue<Vector2>();
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            IsJump = context.ReadValueAsButton();
        }
        
        public void OnPush(InputAction.CallbackContext context)
        {
            IsPush = context.ReadValueAsButton();
        }
    }
}
