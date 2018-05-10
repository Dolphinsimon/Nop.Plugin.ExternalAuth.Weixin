using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.Weixin
{
    /// <summary>
    /// Weixin externalAuth processor
    /// </summary>
    public class WeixinAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public WeixinAuthenticationMethod(ISettingService settingService, IWebHelper webHelper)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/WeixinAuthentication/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "WeixinAuthentication";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new WeixinAuthenticationSettings
            {
                ClientKeyIdentifier = "",
                ClientSecret = "",
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Weixin.Login", "Login using Weixin account");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientKeyIdentifier", "App ID/API Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your FaceBook application page.");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientSecret", "App Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientSecret.Hint", "Enter your app secret here. You can find it on your FaceBook application page.");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WeixinAuthenticationSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Weixin.Login");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientKeyIdentifier");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientKeyIdentifier.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Weixin.ClientSecret.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
