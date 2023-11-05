using System.Threading.Tasks;

namespace GamerVII.Launcher.Services.System
{
    /// <summary>
    /// Represents a service for system-related operations.
    /// </summary>
    public interface ISystemService
    {
        /// <summary>
        /// Retrieves the maximum available RAM in bytes.
        /// </summary>
        /// <returns>The maximum available RAM in bytes.</returns>
        ulong GetMaxAvailableRam();

        /// <summary>
        /// Retrieves the installation directory asynchronously.
        /// </summary>
        /// <returns>An asynchronous operation that yields the installation directory path.</returns>
        Task<string> GetInstallationDirectory();

        /// <summary>
        /// Retrieves the game path asynchronously.
        /// </summary>
        /// <returns>An asynchronous operation that yields the game path.</returns>
        Task<string> GetGamePath();

        /// <summary>
        /// Sets the installation directory asynchronously.
        /// </summary>
        /// <param name="appDirectory">The path to the installation directory.</param>
        Task SetInstallationDirectory(string appDirectory);
    }
}
