using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FreshPlanet.Data;
using FreshPlanet.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.ResultScreen
{
    /// <summary>
    /// Quiz result screen
    /// Displays the user score and for each question if the user guessed right or wrong.
    /// Contains "Next" button that redirects to the welcome screen, where the user can pick another playlist (or the same one if he wants) and play again.
    /// </summary>
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

        /// <summary>
        /// Collects playlist results and updates max scores in the saving system
        /// </summary>
        private void CollectQuizResults()
        {
            int completedQuestions = completedPlaylist.Questions.Count(question => question.Result.AnsweredCorrectly);
            completedPlaylist.SetCompletedQuestionsAmount(completedQuestions);
            completedPerfectly = completedQuestions == completedPlaylist.Questions.Count;
        }
        
        /// <summary>
        /// Displays playlists result
        /// Shows result title based on player's performance and displays questions results
        /// </summary>
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
        
        /// <summary>
        /// Handle next button clicked
        /// Turns off the result screen and redirects to the welcome screen
        /// </summary>
        private void HandleNextButtonClicked()
        {
            Active = false;
            UIController.Instance.WelcomeScreen.Active = true;
        }
    }
}