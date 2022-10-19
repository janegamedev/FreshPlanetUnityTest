using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FreshPlanet.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.ResultScreen
{
    public class ResultUIScreen : UIScreen
    {
        private const string PLAYER_WON_TITLE = "You won!";
        private const string PLAYER_FAILED_TITLE = "Try again!";
        
        [SerializeField]
        private TextMeshProUGUI resultTitle;
        [SerializeField]
        private Button nextButton;
        [SerializeField]
        private List<QuestionResultElement> resultElements = new List<QuestionResultElement>();

        private Playlist completedPlaylist;
        private bool completedPerfectly;

        protected override void Awake()
        {
            base.Awake();
            nextButton.onClick.AddListener(HandleNextButtonClicked);
        }

        private void OnDestroy()
        {
            nextButton.onClick.RemoveListener(HandleNextButtonClicked);
        }

        protected override IEnumerator TransitionIn()
        {
            foreach (QuestionResultElement resultElement in resultElements)
            {
                resultElement.Fade(0, true);
            }
            
            completedPlaylist = PlaylistPreloader.Instance.PreloadedPlaylist;
            CollectQuizResults();
            DisplayResults();
            
            yield return base.TransitionIn();
            
            foreach (QuestionResultElement resultElement in resultElements)
            {
                yield return resultElement.Fade(1).WaitForCompletion();
            }
        }

        private void CollectQuizResults()
        {
            int completedQuestions = 0;
            
            foreach (Question question in completedPlaylist.Questions)
            {
                Result result = question.Result;
                if (result.AnsweredCorrectly)
                {
                    completedQuestions++;
                }
            }
            
            completedPlaylist.SetCompletedQuestionsAmount(completedQuestions);
            completedPerfectly = completedQuestions == completedPlaylist.Questions.Count;
        }
        
        private void DisplayResults()
        {
            resultTitle.text = completedPerfectly ? PLAYER_WON_TITLE : PLAYER_FAILED_TITLE;

            for (int i = 0; i < resultElements.Count; i++)
            {
                QuestionResultElement resultElement = resultElements[i];
                Question question = completedPlaylist.Questions[i];
                resultElement.DisplayQuestionResult(question);
            }
        }
        
        private void HandleNextButtonClicked()
        {
            Active = false;
            UIController.Instance.WelcomeScreen.Active = true;
        }
    }
}