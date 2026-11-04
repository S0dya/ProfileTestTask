using System;
using System.Collections.Generic;

namespace Models
{
    [Serializable]
    public class AccountsState
    {
        public List<AccountState> accounts = new();
    }

    [Serializable]
    public class AccountState
    {
        public int id;
        public bool isFavourite;
        public int avatarIndex = -1;
    }
}
