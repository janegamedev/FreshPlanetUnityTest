using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FreshPlanet.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.WelcomeScreen
{
    public class WelcomeUIScreen : UIScreen
    {
        [Header("Sections")]
        [SerializeField]
        private RectTransform sectionSelectArrow;
        [SerializeField]
        private RectTransform topPanelRectTransform;
        [SerializeField]
        private float sectionTransitionDuration = 0.5f;
        
        [Serializable]
        public class SectionGroup
        {
            public PlaylistStatus status;
            public TextMeshProUGUI sectionLabel;
            public RectTransform rectTransform;
            public Button sectionButton;
        }

        [SerializeField]
        private List<SectionGroup> sectionGroups = new List<SectionGroup>();

        [Header("Playlists")]
        [SerializeField]
        private RectTransform playlistsContentParent;
        [SerializeField]
        private CanvasGroup playlistContentCanvasGroup;
        [SerializeField]
        private PlaylistPreviewElement playlistPreviewElement;
        
        private readonly List<PlaylistPreviewElement> spawnedPlaylistElements = new List<PlaylistPreviewElement>();
        private readonly Dictionary<PlaylistStatus, SectionGroup> statusSectionTable =
            new Dictionary<PlaylistStatus, SectionGroup>();
        private readonly Dictionary<PlaylistStatus, List<Playlist>> playlistStatusTable =
            new Dictionary<PlaylistStatus, List<Playlist>>();
        private SectionGroup selectedSection;
        private Tween arrowTween;
        private Tween fadeTween;
        private Coroutine sectionChangeRoutine;

        protected override void Awake()
        {
            base.Awake();
            PlaylistPreviewElement.OnPlaylistSelected += HandlePlaylistSelected;
            PlaylistPreloader.OnPlaylistPreloadCompleted += HandlePlaylistPreloadCompleted;
            
            foreach (SectionGroup sectionGroup in sectionGroups)
            {
                if (statusSectionTable.ContainsKey(sectionGroup.status))
                {
                    continue;
                }
                
                statusSectionTable.Add(sectionGroup.status, sectionGroup);
                sectionGroup.sectionButton.onClick.AddListener(() => HandleStatusSectionClicked(sectionGroup.status));
            }
        }

        private void OnDestroy()
        {
            PlaylistPreviewElement.OnPlaylistSelected -= HandlePlaylistSelected;
            PlaylistPreloader.OnPlaylistPreloadCompleted -= HandlePlaylistPreloadCompleted;
            
            foreach (SectionGroup sectionGroup in statusSectionTable.Values)
            {
                sectionGroup.sectionButton.onClick.RemoveListener(() => HandleStatusSectionClicked(sectionGroup.status));
            }

            TerminateRoutine();
        }

        private void TerminateRoutine()
        {
            arrowTween?.Kill();
            fadeTween?.Kill();
            
            if (sectionChangeRoutine != null)
            {
                StopCoroutine(sectionChangeRoutine);
                sectionChangeRoutine = null;
            }
        }

        protected override IEnumerator TransitionIn()
        {
            LoadPlaylists();
            SelectSection(PlaylistStatus.Activated, true);
            yield return base.TransitionIn();
        }

        protected override IEnumerator TransitionOut()
        {
            yield return base.TransitionOut();
            TerminateRoutine();
            selectedSection = null;
        }

        private PlaylistPreviewElement SpawnPlaylistPreviewElement()
        {
            PlaylistPreviewElement element = Instantiate(playlistPreviewElement, playlistsContentParent);
            spawnedPlaylistElements.Add(element);
            LayoutRebuilder.ForceRebuildLayoutImmediate(playlistsContentParent);
            return element;
        }

        private void LoadPlaylists()
        {
            playlistStatusTable.Clear();
           
            foreach (Playlist playlist in PlaylistPreloader.Instance.Playlists)
            {
                PlaylistStatus status = playlist.GetPlaylistStatus();

                if (playlistStatusTable.TryGetValue(status, out List<Playlist> playlists))
                {
                    playlists.Add(playlist);
                }
                else
                {
                    playlistStatusTable.Add(status, new List<Playlist>(){ playlist });
                }
            }
        }

        private void SelectSection(PlaylistStatus status, bool instant = false)
        {
            SectionGroup sectionGroup = statusSectionTable[status];
            if (selectedSection == sectionGroup)
            {
                return;
            }

            if (selectedSection != null)
            {
                bool isSet = (selectedSection.sectionLabel.fontStyle & FontStyles.Bold) != 0;
                if(isSet)
                {
                    selectedSection.sectionLabel.fontStyle ^= FontStyles.Bold;
                }
            }
            
            selectedSection = sectionGroup;
            selectedSection.sectionLabel.fontStyle |= FontStyles.Bold;
            
            TerminateRoutine();
            sectionChangeRoutine = StartCoroutine(SectionChangeRoutine(instant));
        }

        private IEnumerator SectionChangeRoutine(bool instant)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(topPanelRectTransform);

            arrowTween?.Kill();
            arrowTween = sectionSelectArrow.DOMoveX(selectedSection.rectTransform.position.x, instant ? 0 : sectionTransitionDuration )
                .SetEase(Ease.Linear);
            fadeTween?.Kill();
            
            if (!instant)
            {
                playlistContentCanvasGroup.interactable = false;
                fadeTween = playlistContentCanvasGroup.DOFade(0, sectionTransitionDuration / 2)
                    .SetEase(Ease.Linear);
                yield return fadeTween.WaitForCompletion();
            }
            
            DisplayPlaylists();

            fadeTween = playlistContentCanvasGroup.DOFade(1, instant ? 0 : sectionTransitionDuration / 2)
                .SetEase(Ease.Linear);
            yield return fadeTween.WaitForCompletion();
            
            playlistContentCanvasGroup.interactable = true;
        }
        
        private void DisplayPlaylists()
        {
            playlistStatusTable.TryGetValue(selectedSection.status, out List<Playlist> playlists);
            playlists ??= new List<Playlist>();
            
            if (spawnedPlaylistElements.Count > playlists.Count)
            {
                for (int i = spawnedPlaylistElements.Count - 1; i >= playlists.Count; i--)
                {
                    PlaylistPreviewElement element = spawnedPlaylistElements[i];
                    DestroyImmediate(element.gameObject);
                    spawnedPlaylistElements.Remove(element);
                }
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(playlistsContentParent);
            }

            for (int i = 0; i < playlists.Count; i++)
            {
                PlaylistPreviewElement element = spawnedPlaylistElements.Count > i ? spawnedPlaylistElements[i] : SpawnPlaylistPreviewElement();
                element.DisplayPlaylist(playlists[i]);
            }
        }
        
        private void HandlePlaylistSelected(PlaylistPreviewElement previewElement, Playlist playlist)
        {
            PlaylistPreloader.Instance.PreloadPlaylist(playlist);
        }

        private void HandlePlaylistPreloadCompleted(PlaylistPreloader playlistPreloader, Playlist playlist)
        {
            Active = false;
            UIController.Instance.QuizScreen.Active = true;
        }
        
        private void HandleStatusSectionClicked(PlaylistStatus status)
        {
            SelectSection(status);
        }
    }
}