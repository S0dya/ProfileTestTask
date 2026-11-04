using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Configs;
using Core;
using UnityEngine;

namespace Services
{
    public class PicsService
    {
        private readonly AppConfig _appConfig;
        private readonly HttpService _httpService;
        
        private readonly Sprite[] _avatars;
        private readonly string _avatarsPath;

        public PicsService(AppConfig appConfig, HttpService httpService)
        {
            _appConfig = appConfig;
            _httpService = httpService;
            
            _avatars = new Sprite[appConfig.AvatarCount];
            _avatarsPath = Path.Combine(Application.persistentDataPath, StorageKeys.StorageFolder, StorageKeys.AvatarsFolder);
        }

        public async Task Init(CancellationToken token)
        {
            Directory.CreateDirectory(_avatarsPath);

            for (int i = 0; i < _appConfig.AvatarCount; i++)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    _avatars[i] = await LoadAvatar(i, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"failed loading avatar | {e.Message}");
                }
            }
        }

        public Sprite Get(int index) => _avatars[index];

        private async Task<Sprite> LoadAvatar(int index, CancellationToken token)
        {
            var path = Path.Combine(_avatarsPath, $"{index}.png");
            byte[] bytes = await GetImageBytes(path, token);

            token.ThrowIfCancellationRequested();

            return CreateSprite(bytes);
        }

        private async Task<byte[]> GetImageBytes(string path, CancellationToken token)
        {
            if (File.Exists(path))
                return await File.ReadAllBytesAsync(path, token);

            byte[] bytes = await _httpService.Download(_appConfig.AvatarUrl, token);
            await File.WriteAllBytesAsync(path, bytes, token);
            return bytes;
        }

        private Sprite CreateSprite(byte[] bytes)
        {
            var texture = new Texture2D(2, 2);
            
            if (!texture.LoadImage(bytes))
            {
                UnityEngine.Object.Destroy(texture);
                return null;
            }

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }
    }
}
