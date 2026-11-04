using System;
using Configs;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProfileWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI genderText;
        [SerializeField] private TextMeshProUGUI emailText;
        [SerializeField] private TextMeshProUGUI ipAddressText;
        [SerializeField] private Image icon;
        [Space]
        [SerializeField] private Button favouriteButton;
        [SerializeField] private Image favouriteIcon;
        [Space]
        [SerializeField] private Button backButton;

        private UIConfig _uiConfig;
        private AccountService _accountService;
        private PicsService _picsService;
        
        private Action _onBack;
        private int _accountId;

        private void Awake()
        {
            favouriteButton.onClick.AddListener(OnFavouriteClicked);
            backButton.onClick.AddListener(OnBackClicked);
        }

        public void Init(AccountService accountService, PicsService picsService, Action onBack, UIConfig uiConfig)
        {
            _accountService = accountService;
            _picsService = picsService;
            _uiConfig = uiConfig;
            
            _onBack = onBack;
            
            _accountService.FavouriteChanged += OnFavouriteChanged;
        }

        public void Set(int accountId)
        {
            _accountId = accountId;
            
            var account = _accountService.GetAccount(_accountId);
            nameText.text = account.Data.FullName;
            genderText.text = account.Data.gender;
            emailText.text = account.Data.email;
            ipAddressText.text = account.Data.ip_address;
            favouriteIcon.sprite = account.State.isFavourite ? _uiConfig.FavouriteOn : _uiConfig.FavouriteOff;

            var avatar = _picsService.Get(account.State.avatarIndex);
            icon.sprite = avatar != null ? avatar : _uiConfig.DefaultIcon;
        }

        private void OnFavouriteChanged(int id, bool value)
        {
            if (id == _accountId)
            {
                favouriteIcon.sprite = value ? _uiConfig.FavouriteOn : _uiConfig.FavouriteOff;
            }
        }

        private void OnFavouriteClicked() => _accountService.ToggleFavourite(_accountId);
        private void OnBackClicked()
        {
            _onBack?.Invoke();

            _accountId = -1;
        }

        private void OnDestroy()
        {
            if (_accountService != null) _accountService.FavouriteChanged -= OnFavouriteChanged;
        }
    }
}
