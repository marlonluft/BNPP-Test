namespace ClassLibrary1.Core.Crosscutting
{
    public interface ILog
    {
        void LogError(string message, object? arg);
    }
}
