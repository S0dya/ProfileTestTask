using System;
using Configs;
using Models;
using Services;
using UnityEngine;

namespace UI
{
    public abstract class AccountsWindow : MonoBehaviour
    {
        [SerializeField] protected AccountsScrollView scrollView;

        protected AccountService _accountService;
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
                if (!CheckAccount(_accountService.Accounts[_currentOffset])) continue;

                AddAccount(_accountService.Accounts[_currentOffset]);
                count++;
            }
            
            scrollView.Refresh();
            scrollView.StopLoading(_currentOffset < _accountService.Accounts.Count);
        }

        private void OnLoadMore()
        {
            if (_currentOffset >= _accountService.Accounts.Count)
            {
                scrollView.StopLoading(_currentOffset < _accountService.Accounts.Count);
                return;
            }
            
            LoadNextBatch();
        }

        protected void AddAccount(Account account)
        {
            var avatar = _picsService.Get(account.State.avatarIndex);
            scrollView.AddAccount(account.Data, account.State.isFavourite, avatar);
        }

        protected virtual void OnFavouriteChanged(int id, bool value)
        {
            scrollView.SetFavourite(id, value);
        }

        protected virtual bool CheckAccount(Account account) => true;

        protected virtual void OnDestroy()
        {
            if (_accountService != null) _accountService.FavouriteChanged -= OnFavouriteChanged;
        }
    }
}
