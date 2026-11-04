using System;
using Configs;
using Models;
using Services;
using UnityEngine;

namespace UI
{
    public class FavouritesWindow : MonoBehaviour
    {
        [SerializeField] private AccountsListView listView;

        private AccountService _accountService;
        private PicsService _picsService;

        public void Init(AccountService accountService, PicsService picsService, Action<int> onProfile, UIConfig uiConfig)
        {
            _accountService = accountService;
            _picsService = picsService;
            
            listView.Init(accountService.ToggleFavourite, onProfile, uiConfig);

            foreach (var account in accountService.Accounts)
                if (account.State.isFavourite)
                    AddAccount(account);

            _accountService.FavouriteChanged += OnFavouriteChanged;
        }

        private void AddAccount(Account account)
        {
            var avatar = _picsService.Get(account.State.avatarIndex);
            listView.AddAccount(account.Data, account.State.isFavourite, avatar);
        }

        private void OnFavouriteChanged(int id, bool value)
        {
            if (value)
                AddAccount(_accountService.GetAccount(id));
            else
                listView.RemoveAccount(id);
        }

        private void OnDestroy()
        {
            if (_accountService != null) _accountService.FavouriteChanged -= OnFavouriteChanged;
        }
    }
}
