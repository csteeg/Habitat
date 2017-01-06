using System;

namespace Sitecore.Foundation.Redirects.Services
{
    public class RedirectPublishDisabler : IDisposable
    {
        private static bool _isDisabled;
        public static bool IsDisabled { get { return _isDisabled; } }

        public void Dispose()
        {
            _isDisabled = false;
        }

        public RedirectPublishDisabler()
        {
            _isDisabled = true;
        }
    }
}
