using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public class AudioDebug:MonoBehaviour
    {
#if UNITY_EDITOR && AUDIO_DEBUG

        [HideInInspector] public string bgmInfo;
        [HideInInspector] public string voiceInfo;
        [HideInInspector] public string uiInfo;
        [HideInInspector] public string sceneInfo;

        private float dur = 0.1f;

        private float totalTime = 0;
        private void Update()
        {
            totalTime += Time.deltaTime;
            if (totalTime >= dur)
            {
                totalTime = 0;
                var result = AudioService.GetInfo();
                bgmInfo = result.Item1;
                voiceInfo = result.Item2;
                uiInfo = result.Item3;
                sceneInfo = result.Item4;
            }
        }
        
#endif
    }
}