namespace Models
{
    public class Account
    {
        public AccountData Data { get; }
        public AccountState State { get; }

        public Account(AccountData accountData, AccountState accountState)
        {
            Data = accountData;
            State = accountState;
        }
    }
}
