using System;
using Configs;
using Models;
using Services;
using UnityEngine;

namespace UI
{
    public class MainWindow : MonoBehaviour
    {
        [SerializeField] private AccountsScrollView scrollView;

        private AccountService _accountService;
        private PicsService _picsService;
        private AppConfig _appConfig;
        
        private int _currentOffset;

        public void Init(AccountService accountService, PicsService picsService, Action<int> onProfile, UIConfig uiConfig, AppConfig appConfig)
        {
            _accountService = accountService;
            _picsService = picsService;
            _appConfig = appConfig;
            
            scrollView.Init(accountService.ToggleFavourite, onProfile, uiConfig, OnLoadMore);

            LoadNextBatch();

            _accountService.FavouriteChanged += OnFavouriteChanged;
        }
        
        
        private void LoadNextBatch()
        {
            int count = 0;
            for (; _currentOffset < _accountService.Accounts.Count && count < _appConfig.BatchSize; _currentOffset++)
            {
                AddAccount(_accountService.Accounts[_currentOffset]);
                count++;
            }
            
            scrollView.Refresh();
            scrollView.StopLoading();
        }

        private void OnLoadMore()
        {
            if (_currentOffset >= _accountService.Accounts.Count)
            {
                scrollView.StopLoading();
                return;
            }
            
            LoadNextBatch();
        }

        private void AddAccount(Account account)
        {
            var avatar = _picsService.Get(account.State.avatarIndex);
            scrollView.AddAccount(account.Data, account.State.isFavourite, avatar);
        }

        private void OnFavouriteChanged(int id, bool value)
        {
            scrollView.SetFavourite(id, value);
        }

        private void OnDestroy()
        {
            if (_accountService != null) _accountService.FavouriteChanged -= OnFavouriteChanged;
        }
    }
}
