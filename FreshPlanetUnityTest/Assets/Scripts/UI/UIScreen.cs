using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace FreshPlanet.UI
{
    /// <summary>
    /// Base abstract class for fullscreen UI 'pages' in-game.
    /// Has transitions in and out
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        public event Action<UIScreen> OnTransitionInBegun;
        public event Action<UIScreen> OnTransitionInFinished;
        public event Action<UIScreen> OnTransitionOutBegun;
        public event Action<UIScreen> OnTransitionOutFinished;
        /// <summary>
        /// Called when the Active property of this screen changes value. Handled by the UI Controller.
        /// </summary>
        public event Action<UIScreen> OnActiveStatusChanged;

        [SerializeField]
        protected CanvasGroup canvasGroup;
        
        [SerializeField]
        protected float transitionDuration =  0.25f;
        
        private bool active;
        /// <summary>
        /// Special property for setting UIScreens active or inactive.
        /// Getter just returns whether or not the screen is active.
        /// Setter changes the bool value and triggers the OnActiveStatusChanged event if and only if
        /// the value of the given bool is different than the current value of 'active'. The UIController
        /// will handle this event and display the screen if its priority is high enough.
        /// </summary>
        public bool Active
        {
            get => active;
            set
            {
                bool prev = active;
                active = value;
                
                if (value != prev)
                {
                    OnActiveStatusChanged?.Invoke(this);
                }
            }
        }
        
        private Coroutine transitionInCoroutine;
        private Coroutine transitionOutCoroutine;
        private Tween fadeRoutine;
        
        protected virtual void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            fadeRoutine?.Kill();
        }

        private void StopTransitions()
        {
            if (transitionInCoroutine != null)
            {
                StopCoroutine(transitionInCoroutine);
                transitionInCoroutine = null;
            }

            if (transitionOutCoroutine != null)
            {
                StopCoroutine(transitionOutCoroutine);
                transitionOutCoroutine = null;
            }
        }
        
        /// <summary>
        /// Begins transitioning the UIScreen in to make it active on screen
        /// Ignore a call to transition in if we're already currently transitioning in.
        /// </summary>
        public void BeginTransitionIn()
        {
            if (transitionInCoroutine != null)
            {
                return;
            }
            
            StopTransitions();
            
            if (!gameObject.activeInHierarchy)
            {
                Debug.Log($"{gameObject.name} is not active. Transition in cannot be started");
                return;
            }
            
            transitionInCoroutine = StartCoroutine(CompleteTransitionIn());
        }

        private IEnumerator CompleteTransitionIn()
        {
            OnTransitionInBegun?.Invoke(this);
            yield return TransitionIn();
            transitionInCoroutine = null;
            OnTransitionInFinished?.Invoke(this);
        }

        protected virtual IEnumerator TransitionIn()
        {
            canvasGroup.blocksRaycasts = true;
            yield return FadeScreen(1).WaitForCompletion();
            canvasGroup.interactable = true;
        }
        
        /// <summary>
        /// Begins transitioning the UIScreen out to remove it from the screen.
        /// Ignore a call to transition out if we're already currently transitioning out.
        /// </summary>
        public void BeginTransitionOut()
        {
            if (transitionOutCoroutine != null)
            {
                return;
            }
            
            StopTransitions();
            
            if (!gameObject.activeInHierarchy)
            {
                Debug.Log($"{gameObject.name} is not active. Transition out cannot be started");
                return;
            }
            
            transitionOutCoroutine = StartCoroutine(CompleteTransitionOut());
        }

        private IEnumerator CompleteTransitionOut()
        {
            OnTransitionOutBegun?.Invoke(this);
            yield return TransitionOut();
            transitionOutCoroutine = null;
            OnTransitionOutFinished?.Invoke(this);
        }

        protected virtual IEnumerator TransitionOut()
        {
            canvasGroup.interactable = false;
            yield return FadeScreen(0).WaitForCompletion();
            canvasGroup.blocksRaycasts = false;
        }

        private Tween FadeScreen(float value, Action callback = null)
        {
            fadeRoutine?.Kill();
            fadeRoutine = canvasGroup.DOFade(value, transitionDuration).SetEase(Ease.Linear);

            if (callback != null)
            {
                fadeRoutine.OnComplete(() => callback?.Invoke());
            }

            return fadeRoutine;
        }
    }
}