using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.QuizScreen
{
    /// <summary>
    /// Question progress element
    /// Displays the status of the question (not started, in progress, completed)
    /// Not started -> shows non animated icon of not started question
    /// In progress -> shows animated spinning icon along with a timer circular fill bar based on the time left
    /// Completed -> shows the question result (correct or wrong) and the time passed
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class QuestionProgressElement : MonoBehaviour
    {
        private const string QUESTION_TIME = "{0}s";
        
        private readonly static int IsStarted = Animator.StringToHash("IsStarted");
        private readonly static int IsCompleted = Animator.StringToHash("IsCompleted");
        private readonly static int Reset = Animator.StringToHash("Reset");

        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private TextMeshProUGUI questionTimer;
        [SerializeField]
        private Animator animator;
        [SerializeField] 
        private Image circularFillBar;

        [SerializeField]
        private Vector2 defaultSize = new Vector2(100, 100);
        [SerializeField]
        private Vector2 inProgressSize = new Vector2(120, 120);
        [SerializeField]
        private float scaleDuration = 0.5f;
        
        private Tween sizeDeltaTween;

        private void OnDestroy()
        {
            sizeDeltaTween?.Kill();
        }

        public void ResetToIdle()
        {
            ScaleDown(true);
            animator.SetBool(IsStarted, false);
            animator.SetBool(IsCompleted, false);
            animator.SetTrigger(Reset);
            SetProgressPct(1);
        }

        public void SetQuestionStarted()
        {
            animator.SetBool(IsStarted, true);
            DoSizeDelta(inProgressSize);
        }

        public void SetQuestionCompleted(float time, Color textColor)
        {
            questionTimer.text = string.Format(QUESTION_TIME, time.ToString("0.0"));
            questionTimer.color = textColor;
            animator.SetBool(IsCompleted, true);
        }

        /// <summary>
        /// Change the current fill & of this progress bar
        /// </summary>
        /// <param name="pct">The desired fill percentage - 0...1 expected</param>
        public void SetProgressPct(float pct)
        {
            circularFillBar.fillAmount = pct;
        }

        public void ScaleDown(bool instant = false)
        {
            DoSizeDelta(defaultSize, instant);
        }

        private void DoSizeDelta(Vector2 size, bool instant = false)
        {
            sizeDeltaTween?.Kill();
            sizeDeltaTween = rectTransform.DOSizeDelta(size, instant ? 0 : scaleDuration).SetEase(Ease.Linear);
        }
    }
}