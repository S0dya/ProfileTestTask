using System;
using System.Threading;
using Configs;
using Models;
using Services;
using UI;
using UnityEngine;

namespace Core
{
    public class AppBootstrap : MonoBehaviour
    {
        [SerializeField] private AppConfig appConfig;
        [SerializeField] private UIConfig uiConfig;
        [SerializeField] private ScreenController screenController;

        private readonly CancellationTokenSource _lifetime = new();
        private JsonSaveService _saveService;
        private AccountService _accountService;
        private HttpService _httpService;
        private PicsService _picsService;

        private async void Start()
        {
            var token = _lifetime.Token;

            try
            {
                _saveService = new JsonSaveService();
                _httpService = new HttpService(appConfig);
                _picsService = new PicsService(appConfig, _httpService);

                var accountsDataService = new AccountsDataService(appConfig, _saveService, _httpService);
                var accounts = await accountsDataService.Load(token);
                token.ThrowIfCancellationRequested();

                var accountsState = _saveService.Load<AccountsState>(StorageKeys.State) ?? new AccountsState();
                _accountService = new AccountService(accounts, accountsState, appConfig, _saveService);

                await _picsService.Init(token);
                token.ThrowIfCancellationRequested();
                screenController.Init(_accountService, _picsService, uiConfig);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void Save()
        {
            _accountService?.Save();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) Save();
        }
        private void OnApplicationFocus(bool focused)
        {
            if (!focused) Save();
        }
        private void OnApplicationQuit() => Save();

        private void OnDestroy()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }
    }
}
