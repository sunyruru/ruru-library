namespace Ruru.Common.Win32
{
    using Microsoft.Win32;

    internal class RegistryAccessor : IRegistryAccessor
    {
        public IRegistryKey CurrentUser
        {
            get { return new RegistryKeyWrapper(Registry.CurrentUser); }
        }

        public IRegistryKey LocalMachine
        {
            get { return new RegistryKeyWrapper(Registry.LocalMachine); }
        }
    }
}
