using BoC.InversionOfControl;
using Sitecore.Feature.Accounts.Services;
using Sitecore.Feature.Accounts.Repositories;

namespace Sitecore.Feature.Accounts.App_Start
{
    public class SetDependencies : IContainerInitializer
    {
        private readonly IDependencyResolver _resolver;

        public SetDependencies(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Execute()
        {
            if (!_resolver.IsRegistered<IAccountRepository>())
                this._resolver.RegisterType<IAccountRepository, AccountRepository>();
            if (!_resolver.IsRegistered<IAccountTrackerService>())
                this._resolver.RegisterType<IAccountTrackerService, AccountTrackerService>();
            if (!_resolver.IsRegistered<IAccountsSettingsService>())
                this._resolver.RegisterType<IAccountsSettingsService, AccountsSettingsService>();
            if (!_resolver.IsRegistered<IContactProfileService>())
                this._resolver.RegisterType<IContactProfileService, ContactProfileService>();
            if (!_resolver.IsRegistered<INotificationService>())
                this._resolver.RegisterType<INotificationService, NotificationService>();
            if (!_resolver.IsRegistered<IProfileSettingsService>())
                this._resolver.RegisterType<IProfileSettingsService, ProfileSettingsService>();
            if (!_resolver.IsRegistered<IUserProfileProvider>())
                this._resolver.RegisterType<IUserProfileProvider, UserProfileProvider>();
            if (!_resolver.IsRegistered<IUserProfileService>())
                this._resolver.RegisterType<IUserProfileService, UserProfileService>();
        }
    }
}