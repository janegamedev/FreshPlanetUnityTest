using System;
using System.Collections.Generic;
using FreshPlanet.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FreshPlanet.UI.WelcomeScreen
{
    /// <summary>
    /// Playlist preview element that displayed in Welcome screen
    /// Shows playlist's label and best progress (or if it's been mastered)
    /// Upon click invokes event OnPlaylistSelected
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PlaylistPreviewElement : MonoBehaviour
    {
        public static event Action<PlaylistPreviewElement, Playlist> OnPlaylistSelected;
        
        [SerializeField]
        private Button playlistButton;
        [SerializeField]
        private TextMeshProUGUI playlistLabel;
        [SerializeField]
        private CanvasGroup progressCanvasGroup;
        [SerializeField]
        private List<Image> progressImages;
        [SerializeField]
        private Sprite inProgressIcon;
        [SerializeField]
        private Sprite completedIcon;
        [SerializeField]
        private CanvasGroup masteredCanvasGroup;

        private Playlist playlistData;

        private void Awake()
        {
            playlistButton.onClick.AddListener(HandlePlaylistButtonClicked);
        }

        private void OnDestroy()
        {
            playlistButton.onClick.RemoveListener(HandlePlaylistButtonClicked);
        }

        public void DisplayPlaylist(Playlist playlist)
        {
            playlistData = playlist;
            playlistLabel.text = playlistData.PlaylistTitle;
            bool mastered = playlistData.GetPlaylistStatus() is PlaylistStatus.Mastered;

            progressCanvasGroup.alpha = mastered ? 0 : 1;
            masteredCanvasGroup.alpha = mastered ? 1 : 0;

            if (!mastered)
            {
                int completedAmount = playlistData.GetMaxCompletedQuestions();
                
                for (int i = 0; i < progressImages.Count; i++)
                {
                    progressImages[i].sprite = i < completedAmount? completedIcon : inProgressIcon;
                }
            }
        }

        private void HandlePlaylistButtonClicked()
        {
            OnPlaylistSelected?.Invoke(this, playlistData);
        }
    }
}