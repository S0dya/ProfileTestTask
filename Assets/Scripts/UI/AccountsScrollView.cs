using System;
using System.Collections.Generic;
using Configs;
using Models;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace UI
{
    public class AccountsScrollView : MonoBehaviour
    {
        [SerializeField] private AccountCardView cardPrefab;
        [SerializeField] private Transform cardParent;
        [SerializeField] private int defaultCapacity = 30;
        [Space]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float scrollLoadOffset;

        private readonly Dictionary<int, AccountCardView> _cards = new();
        private readonly List<AccountCardView> _orderedCards = new();
        private ObjectPool<AccountCardView> _cardPool;
        
        private Action _onLoadMore;
        private bool _isLoadingMore;
        private bool _hasMore = true;

        public void Init(Action<int> onFavourite, Action<int> onProfile, UIConfig uiConfig, Action onLoadMore = null)
        {
            _onLoadMore = onLoadMore;
            
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
                card => Destroy(card.gameObject),
                defaultCapacity: defaultCapacity
            );


            var cards = new List<AccountCardView>(defaultCapacity);
            for (int i = 0; i < defaultCapacity; i++) cards.Add(_cardPool.Get()); 
            for (int i = 0; i < defaultCapacity; i++) _cardPool.Release(cards[i]);
        }

        private void Update()
        {
            if (_isLoadingMore || !_hasMore) return;
            if (!scrollRect || !scrollRect.content) return;

            var contentHeight = scrollRect.content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            
            var currentBottomY = scrollRect.content.anchoredPosition.y + viewportHeight;

            if (currentBottomY >= contentHeight - scrollLoadOffset)
            {
                OnTryLoadMore();
            }
        }

        public void AddAccount(AccountData accountData, bool isFavourite, Sprite avatar)
        {
            if (_cards.ContainsKey(accountData.id)) return;

            var card = _cardPool.Get();
            card.Set(accountData, isFavourite, avatar);
            card.transform.SetAsLastSibling();
            
            _cards.Add(accountData.id, card);
            _orderedCards.Add(card);
        }

        public void RemoveAccount(int id)
        {
            if (!_cards.Remove(id, out var card)) return;
            
            _orderedCards.Remove(card);
            _cardPool.Release(card);
            
        }

        public void SetFavourite(int id, bool value)
        {
            if (_cards.TryGetValue(id, out var card)) card.SetFavourite(value);
        }

        public void Refresh()
        {
            for (int i = 0; i < _orderedCards.Count; i++)
                _orderedCards[i].SetBackground(i % 2 == 1);
        }

        public void StopLoading(bool hasMore)
        {
            _hasMore = hasMore;
            _isLoadingMore = false;
        }

        private void OnTryLoadMore()
        {
            if (_isLoadingMore) return;
            _isLoadingMore = true;

            _onLoadMore?.Invoke();
        }

        private void OnDestroy()
        {
            _cardPool?.Clear();
        }
    }
}
