using UnityEngine;

namespace BG38Game.Abstracts.Movements
{
    public interface IMover
    {
        void MoveAction(Vector3 direction, float moveSpeed);
    }
}
