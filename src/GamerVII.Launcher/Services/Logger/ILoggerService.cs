using System;

namespace GamerVII.Launcher.Services.Logger;

/// <summary>
/// Defines a logger service contract for logging messages.
/// </summary>
public interface ILoggerService : IDisposable
{
    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    void Log(string message);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">Exception</param>
    void Log(string message, Exception exception);
}
