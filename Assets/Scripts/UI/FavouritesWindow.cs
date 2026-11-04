using Models;

namespace UI
{
    public class FavouritesWindow : AccountsWindow
    {
        protected override bool CheckAccount(Account account) => account.State.isFavourite;

        protected override void OnFavouriteChanged(int id, bool value)
        {
            if (value)
                AddAccount(_accountService.GetAccount(id));
            else
                scrollView.RemoveAccount(id);

            scrollView.Refresh();
        }
    }
}
