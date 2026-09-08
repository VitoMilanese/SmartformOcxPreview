using System.Runtime.CompilerServices;
using NLog;

namespace SmartformOcxPreview
{
    public static class LogHelper
    {
        private static Logger Logger { get; } = LogManager.GetLogger("AppLogger");

        public static void LogException(Exception ex, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string caller = null) =>
            LogException(ex, null, lineNumber, caller);

        public static void LogException(Exception ex, string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string caller = null)
        {
            if (ex == null || Logger == null) return;
            LogCallerInfo(lineNumber, caller);
            if (string.IsNullOrWhiteSpace(message)) message = null;
            Logger?.Error(ex, "SmartformOcxPreview: " + (message ?? ex.Message));
            if (ex.InnerException != null)
                LogException(ex.InnerException, lineNumber, caller);
        }

        public static void LogError(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string caller = null)
        {
            if (Logger == null) return;
            LogCallerInfo(lineNumber, caller);
            Logger?.Error("SmartformOcxPreview: " + message);
        }

        public static void LogDebug(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string caller = null)
        {
            if (Logger == null) return;
            LogCallerInfo(lineNumber, caller);
            Logger?.Debug("SmartformOcxPreview: " + message);
        }

        public static void LogInfo(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string caller = null)
        {
            if (Logger == null) return;
            Logger?.Info("SmartformOcxPreview: " + message);
        }

        public static void LogCallerInfo([CallerLineNumber] int lineNumber = 0, [CallerFilePath] string caller = null) =>
            Logger?.Debug($"SmartformOcxPreview: {caller}: line {lineNumber}");
    }
}
