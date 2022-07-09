using System;
using UnityEngine;

namespace SF
{
    public class AudioAgent
    {
        private GameObject m_root;
        private AudioSource m_audioSource;
        internal SoundType soundType;
        internal PlayState playState;
        internal string path;
        private Action<AudioSource> m_onComplete;

        public AudioAgent(GameObject root,SoundType type)
        {
            m_root = root;
            soundType = type;
            playState = PlayState.Free;
            m_audioSource = m_root.AddComponent<AudioSource>();
        }

        public void SetVolume(float value)
        {
            if (m_audioSource != null)
            {
                m_audioSource.volume = value;
            }
        }

        public void SetMute(bool mute)
        {
            if (m_audioSource != null)
            {
                m_audioSource.mute = mute;
            }
        }

        public void SetPitch(float value)
        {
            if (m_audioSource != null)
            {
                m_audioSource.pitch = value;
            }
        }

        public void OnUpdate()
        {
            if (m_audioSource != null)
            {
                if (playState == PlayState.Playing && !m_audioSource.isPlaying)
                {
                    playState = PlayState.Complete;
                    m_onComplete?.Invoke(m_audioSource);
                    AudioService.m_loader.UnloadSound(path);
                }

                if (playState == PlayState.Stop || playState == PlayState.Complete)
                {
                    playState = PlayState.Free;
#if UNITY_EDITOR && AUDIO_DEBUG
                    
                    if (soundType==SoundType.BGM)
                    {
                        AudioService.activeBgm--;
                    }
                    else if (soundType==SoundType.Voice)
                    {
                        AudioService.activeVoice--;
                    }
                    else if(soundType==SoundType.UI)
                    {
                        AudioService.activeUI--;
                    }
                    else if(soundType==SoundType.Scene)
                    {
                        AudioService.activeScene--;
                    }
#endif
                }
            }
        }

        public void PlaySound(string path, bool isLoop, float volume,bool isMute, Action<AudioSource> complete=null)
        {
#if UNITY_EDITOR && AUDIO_DEBUG
            if (soundType==SoundType.BGM)
            {
                AudioService.activeBgm++;
            }
            else if (soundType==SoundType.Voice)
            {
                AudioService.activeVoice++;
            }
            else if(soundType==SoundType.UI)
            {
                AudioService.activeUI++;
            }
            else if(soundType==SoundType.Scene)
            {
                AudioService.activeScene++;
            }
#endif
            this.path = path;
            playState = PlayState.Playing;
            AudioClip clip = AudioService.m_loader.LoadSound(path);
            
            if (clip == null)
            {
                AudioService.m_log.LogError("加载音效失败："+path);
                playState = PlayState.Stop;
                return;
            }

            m_audioSource.clip = clip;
            m_audioSource.loop = isLoop;
            m_audioSource.volume = volume;
            m_audioSource.mute = isMute;
            m_audioSource.Play();
            m_onComplete = complete;
        }

        public void Stop()
        {
            playState = PlayState.Stop;
            if (m_audioSource != null)
            {
                m_audioSource.Stop();
                AudioService.m_loader.UnloadSound(path);
            }
        }
        
        public void Pause()
        {
            playState = PlayState.Pause;
            if (m_audioSource != null)
            {
                m_audioSource.Pause();
            }
        }

        public void Resume()
        {
            playState = PlayState.Playing;
            if (m_audioSource != null)
            {
                m_audioSource.UnPause();
            }
        }

        public void Destroy()
        {
            if (playState == PlayState.Playing || playState == PlayState.Pause)
            {
                AudioService.m_loader.UnloadSound(path);
            }
            
            if (m_audioSource != null)
            {
                GameObject.Destroy(m_audioSource);
            }
        }
    }
    
    public enum PlayState
    {
        Free,
        Playing,
        Pause,
        Stop,
        Complete
    }
}