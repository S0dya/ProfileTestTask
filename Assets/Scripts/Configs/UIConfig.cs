using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "ProfileTestTask/UI Config")]
    public class UIConfig : ScriptableObject
    {
        [SerializeField] private Sprite defaultIcon;
        [SerializeField] private Sprite favouriteOn;
        [SerializeField] private Sprite favouriteOff;

        public Sprite DefaultIcon => defaultIcon;
        public Sprite FavouriteOn => favouriteOn;
        public Sprite FavouriteOff => favouriteOff;
    }
}
