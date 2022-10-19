using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FreshPlanet.Data;
using FreshPlanet.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.QuizScreen
{
    /// <summary>
    /// Quiz screen
    /// Displays series of questions with 4 possible choice buttons
    /// Contains additional logic for timers and playlist progress along the way
    /// </summary>
    public class QuizUIScreen : UIScreen
    {
        private readonly static int Idle = Animator.StringToHash("Idle");
        
        [SerializeField]
        private TextMeshProUGUI playlistLabel;
        
        [Header("Cover")]
        [SerializeField]
        private RawImage playlistIcon;
        [SerializeField]
        private Animator coverAnimator;

        [Header("Answers")]
        [SerializeField]
        private List<QuestionProgressElement> questionProgressElements = new List<QuestionProgressElement>();
        [SerializeField]
        private List<AnswerButtonElement> answerButtonElements = new List<AnswerButtonElement>();

        [Header("Colors")]
        [SerializeField]
        private Color correctColor;
        [SerializeField]
        private Color wrongColor;
        
        [SerializeField]
        private AudioSource audioSource;

        private Playlist playlist;
        private int currentQuestionIndex;
        private Question currentQuestion;
        private Coroutine nextQuestionRoutine;
        private Coroutine resultRoutine;
        private Coroutine timeRoutine;
        private QuestionProgressElement currentQuestionProgress;
        private float answerTime;
        private bool currentAnsweredCorrectly;

        protected override void Awake()
        {
            base.Awake();
            AnswerButtonElement.OnAnswerClicked += HandleAnswerClicked;
        }

        private void OnDestroy()
        {
            AnswerButtonElement.OnAnswerClicked -= HandleAnswerClicked;
        }

        private void TerminateRoutines()
        {
            if (nextQuestionRoutine != null)
            {
                StopCoroutine(nextQuestionRoutine);
                nextQuestionRoutine = null;
            }

            if (timeRoutine != null)
            {
                StopCoroutine(timeRoutine);
                timeRoutine = null;
            }

            if (resultRoutine != null)
            {
                StopCoroutine(resultRoutine);
                resultRoutine = null;
            }
        }

        protected override IEnumerator TransitionIn()
        {
            currentQuestionIndex = -1;
            playlist = PlaylistPreloader.Instance.PreloadedPlaylist;
            playlistLabel.text = playlist.PlaylistTitle;

            foreach (QuestionProgressElement progressElement in questionProgressElements)
            {
                progressElement.ResetToIdle();
            }

            LoadNextQuestion();
            return base.TransitionIn();
        }

        protected override IEnumerator TransitionOut()
        {
            TerminateRoutines();
            yield return base.TransitionOut();
        }
        
        /// <summary>
        /// Loads a next question if such exists
        /// Otherwise, completes a playlist
        /// </summary>
        private void LoadNextQuestion()
        {
            currentQuestionIndex++;

            if (currentQuestionIndex >= playlist.Questions.Count)
            {
                CompletePlaylist();
                return;
            }
            
            TerminateRoutines();
            nextQuestionRoutine = StartCoroutine(NextQuestionRoutine());
        }

        private IEnumerator NextQuestionRoutine()
        {
            yield return new WaitForSeconds(1f);
            
            // Scale down previous question progress to the regular size
            if (currentQuestionProgress != null)
            {
                currentQuestionProgress.ScaleDown();
            }

            // Broadcast cover animator to idle
            // Hides the song cover and displays unknown song picture
            coverAnimator.SetBool(Idle, true);
            
            // Fade out button element's previous answers
            Sequence fadeSequence = DOTween.Sequence();
            foreach (AnswerButtonElement buttonElement in answerButtonElements)
            {
                fadeSequence.Join(buttonElement.FadeColorAlpha(0));
            }

            yield return fadeSequence.WaitForCompletion();

            // Assigns a new question
            currentQuestion = playlist.Questions[currentQuestionIndex];
            currentQuestion.SetResult(null);
            // Loads current song's cover
            playlistIcon.texture = currentQuestion.CurrentSong.SongPicture;

            // Gets new question progress element and broadcast question start
            currentQuestionProgress = questionProgressElements[currentQuestionIndex];
            currentQuestionProgress.SetQuestionStarted();

            // Displays answers on each answer button element
            // DisplayAnswer will automatically fade in button element text
            for (int i = 0; i < answerButtonElements.Count; i++)
            {
                AnswerButtonElement answerButtonElement = answerButtonElements[i]; 
                Choice choice = currentQuestion.Choices[i];
                answerButtonElement.DisplayAnswer(choice.Artist, choice.Title);
            }
            
            // Sets current song's sample and plays it
            audioSource.clip = currentQuestion.CurrentSong.SongSample;
            audioSource.Play();

            yield return null;

            // Starts the timer routine
            timeRoutine = StartCoroutine(TimerRoutine());
        }

        private IEnumerator TimerRoutine()
        {
            float totalTime = audioSource.clip.length;
            answerTime = 0;
            float percentageLeft = 1;

            while (percentageLeft > 0)
            {
                answerTime += Time.deltaTime;
                percentageLeft = 1 - answerTime / totalTime;
                currentQuestionProgress.SetProgressPct(percentageLeft);
                yield return null;
            }
            
            // When time is out, count it as a wrong answer and display correct one
            DisplayQuestionResults(-1, false);
        }
        
        /// <summary>
        /// Handles answer button element clicked
        /// Checks what index was clicked and if that was a correct answer
        /// Invokes DisplayQuestionResults
        /// </summary>
        /// <param name="clickedAnswer">Answer button element that was clicked</param>
        private void HandleAnswerClicked(AnswerButtonElement clickedAnswer)
        {
            if (currentQuestion.Result != null)
            {
                return;
            }
            
            TerminateRoutines();
            int clickedIndex = answerButtonElements.IndexOf(clickedAnswer);
            bool clickedCorrect = clickedIndex == currentQuestion.AnswerIndex;
            DisplayQuestionResults(clickedIndex, clickedCorrect);
        }

        /// <summary>
        /// Displays question results by starting QuestionResultsRoutine
        /// </summary>
        /// <param name="clickedIndex">Clicked answer index</param>
        /// <param name="clickedCorrect">Did player click the correct answer?</param>
        private void DisplayQuestionResults(int clickedIndex, bool clickedCorrect)
        {
            TerminateRoutines();
            resultRoutine = StartCoroutine(QuestionResultsRoutine(clickedIndex, clickedCorrect));
        }

        /// <summary>
        /// Displays question results
        /// Shows song cover and the correct answer, updates question progress
        /// Waits for a few seconds before loading a next question
        /// </summary>
        /// <param name="clickedIndex">Clicked answer index</param>
        /// <param name="clickedCorrect">Did player click the correct answer?</param>
        private IEnumerator QuestionResultsRoutine(int clickedIndex, bool clickedCorrect)
        {
            Result result = new Result(clickedCorrect, answerTime);
            currentQuestion.SetResult(result);

            currentAnsweredCorrectly = clickedCorrect;
            audioSource.Stop();
            coverAnimator.SetBool(Idle, false);
            
            Color resultColor = currentAnsweredCorrectly ? correctColor : wrongColor;
            currentQuestionProgress.SetQuestionCompleted(answerTime, resultColor);

            for (int i = 0; i < answerButtonElements.Count; i++)
            {
                AnswerButtonElement answerButtonElement = answerButtonElements[i];

                if (i == clickedIndex)
                {
                  
                    answerButtonElement.FadeColor(resultColor);
                    continue;
                }
                
                if (i == currentQuestion.AnswerIndex)
                {
                    answerButtonElement.FadeColor(correctColor);
                }
                else
                {
                    answerButtonElement.SetInteractable(false);
                }
            }
            
            yield return new WaitForSeconds(1f);
            
            LoadNextQuestion();
        }
        
        /// <summary>
        /// Complete playlist quiz by redirecting the player to the result screen
        /// Disables Quiz screen and actives Result screen
        /// </summary>
        private void CompletePlaylist()
        {
            Active = false;
            UIController.Instance.ResultScreen.Active = true;
        }
    }
}