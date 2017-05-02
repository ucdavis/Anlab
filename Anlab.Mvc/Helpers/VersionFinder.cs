using System.Reflection;

namespace AnlabMvc.Helpers
{
    public static class VersionFinder
    {
        private static string _version;

        public static string Version
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_version))
                {
                    _version = typeof(VersionFinder).GetTypeInfo()
                        .Assembly.GetName()
                        .Version.ToString();
                }
                return _version;
            }
            
        }
    }
}
