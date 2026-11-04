using System;
using Configs;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AccountCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI contactText;
        [SerializeField] private Image icon;
        [Space]
        [SerializeField] private Button favouriteButton;
        [SerializeField] private Image favouriteIcon;
        [Space]
        [SerializeField] private Button profileButton;
        [SerializeField] private Image background;

        private UIConfig _uiConfig;
        
        private Action<int> _onFavourite;
        private Action<int> _onProfile;
        private int _accountId;

        private void Awake()
        {
            favouriteButton.onClick.AddListener(OnFavouriteClicked);
            profileButton.onClick.AddListener(OnProfileClicked);
        }

        public void Init(Action<int> onFavourite, Action<int> onProfile, UIConfig uiConfig)
        {
            _onFavourite = onFavourite;
            _onProfile = onProfile;
            _uiConfig = uiConfig;
        }

        public void Set(AccountData account, bool isFavourite, Sprite avatar)
        {
            _accountId = account.id;
            
            nameText.text = account.FullName;
            contactText.text = $"{account.email} | {account.ip_address}";
            icon.sprite = avatar != null ? avatar : _uiConfig.DefaultIcon;
            
            SetFavourite(isFavourite);
        }

        public void SetFavourite(bool value)
        {
            favouriteIcon.sprite = value ? _uiConfig.FavouriteOn : _uiConfig.FavouriteOff;
        }

        public void SetBackground(bool isAlternative)
        {
            background.enabled = isAlternative;
        }

        private void OnFavouriteClicked() => _onFavourite?.Invoke(_accountId);
        private void OnProfileClicked() => _onProfile?.Invoke(_accountId);
    }
}
