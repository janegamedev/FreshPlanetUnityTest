using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.QuizScreen
{
    /// <summary>
    /// Answer button element
    /// Represent a button with an answer to the current question
    /// Can change the text color and fade in/out this text
    /// Upon click invokes OnAnswerClicked event
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class AnswerButtonElement : MonoBehaviour
    {
        public static event Action<AnswerButtonElement> OnAnswerClicked;
        
        [SerializeField]
        private Button answerButton;
        [SerializeField]
        private TextMeshProUGUI artistLabel;
        [SerializeField]
        private TextMeshProUGUI songTitle;
        
        [Space]
        [SerializeField]
        private Color defaultColor;
        [SerializeField]
        private float colorFadeDuration = 0.25f;

        private Tween colorFade;

        private void Awake()
        {
            answerButton.onClick.AddListener(HandleAnswerButtonClicked);
        }

        private void OnDestroy()
        {
            colorFade?.Kill();
        }
        
        public void DisplayAnswer(string artist, string title)
        {
            colorFade?.Kill();
            artistLabel.text = artist;
            songTitle.text = title;
            artistLabel.color = defaultColor;
            songTitle.color = defaultColor;
            SetInteractable(true);
            FadeColorAlpha(1);
        }
        
        public void SetInteractable(bool interactable)
        {
            answerButton.interactable = interactable;
        }
        
        private void HandleAnswerButtonClicked()
        {
            OnAnswerClicked?.Invoke(this);
        }

        public void FadeColor(Color fadeColor)
        {
            colorFade?.Kill();
            colorFade = DOTween.Sequence()
                .Append(artistLabel.DOColor(fadeColor, colorFadeDuration).SetEase(Ease.Linear))
                .Join(songTitle.DOColor(fadeColor, colorFadeDuration).SetEase(Ease.Linear));
        }

        public Tween FadeColorAlpha(float value)
        {
            colorFade?.Kill();
            colorFade = DOTween.Sequence()
                .Append(artistLabel.DOFade(value, colorFadeDuration).SetEase(Ease.Linear))
                .Join(songTitle.DOFade(value, colorFadeDuration).SetEase(Ease.Linear));
            return colorFade;
        }
    }
}