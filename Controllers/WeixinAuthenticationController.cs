using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Weixin.Infrastructure;
using Nop.Plugin.ExternalAuth.Weixin.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Nop.Plugin.ExternalAuth.Weixin.Controllers
{
    public class WeixinAuthenticationController : BasePluginController
    {

        #region Fields

        private readonly WeixinAuthenticationSettings _weixinExternalAuthSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IOptionsMonitorCache<WeixinOptions> _optionsCache;

        #endregion

        #region Ctor

        public WeixinAuthenticationController(WeixinAuthenticationSettings weixinExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ISettingService settingService,
            IPermissionService permissionService,
            IOptionsMonitorCache<WeixinOptions> optionsCache,
            ILocalizationService localizationService)
        {
            this._weixinExternalAuthSettings = weixinExternalAuthSettings;
            this._externalAuthenticationService = externalAuthenticationService;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._optionsCache = optionsCache;

        }

        #endregion

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientKeyIdentifier = _weixinExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _weixinExternalAuthSettings.ClientSecret,
            };

            return View("~/Plugins/ExternalAuth.Weixin/Views/WeixinAuthentication/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _weixinExternalAuthSettings.ClientKeyIdentifier = model.ClientKeyIdentifier;
            _weixinExternalAuthSettings.ClientSecret = model.ClientSecret;
            _settingService.SaveSetting(_weixinExternalAuthSettings);

            //clear Weixin authentication options cache
            _optionsCache.TryRemove(WeixinDefaults.AuthenticationScheme);

            //now clear settings cache
            //_settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }



        public IActionResult Login(string returnUrl)
        {
            if (!_externalAuthenticationService.ExternalAuthenticationMethodIsAvailable(WeixinAuthenticationDefaults.ProviderSystemName))
                throw new NopException("Weixin authentication module cannot be loaded");

            if (string.IsNullOrEmpty(_weixinExternalAuthSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(_weixinExternalAuthSettings.ClientSecret))
                throw new NopException("Weixin authentication module not configured");

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "WeixinAuthentication", new { returnUrl = returnUrl })
            };

            return Challenge(authenticationProperties, WeixinDefaults.AuthenticationScheme);
        }


        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            var authenticateResult = await this.HttpContext.AuthenticateAsync(WeixinDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = WeixinAuthenticationDefaults.ProviderSystemName,
                AccessToken = await this.HttpContext.GetTokenAsync(WeixinDefaults.AuthenticationScheme, "access_token"),
                Email = string.Empty,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return _externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
        }
    }
}