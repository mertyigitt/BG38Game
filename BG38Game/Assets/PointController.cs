using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BG38Game
{
    public class PointController : NetworkBehaviour
    {
        public NetworkVariable<int> currentPoints = new NetworkVariable<int>(0);

        [ServerRpc(RequireOwnership = false)]
        public void AddPointsServerRpc(int points)
        {
            if (!IsServer) return;

            currentPoints.Value += points;
            Debug.Log($"{gameObject.name} has {currentPoints.Value} points.");
        }
    }
    // public class PointController : NetworkBehaviour
    // {
    //     public NetworkVariable<int> currentPoints = new NetworkVariable<int>(0);
    //
    //     public void AddPoints(int points)
    //     {
    //         currentPoints.Value += points;
    //         Debug.Log($"{gameObject.name} has {currentPoints} points.");
    //     }
    // }
}
