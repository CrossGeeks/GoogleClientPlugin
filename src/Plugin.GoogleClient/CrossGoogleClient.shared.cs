using System;

namespace Plugin.GoogleClient
{
    /// <summary>
    /// Cross GoogleClient
    /// </summary>
    public static class CrossGoogleClient
    {
        static Lazy<IGoogleClientManager> implementation = new Lazy<IGoogleClientManager>(() => CreateGoogleClient(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;
        


        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static IGoogleClientManager Current
        {
            get
            {
                IGoogleClientManager ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }

                
                return ret;
            }
        }

        static IGoogleClientManager CreateGoogleClient()
        {
#if  NETSTANDARD2_0
            return null;
#else
            return new GoogleClientManager();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    }
}
