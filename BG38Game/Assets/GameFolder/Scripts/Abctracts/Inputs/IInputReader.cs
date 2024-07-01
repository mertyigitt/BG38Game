using UnityEngine;

namespace BG38Game.Abstracts.Inputs
{
    public interface IInputReader
    {
        Vector3 Direction { get; }
        Vector2 Rotation { get; }
        bool IsJump { get; }
        bool IsPush { get; }
    }
}
