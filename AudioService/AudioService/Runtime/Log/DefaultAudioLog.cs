using UnityEngine;

namespace SF
{
    public class DefaultAudioLog:ISoundLog
    {
        public void Log(string content)
        {
            Debug.Log(content);
        }

        public void LogError(string content)
        {
            Debug.LogError(content);
        }

        public void LogWarning(string content)
        {
            Debug.LogWarning(content);
        }
    }
}