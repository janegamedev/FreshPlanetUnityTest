using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.QuizScreen
{
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
        private Color correctColor;
        [SerializeField]
        private Color wrongColor;
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
        }
        
        public void SetInteractable(bool interactable)
        {
            answerButton.interactable = interactable;
        }
        
        private void HandleAnswerButtonClicked()
        {
            OnAnswerClicked?.Invoke(this);
        }

        public void FadeColor(bool isCorrect)
        {
            colorFade?.Kill();
            Color fadeColor = isCorrect ? correctColor : wrongColor;
            colorFade = DOTween.Sequence()
                .Append(songTitle.DOColor(fadeColor, colorFadeDuration).SetEase(Ease.Linear))
                .Join(songTitle.DOColor(fadeColor, colorFadeDuration).SetEase(Ease.Linear));
        }
    }
}