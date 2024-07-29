using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class PointController : MonoBehaviour
    {
        public int currentPoints = 0;

        public void AddPoints(int points)
        {
            currentPoints += points;
            Debug.Log($"{gameObject.name} has {currentPoints} points.");
        }
    }
}
