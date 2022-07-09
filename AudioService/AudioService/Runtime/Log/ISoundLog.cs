namespace SF
{
    public interface ISoundLog
    {
        void Log(string content);
        void LogError(string content);
        void LogWarning(string content);
    }
}