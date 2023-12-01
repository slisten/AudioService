using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using UnityEngine;

namespace SF
{
    /// <summary>
    /// 音频服务
    /// </summary>
    public static partial class AudioService
    {
        internal static IAudioLoader m_loader;
        internal static ISoundLog m_log;
        private static GameObject m_root;
        private static AudioListener m_listener;
        
        internal static List<AudioAgent> bgmAgents = new List<AudioAgent>();
        internal static List<AudioAgent> voiceAgents = new List<AudioAgent>();
        internal static List<AudioAgent> uiAgents = new List<AudioAgent>();
        internal static List<AudioAgent> sceneAgents = new List<AudioAgent>();
        

        #region Volume
        
        private static float m_bgmVolume = DefaultBGMVolume;
        public static float BGMVolume
        {
            get
            {
                return m_bgmVolume;
            }
            set
            {
                m_bgmVolume = value;
                foreach (AudioAgent agent in bgmAgents)
                {
                    agent.SetVolume(m_bgmVolume);
                }
            }
        }

        private static float m_voiceVolume = DefaultVoiceVolume;
        public static float VoiceVolume
        {
            get
            {
                return m_voiceVolume;
            }
            set
            {
                m_voiceVolume = value;
                foreach (AudioAgent agent in voiceAgents)
                {
                    agent.SetVolume(m_voiceVolume);
                }
            }
        }
        private static float m_uiVolume = DefaultUIVolume;
        public static float UIVolume
        {
            get
            {
                return m_uiVolume;
            }
            set
            {
                m_uiVolume = value;
                foreach (AudioAgent agent in uiAgents)
                {
                    agent.SetVolume(m_uiVolume);
                }
            }
        }
        private static float m_sceneVolume = DefaultSceneVolume;
        public static float SceneVolume
        {
            get
            {
                return m_sceneVolume;
            }
            set
            {
                m_sceneVolume = value;
                foreach (AudioAgent agent in sceneAgents)
                {
                    agent.SetVolume(m_sceneVolume);
                }
            }
        }
        #endregion

        #region Mute
        
        private static bool m_bgmMute = false;
        public static bool BGMMute
        {
            get
            {
                return m_bgmMute;
            }
            set
            {
                m_bgmMute = value;
                foreach (AudioAgent agent in bgmAgents)
                {
                    agent.SetMute(m_bgmMute);
                }
            }
        }
        
        private static bool m_voiceMute = false;
        public static bool VoiceMute
        {
            get
            {
                return m_voiceMute;
            }
            set
            {
                m_voiceMute = value;
                foreach (AudioAgent agent in voiceAgents)
                {
                    agent.SetMute(m_voiceMute);
                }
            }
        }
        
        private static bool m_uiMute = false;
        public static bool UIMute
        {
            get
            {
                return m_uiMute;
            }
            set
            {
                m_uiMute = value;
                foreach (AudioAgent agent in uiAgents)
                {
                    agent.SetMute(m_uiMute);
                }
            }
        }
        
        private static bool m_sceneMute = false;
        public static bool SceneMute
        {
            get
            {
                return m_sceneMute;
            }
            set
            {
                m_sceneMute = value;
                foreach (AudioAgent agent in sceneAgents)
                {
                    agent.SetMute(m_sceneMute);
                }
            }
        }
        
        #endregion

        #region Debug
#if UNITY_EDITOR && AUDIO_DEBUG
        internal static int activeBgm;
        internal static int activeVoice;
        internal static int activeUI;
        internal static int activeScene;
        public static (string,string,string,string) GetInfo()
        {
            string item1 = string.Format("BGM:{0}/{1}\n", activeBgm, bgmAgents.Count);
            string item2 = string.Format("Voice:{0}/{1}\n", activeVoice, voiceAgents.Count);
            string item3 = string.Format("UI:{0}/{1}\n", activeUI, uiAgents.Count);
            string item4 = string.Format("Scene:{0}/{1}\n", activeScene, sceneAgents.Count);
            return (item1, item2, item3, item4);
        }
#endif
        

        #endregion

        public static void Init(IAudioLoader loader=null,ISoundLog log=null)
        {
            if (loader != null)
            {
                m_loader = loader;
            }
            else
            {
                m_loader = new DefaultAudioLoader();
            }
            if (log != null)
            {
                m_log = log;
            }
            else
            {
                m_log = new DefaultAudioLog();
            }

            m_root = new GameObject("[Audio]");
            GameObject.DontDestroyOnLoad(m_root);
            
            m_listener=MonoBehaviour.FindObjectOfType<AudioListener>();
            if(!m_listener)
                m_listener = m_root.AddComponent<AudioListener>();

            
            for (int i = 0; i < BGMMaxCount; i++)
            {
                AudioAgent agent=new AudioAgent(m_root,SoundType.BGM);
                bgmAgents.Add(agent);
            }

            for (int i = 0; i < VoiceMaxCount; i++)
            {
                AudioAgent agent=new AudioAgent(m_root,SoundType.Voice);
                voiceAgents.Add(agent);
            }
            
            for (int i = 0; i < UIMaxCount; i++)
            {
                AudioAgent agent=new AudioAgent(m_root,SoundType.UI);
                uiAgents.Add(agent);
            }
            
            for (int i = 0; i < SceneMaxCount; i++)
            {
                AudioAgent agent=new AudioAgent(m_root,SoundType.Scene);
                sceneAgents.Add(agent);
            }

#if UNITY_EDITOR && AUDIO_DEBUG
            GameObject debugObj=new GameObject("[Audio Debug]");
            GameObject.DontDestroyOnLoad(debugObj);
            debugObj.AddComponent<AudioDebug>();
#endif
        }

