using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        private Dictionary<int, string> playerChoices = new Dictionary<int, string>();

        public int correctIndex;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SaveChoice(int questionIndex, string choice)
        {
            if (!playerChoices.ContainsKey(questionIndex))
            {
                playerChoices.Add(questionIndex, choice);
            }
        }

        public string GetChoice(int questionIndex) 
        {
            return playerChoices.ContainsKey(questionIndex) ? playerChoices[questionIndex] : null; 
        }

        public void SetCorrectAnswerForPlatform(int correctIdx)
        {
            correctIndex = correctIdx;
        }
    }
}
