using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Core;
using Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Services
{
    public class AccountService
    {
        public event Action<int, bool> FavouriteChanged;

        public List<Account> Accounts { get; } = new();

        private readonly Dictionary<int, Account> _accountsById = new();
        private readonly JsonSaveService _saveService;

        public AccountService(AccountData[] accounts, AccountsState savedState, AppConfig appConfig, JsonSaveService saveService)
        {
            _saveService = saveService;
            
            int previousAvatarIndex = -1;

            foreach (var accountData in accounts)
            {
                var accountState = GetOrCreateState(accountData, savedState);
                
                if (accountState.avatarIndex < 0)
                {
                    do
                    {
                        accountState.avatarIndex = Random.Range(0, appConfig.AvatarCount);
                    }
                    while (accountState.avatarIndex == previousAvatarIndex);
                }

                previousAvatarIndex = accountState.avatarIndex;
                
                var account = new Account(accountData, accountState);
                _accountsById.Add(accountData.id, account);
                Accounts.Add(account);
            }
        }
        
        public void ToggleFavourite(int id)
        {
            var state = _accountsById[id].State;
            state.isFavourite = !state.isFavourite;
            FavouriteChanged?.Invoke(id, state.isFavourite);
        }
        
        public Account GetAccount(int id)
        {
            if (!_accountsById.TryGetValue(id, out var account)) throw new KeyNotFoundException($"Account with id {id} not found");
            return account;
        }

        public void Save()
        {
            _saveService.Save(StorageKeys.State, new AccountsState
            {
                accounts = Accounts.Select(a => a.State).ToList()
            });
        }

        private AccountState GetOrCreateState(AccountData accountData, AccountsState savedState)
        {
            var accountState = savedState.accounts.FirstOrDefault(x => x.id == accountData.id);
            
            if (accountState == null)
            {
                accountState = new AccountState
                {
                    id = accountData.id
                };
                savedState.accounts.Add(accountState);
            }
            
            return accountState;
        }
    }
}