        public static void Destroy()
        {
            foreach (AudioAgent agent in bgmAgents)
            {
                agent.Destroy();
            }
            bgmAgents.Clear();
            
            foreach (AudioAgent agent in voiceAgents)
            {
                agent.Destroy();
            }
            voiceAgents.Clear();
            
            foreach (AudioAgent agent in uiAgents)
            {
                agent.Destroy();
            }
            uiAgents.Clear();
            
            foreach (AudioAgent agent in sceneAgents)
            {
                agent.Destroy();
            }
            sceneAgents.Clear();
        }

        #region Sound
        

        public static int PlaySound(string path, SoundType type=SoundType.Scene, bool isLoop=false, Action<AudioSource> onComplete = null)
        {
            var tuple = PopFreeAgent(type);
            tuple.Item1.PlaySound(path,isLoop,GetVolByType(type),GetMuteByType(type),onComplete);
            return tuple.Item2;
        }
        
        public static void StopSound(int channel,SoundType type=SoundType.Scene)
        {
            var agents = GetListByType(type);
            if (agents.Count > channel)
            {
                if (agents[channel].playState != PlayState.Stop)
                {
                    agents[channel].Stop();
                }
            }
            else
            {
                m_log.LogWarning("不存在该channel："+channel);
            }
        }

        public static void PauseSound(int channel,SoundType type=SoundType.Scene)
        {
            var agents = GetListByType(type);
            if (agents.Count > channel)
            {
                if (agents[channel].playState == PlayState.Playing)
                {
                    agents[channel].Pause();
                }
            }
            else
            {
                m_log.LogWarning("该音频不处于活动状态："+channel);
            }
        }

        public static void ResumeSound(int channel,SoundType type=SoundType.Scene)
        {
            var agents = GetListByType(type);
            if (agents.Count > channel)
            {
                if (agents[channel].playState == PlayState.Pause)
                {
                    agents[channel].Resume();
                }
            }
            else
            {
                m_log.LogWarning("该音频不处于活动状态："+channel);
            }
        }
        
        #endregion


        public static void Update()
        {
            foreach (AudioAgent agent in bgmAgents)
            {
                agent.OnUpdate();
            }
            
            foreach (AudioAgent agent in voiceAgents)
            {
                agent.OnUpdate();
            }
            
            foreach (AudioAgent agent in uiAgents)
            {
                agent.OnUpdate();
            }
            
            foreach (AudioAgent agent in sceneAgents)
            {
                agent.OnUpdate();
            }
        }
        #region Inner

        private static (AudioAgent,int) PopFreeAgent(SoundType type)
        {
            AudioAgent agent = null;
            int channel = -1;
            var agents = GetListByType(type);
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].playState == PlayState.Free)
                {
                    agent = agents[i];
                    channel = i;
                    break;
                }
            }

            if (agent == null)
            {
                agent=new AudioAgent(m_root,type);
                agents.Add(agent);
                channel = agents.Count - 1;
                m_log.LogWarning("扩展："+agents.Count);
            }

            return (agent, channel);
        }

        private static List<AudioAgent> GetListByType(SoundType type)
        {
            List<AudioAgent> agents = null;
            switch (type)
            {
                case SoundType.BGM:
                    agents = bgmAgents;
                    break;
                case SoundType.Voice:
                    agents = voiceAgents;
                    break;
                case SoundType.UI:
                    agents = uiAgents;
                    break;
                case SoundType.Scene:
                    agents = sceneAgents;
                    break;
            }

            return agents;
        }
        
        private static float GetVolByType(SoundType type)
        {
            float vol = 1;
            switch (type)
            {
                case SoundType.BGM:
                    vol = m_bgmVolume;
                    break;
                case SoundType.Voice:
                    vol = m_voiceVolume;
                    break;
                case SoundType.UI:
                    vol = m_uiVolume;
                    break;
                case SoundType.Scene:
                    vol = m_sceneVolume;
                    break;
            }

            return vol;
        }
        
        private static bool GetMuteByType(SoundType type)
        {
            bool mute = false;
            switch (type)
            {
                case SoundType.BGM:
                    mute = m_bgmMute;
                    break;
                case SoundType.Voice:
                    mute = m_voiceMute;
                    break;
                case SoundType.UI:
                    mute = m_uiMute;
                    break;
                case SoundType.Scene:
                    mute = m_sceneMute;
                    break;
            }

            return mute;
        }

        #endregion
    }

    public enum SoundType
    {
        Voice,
        UI,
        Scene,
        BGM
    }
}