using DG.Tweening;
using FreshPlanet.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.ResultScreen
{
    /// <summary>
    /// Question result element that displayed in Result screen
    /// Shows question's result including answer time and correctness, song cover, title and the artist
    /// </summary>
    public class QuestionResultElement : MonoBehaviour
    {
        private const string QUESTION_TIME = "{0}s";

        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private RawImage songCover;
        [SerializeField]
        private TextMeshProUGUI songTitle;
        [SerializeField]
        private TextMeshProUGUI artist;
        [SerializeField]
        private Image resultIcon;
        [SerializeField]
        private TextMeshProUGUI answerTime;
        [SerializeField]
        private float fadeDuration = 0.5f;
        
        [Header("Icons")]
        [SerializeField]
        private Sprite correctIcon;
        [SerializeField]
        private Color correctColor;
        [SerializeField]
        private Sprite wrongIcon;
        [SerializeField]
        private Color wrongColor;

        private Tween fadeTween;

        private void OnDestroy()
        {
            fadeTween?.Kill();
        }

        public void DisplayQuestionResult(Question question)
        {
            songCover.texture = question.CurrentSong.SongPicture;
            songTitle.text = question.CurrentSong.Title;
            artist.text = question.CurrentSong.Artist;
            resultIcon.sprite = question.Result.AnsweredCorrectly ? correctIcon : wrongIcon;
            answerTime.text = string.Format(QUESTION_TIME, question.Result.AnswerTime.ToString("0.0"));
            answerTime.color = question.Result.AnsweredCorrectly ? correctColor : wrongColor;
        }

        public Tween Fade(float value, bool instant = false)
        {
            fadeTween?.Kill();
            fadeTween = canvasGroup.DOFade(value, instant ? 0 : fadeDuration).SetEase(Ease.Linear);
            return fadeTween;
        }
    }
}