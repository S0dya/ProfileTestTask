using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Configs;
using Core;
using Models;
using UnityEngine;

namespace Services
{
    public class AccountsDataService
    {
        [Serializable]
        private class AccountsJson
        {
            public AccountData[] data;
        }

        private readonly AppConfig _appConfig;
        private readonly JsonSaveService _jsonSaveService;
        private readonly HttpService _httpService;

        public AccountsDataService(AppConfig appConfig, JsonSaveService jsonSaveService, HttpService httpService)
        {
            _appConfig = appConfig;
            _jsonSaveService = jsonSaveService;
            _httpService = httpService;
        }

        public async Task<AccountData[]> Load(CancellationToken cancellationToken)
        {
            var saved = _jsonSaveService.Load<AccountsJson>(StorageKeys.Contacts);
            if (saved != null) return saved.data;

            var bytes = await _httpService.Download(_appConfig.AccountsUrl, cancellationToken);
            var accounts = Parse(Encoding.UTF8.GetString(bytes));
            _jsonSaveService.Save(StorageKeys.Contacts, new AccountsJson { data = accounts });

            return accounts;
        }

        private AccountData[] Parse(string json)
        {
            var result = JsonUtility.FromJson<AccountsJson>(json);
            if (result?.data == null) throw new InvalidDataException("Expected a JSON object with a 'data' array.");

            return result.data;
        }
    }
}
