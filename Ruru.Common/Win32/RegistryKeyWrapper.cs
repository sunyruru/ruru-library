namespace Ruru.Common.Win32
{
    using Microsoft.Win32;

    internal class RegistryKeyWrapper : RegistryKeyBase, IRegistryKey
    {
        private RegistryKey registryKey;

        public RegistryKeyWrapper(RegistryKey registryKey)
        {
            this.registryKey = registryKey;
        }

        protected override object DoGetValue(string name)
        {
            return this.registryKey.GetValue(name);
        }

        public override void Close()
        {
            this.registryKey.Close();
        }

        public override string[] GetValueNames()
        {
            return this.registryKey.GetValueNames();
        }

        public override IRegistryKey DoOpenSubKey(string name)
        {
            RegistryKey subKey = this.registryKey.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree);
            if (subKey != null)
                return new RegistryKeyWrapper(subKey);

            return null;
        }

        public void Dispose()
        {
            this.Close();
        }

        public override string Name
        {
            get { return registryKey.Name; }
        }
    }
}
