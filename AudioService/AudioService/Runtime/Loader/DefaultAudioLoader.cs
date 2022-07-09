using System;
using UnityEngine;

namespace SF
{
    public class DefaultAudioLoader:IAudioLoader
    {
        public AudioClip LoadSound(string path)
        {
            return Resources.Load<AudioClip>(path);
        }

        public void LoadSoundAsync(string path, Action<AudioClip> onComplete)
        {
            var request = Resources.LoadAsync<AudioClip>(path);
            request.completed+= delegate(AsyncOperation operation)
            {
                AudioClip clip=request.asset as AudioClip;
                onComplete?.Invoke(clip);
            };
        }

        public void UnloadSound(string path)
        {
            
        }
    }
}