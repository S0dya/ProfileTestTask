using Configs;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScreenController : MonoBehaviour
    {
        [SerializeField] private ProfileWindow profileWindow;
        [SerializeField] private MainWindow mainWindow;
        [SerializeField] private FavouritesWindow favouritesWindow;
        [Space]
        [SerializeField] private Button mainButton;
        [SerializeField] private Button favouritesButton;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image favouritesImage;
        [Space]
        [SerializeField] private Sprite footerSpriteOn;
        [SerializeField] private Sprite footerSpriteOff;

        private MonoBehaviour _currentWindow;

        private void Awake()
        {
            mainButton.onClick.AddListener(() => OpenScreen(mainWindow));
            favouritesButton.onClick.AddListener(() => OpenScreen(favouritesWindow));
            
            CloseProfile();
        }

        public void Init(AccountService accountService, PicsService picsService, UIConfig uiConfig)
        {
            profileWindow.Init(accountService, picsService, CloseProfile, uiConfig);
            mainWindow.Init(accountService, picsService, OpenProfile, uiConfig);
            favouritesWindow.Init(accountService, picsService, OpenProfile, uiConfig);
            
            OpenScreen(mainWindow);
        }

        private void OpenScreen(MonoBehaviour window)
        {
            if (_currentWindow == window) return;
            _currentWindow = window;

            mainWindow.gameObject.SetActive(window == mainWindow);
            favouritesWindow.gameObject.SetActive(window == favouritesWindow);
        }

        private void OpenProfile(int accountId)
        {
            profileWindow.Set(accountId);
            profileWindow.gameObject.SetActive(true);
        }

        private void CloseProfile()
        {
            profileWindow.gameObject.SetActive(false);
        }
    }
}
