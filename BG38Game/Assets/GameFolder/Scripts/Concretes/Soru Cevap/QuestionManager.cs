using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace BG38Game
{
    public class QuestionManager : MonoBehaviour
    {
        public GameObject questionPanel;
        public TextMeshProUGUI questionText;
        public Button[] optionButtons;

        //Normalde sorular ve optionlar icin ayri bir class olustucaktim ama bu da simdilik ayni islevi goruyor. Daha sonra ugrasirim onunla.
        
        private List<string> questions = new List<string> { "What is your favorite color?", "Which is better?", "What is behind the curtain?" };
        private List<string[]> options = new List<string[]> {
            new string[] { "Red", "Blue", "Green" },
            new string[] { "Pizza", "Burger", "Pasta" },
            new string[] { "Curtain?", "German Tank", "AK-47" }
        };

        public int currentQuestionIndex;

        void Start()
        {
            ShowRandomQuestion();
        }

        //Random degil de sirayla da sorabiliriz

        void ShowRandomQuestion()
        {
            currentQuestionIndex = Random.Range(0, questions.Count);
            questionText.text = questions[currentQuestionIndex];
            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < options[currentQuestionIndex].Length) 
                {
                    optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[currentQuestionIndex][i];
                    optionButtons[i].gameObject.SetActive(true);
                    int index = i; 
                    optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
                }
                else //eger buton sayisindan daha az secenek koyduksak diye kontrol ediyor
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }
        }

        void OnOptionSelected(int index)
        {
            string choice = options[currentQuestionIndex][index];
            DataManager.Instance.SaveChoice(currentQuestionIndex, choice); //belki oyuncu numaralarini key olarak kullanabiliriz.
            questionPanel.SetActive(false);
            DataManager.Instance.SetCorrectAnswerForPlatform(index);
        }
    }
}
