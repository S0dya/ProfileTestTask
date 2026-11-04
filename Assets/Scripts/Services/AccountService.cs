using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Core;
using Models;
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
            
            if (appConfig.AvatarCount < 2)
                throw new ArgumentOutOfRangeException(nameof(appConfig), "At least two avatars are required.");

            var statesById = savedState.accounts.ToDictionary(x => x.id);
            int previousAvatarIndex = -1;

            foreach (var accountData in accounts)
            {
                var accountState = GetOrCreateState(accountData, statesById);
                
                if (accountState.avatarIndex < 0 || accountState.avatarIndex >= appConfig.AvatarCount)
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

            Save();
        }
        
        public void ToggleFavourite(int id)
        {
            var state = _accountsById[id].State;
            state.isFavourite = !state.isFavourite;
            Save();
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

        private AccountState GetOrCreateState(AccountData accountData, Dictionary<int, AccountState> statesById)
        {
            if (!statesById.TryGetValue(accountData.id, out var accountState))
            {
                accountState = new AccountState
                {
                    id = accountData.id
                };
                statesById.Add(accountData.id, accountState);
            }
            
            return accountState;
        }
    }
}
