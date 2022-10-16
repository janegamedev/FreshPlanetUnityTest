using TMPro;
using UnityEngine;

namespace FreshPlanet.UI.QuizScreen
{
    [RequireComponent(typeof(Animator))]
    public class QuestionProgressElement : MonoBehaviour
    {
        private const string QUESTION_TIME = "{0}s";
        
        private readonly static int IsStarted = Animator.StringToHash("IsStarted");
        private readonly static int IsCompleted = Animator.StringToHash("IsCompleted");
        private readonly static int IsFailed = Animator.StringToHash("IsFailed");
        private readonly static int TransitionedOut = Animator.StringToHash("TransitionedOut");
        private readonly static int Reset = Animator.StringToHash("Reset");
        
        [SerializeField]
        private TextMeshProUGUI questionTimer;
        [SerializeField]
        private Animator animator;
        
        public void ResetProgress()
        {
            SetTime(0f);
            animator.SetBool(IsCompleted, false);
            animator.SetBool(IsFailed, false);
            animator.SetBool(IsStarted, false);
            animator.SetBool(TransitionedOut, false);
            animator.SetTrigger(Reset);
        }

        public void SetQuestionStarted()
        {
            animator.SetBool(IsStarted, true);
        }
        
        public void SetQuestionCompleted()
        {
            animator.SetBool(IsCompleted, true);
        }
        
        public void SetQuestionFailed()
        {
            animator.SetBool(IsFailed, true);
        }

        public void TransitionOut()
        {
            animator.SetBool(TransitionedOut, true);
        }

        public void SetTime(float sec)
        {
            questionTimer.text = string.Format(QUESTION_TIME, sec.ToString("0.0"));
        }
    }
}