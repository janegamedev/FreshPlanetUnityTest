using System.Collections;
using System.Collections.Generic;
using FreshPlanet.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.QuizScreen
{
    public class QuizUIScreen : UIScreen
    {
        [SerializeField]
        private TextMeshProUGUI playlistLabel;
        [SerializeField]
        private RawImage playlistIcon;
        [SerializeField]
        private List<QuestionProgressElement> questionProgressElements = new List<QuestionProgressElement>();
        [SerializeField]
        private List<AnswerButtonElement> answerButtonElements = new List<AnswerButtonElement>();

        [SerializeField]
        private AudioSource audioSource;

        private Playlist playlist;
        private int currentQuestionIndex;
        private Question currentQuestion;
        private Coroutine nextQuestionRoutine;
        private Coroutine timeRoutine;
        private QuestionProgressElement currentQuestionProgress;

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
                progressElement.ResetProgress();
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

            if (currentQuestionProgress != null)
            {
                currentQuestionProgress.TransitionOut();
            }

            TerminateRoutine();
            nextQuestionRoutine = StartCoroutine(NextQuestionRoutine());
        }

        private IEnumerator NextQuestionRoutine()
        {
            currentQuestion = playlist.Questions[currentQuestionIndex];
            playlistIcon.texture = currentQuestion.CurrentSong.SongPicture;

            currentQuestionProgress = questionProgressElements[currentQuestionIndex];
            currentQuestionProgress.SetQuestionStarted();
            
            yield return new WaitForSeconds(2);

            for (int i = 0; i < answerButtonElements.Count; i++)
            {
                AnswerButtonElement answerButtonElement = answerButtonElements[i]; 
                Choice choice = currentQuestion.Choices[i];
                
                answerButtonElement.DisplayAnswer(choice.Artist, choice.Title);
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
            float timer = 0;

            while (nextQuestionRoutine != null)
            {
                timer += Time.deltaTime;
                currentQuestionProgress.SetTime(timer);
                yield return null;
            }
        }

        private void CompletePlaylist()
        {
            
        }

        private void HandleAnswerClicked(AnswerButtonElement clickedAnswer)
        {
            TerminateRoutine();
            int clickedIndex = answerButtonElements.IndexOf(clickedAnswer);
            bool clickedCorrect = clickedIndex == currentQuestion.AnswerIndex;
            DisplayQuestionResults(clickedIndex, clickedCorrect);
        }

        private void DisplayQuestionResults(int clickedIndex, bool clickedCorrect)
        {
            audioSource.Stop();
            if (clickedCorrect)
            {
                currentQuestionProgress.SetQuestionCompleted();
            }
            else
            {
                currentQuestionProgress.SetQuestionFailed();
            }
            
            for (int i = 0; i < answerButtonElements.Count; i++)
            {
                AnswerButtonElement answerButtonElement = answerButtonElements[i];

                if (i == clickedIndex)
                {
                    answerButtonElement.FadeColor(clickedCorrect);
                    continue;
                }
                
                if (i == currentQuestion.AnswerIndex)
                {
                    answerButtonElement.FadeColor(true);
                }
                else
                {
                    answerButtonElement.SetInteractable(false);
                }
            }
            
            LoadNextQuestion();
        }
    }
}