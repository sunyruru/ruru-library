namespace Ruru.Common.Win32
{
    /// <summary>
    /// Provides access to the registry.
    /// </summary>
    /// <remarks>
    /// This interface allows for unit testing without requiring access to the machine's registry.
    /// </remarks>
    public interface IRegistryAccessor
    {
        /// <summary>
        /// Gets registry key HKCU.
        /// </summary>
        IRegistryKey CurrentUser { get; }

        /// <summary>
        /// Gets registry key HKLM.
        /// </summary>
        IRegistryKey LocalMachine { get; }
    }
}
