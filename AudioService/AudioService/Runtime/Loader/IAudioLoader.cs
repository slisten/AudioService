using System;
using UnityEngine;

namespace SF
{
    public interface IAudioLoader
    {
        AudioClip LoadSound(string path);
        void LoadSoundAsync(string path, Action<AudioClip> onComplete);
        void UnloadSound(string path);
    }
}