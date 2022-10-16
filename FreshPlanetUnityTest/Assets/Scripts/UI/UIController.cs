using System.Collections.Generic;
using FreshPlanet.Utilities;
using UnityEngine;

namespace FreshPlanet.UI
{
    /// <summary>
    /// Handles UI navigation and user input events.
    /// Is a singleton and can be referenced as UIController.Instance.
    /// </summary>
    public class UIController : Singleton<UIController>
    {
        [SerializeField]
        private UIScreen welcomeScreen;
        [SerializeField]
        private UIScreen quizScreen;
        public UIScreen QuizScreen => quizScreen;

        private UIScreen currentScreen;

        private UIScreen nextScreen;
        private readonly HashSet<UIScreen> registeredScreens = new HashSet<UIScreen>();

        protected override void Awake()
        {
            base.Awake();
            RegisterUIScreens(welcomeScreen, quizScreen);
        }

        private void Start()
        {
            welcomeScreen.Active = true;
        }

        private void OnDestroy()
        {
            foreach (UIScreen registeredScreen in registeredScreens)
            {
                if (registeredScreen != null)
                {
                    UnregisterUIScreen(registeredScreen, onDestroy: true);
                }
            }
            
            registeredScreens.Clear();
        }

        /// <summary>
        /// Helper to clean up initialization code for the UIController.
        /// </summary>
        private void RegisterUIScreens(params UIScreen[] screens)
        {
            foreach (UIScreen uiScreen in screens)
            {
                RegisterUIScreen(uiScreen);
            }
        }
        
        /// <summary>
        /// Registers a UIScreen to this UIController to handle transition events
        /// </summary>
        /// <param name="screen">The UIScreen to register</param>
        private void RegisterUIScreen(UIScreen screen)
        {
            if (screen == null)
            {
                return;
            }

            if (registeredScreens.Contains(screen))
            {
                Debug.LogError($"Tried to register an already-registered UIScreen: {screen.name}");
                return;
            }
            
            registeredScreens.Add(screen);
            screen.OnTransitionInFinished += HandleScreenTransitionInFinished;
            screen.OnTransitionOutFinished += HandleScreenTransitionOutFinished;
            screen.OnActiveStatusChanged += HandleScreenActiveStatusChanged;
        }

        /// <summary>
        /// Unregisters a UIScreen to this UIController to stop handling transition events
        /// </summary>
        /// <param name="screen">The UIScreen to unregister</param>
        /// <param name="onDestroy">If true, we're calling this from OnDestroy</param>
        private void UnregisterUIScreen(UIScreen screen, bool onDestroy = false)
        {
            if (screen == null)
            {
                return;
            }

            if (!onDestroy)
            {
                registeredScreens.Remove(screen);
            }
            
            screen.OnTransitionInFinished -= HandleScreenTransitionInFinished;
            screen.OnTransitionOutFinished -= HandleScreenTransitionOutFinished;
            screen.OnActiveStatusChanged -= HandleScreenActiveStatusChanged;
        }

        private void HandleScreenTransitionInFinished(UIScreen screen)
        {
            // Screen was transitioned in without using the UIController - handle this case in case
            if (screen != currentScreen)
            {
                currentScreen.BeginTransitionOut();
                currentScreen = screen;
            }
        }

        private void HandleScreenTransitionOutFinished(UIScreen screen)
        {
            // Current screen has transitioned out
            // If there is a nextScreen, transition that in and assign as the currentScreen
            if (screen == currentScreen)
            {
                currentScreen = nextScreen;
                if (currentScreen != null)
                {
                    screen.BeginTransitionIn();
                }
            }
        }
        
        /// <summary>
        /// Handler for a change in a UIScreen's Active property.
        /// DO NOT change (set a new value for) the UIScreen's Active property again in here,
        /// or you will trigger the event for this handler again and create an infinite recursion!
        /// </summary>
        private void HandleScreenActiveStatusChanged(UIScreen uiScreen)
        {
            if (uiScreen.Active)
            {
                if (currentScreen != null)
                {
                    nextScreen = uiScreen;
                    currentScreen.BeginTransitionOut();
                }
                else
                {
                    nextScreen = null;
                    currentScreen = uiScreen;
                    currentScreen.BeginTransitionIn();
                }
            }
            else
            {
                if (currentScreen == uiScreen)
                {
                    currentScreen.BeginTransitionOut();
                    nextScreen = null;
                    currentScreen = null;
                }
            }
        }
    }
}