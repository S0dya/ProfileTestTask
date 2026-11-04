using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "AppConfig", menuName = "ProfileTestTask/App Config")]
    public class AppConfig : ScriptableObject
    {
        [SerializeField] private string accountsUrl;
        [SerializeField] private string avatarUrl;
        [Space]
        [SerializeField] private int requestTimeout;
        [SerializeField] private int avatarCount;
        [Space]
        [SerializeField] private int batchSize = 20;

        public string AccountsUrl => accountsUrl;
        public string AvatarUrl => avatarUrl;
        
        public int RequestTimeout => requestTimeout;
        public int AvatarCount => avatarCount;
        
        public int BatchSize => batchSize;
    }
}
