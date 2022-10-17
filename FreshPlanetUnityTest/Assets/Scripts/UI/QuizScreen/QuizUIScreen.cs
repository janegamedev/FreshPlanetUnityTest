using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FreshPlanet.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.QuizScreen
{
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
        private RectTransform questionProgressParent;
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

        private void TerminateRoutine()
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
            TerminateRoutine();
            return base.TransitionOut();
        }

        private void LoadNextQuestion()
        {
            currentQuestionIndex++;

            if (currentQuestionIndex >= playlist.Questions.Count)
            {
                CompletePlaylist();
                return;
            }
            
            TerminateRoutine();
            nextQuestionRoutine = StartCoroutine(NextQuestionRoutine());
        }

        private IEnumerator NextQuestionRoutine()
        {
            yield return new WaitForSeconds(2f);
            
            if (currentQuestionProgress != null)
            {
                currentQuestionProgress.ScaleDown();
            }

            coverAnimator.SetBool(Idle, true);
            
            Sequence fadeSequence = DOTween.Sequence();
            foreach (AnswerButtonElement buttonElement in answerButtonElements)
            {
                fadeSequence.Join(buttonElement.FadeColorAlpha(0));
            }

            yield return fadeSequence.WaitForCompletion();

            currentQuestion = playlist.Questions[currentQuestionIndex];
            currentQuestion.SetResult(null);
            playlistIcon.texture = currentQuestion.CurrentSong.SongPicture;

            currentQuestionProgress = questionProgressElements[currentQuestionIndex];
            currentQuestionProgress.SetQuestionStarted();

            for (int i = 0; i < answerButtonElements.Count; i++)
            {
                AnswerButtonElement answerButtonElement = answerButtonElements[i]; 
                Choice choice = currentQuestion.Choices[i];
                
                answerButtonElement.DisplayAnswer(choice.Artist, choice.Title);
                answerButtonElement.FadeColorAlpha(1);
            }
            
            audioSource.clip = currentQuestion.CurrentSong.SongSample;
            audioSource.Play();

            yield return null;

            timeRoutine = StartCoroutine(TimerRoutine());
            
            yield return new WaitUntil(() => !audioSource.isPlaying);

            DisplayQuestionResults(-1, false);
        }

        private IEnumerator TimerRoutine()
        {
            float totalTime = audioSource.clip.length;
            answerTime = 0;

            while (nextQuestionRoutine != null)
            {
                answerTime += Time.deltaTime;
                float percentageLeft = 1 - answerTime / totalTime;
                currentQuestionProgress.SetProgressPct(percentageLeft);
                yield return null;
            }
        }
        
        private void HandleAnswerClicked(AnswerButtonElement clickedAnswer)
        {
            if (currentQuestion.Result != null)
            {
                return;
            }
            
            TerminateRoutine();
            int clickedIndex = answerButtonElements.IndexOf(clickedAnswer);
            bool clickedCorrect = clickedIndex == currentQuestion.AnswerIndex;
            DisplayQuestionResults(clickedIndex, clickedCorrect);
        }

        private void DisplayQuestionResults(int clickedIndex, bool clickedCorrect)
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

            LoadNextQuestion();
        }
        
        private void CompletePlaylist()
        {
            
        }
    }
}