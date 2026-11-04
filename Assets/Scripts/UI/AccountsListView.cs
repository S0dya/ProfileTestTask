using System;
using System.Collections.Generic;
using Configs;
using Models;
using UnityEngine;
using UnityEngine.Pool;

namespace UI
{
    public class AccountsListView : MonoBehaviour
    {
        [SerializeField] private AccountCardView cardPrefab;
        [SerializeField] private Transform cardParent;
        [SerializeField] private int defaultCapacity = 30;

        private readonly Dictionary<int, AccountCardView> _cards = new();
        private ObjectPool<AccountCardView> _cardPool;

        public void Init(Action<int> onFavourite, Action<int> onProfile, UIConfig uiConfig)
        {
            _cardPool = new ObjectPool<AccountCardView>(
                () =>
                {
                    var card = Instantiate(cardPrefab, cardParent);
                    card.gameObject.SetActive(false);
                    card.Init(onFavourite, onProfile, uiConfig);
                    return card;
                },
                card => card.gameObject.SetActive(true),
                card => card.gameObject.SetActive(false),
                defaultCapacity: defaultCapacity
            );
            
            for (int i = 0; i < defaultCapacity; i++) 
                _cardPool.Release(_cardPool.Get());
        }

        public void AddAccount(AccountData accountData, bool isFavourite, Sprite avatar)
        {
            var card = _cardPool.Get();
            card.Set(accountData, isFavourite, avatar);
            _cards.Add(accountData.id, card);
        }

        public void RemoveAccount(int id)
        {
            var card = _cards[id];
            _cards.Remove(id);
            
            _cardPool.Release(card);
        }

        public void SetFavourite(int id, bool value)
        {
            _cards[id].SetFavourite(value);
        }

        private void OnDestroy()
        {
            _cardPool?.Clear();
        }
    }
}
