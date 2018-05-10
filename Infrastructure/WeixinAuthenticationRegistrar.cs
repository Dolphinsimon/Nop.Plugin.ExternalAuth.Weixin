using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Weixin.Infrastructure
{
    /// <summary>
    /// Registration of Facebook authentication service (plugin)
    /// </summary>
    public class WeixinAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddWeixin(WeixinDefaults.AuthenticationScheme, options =>
            {
                var settings = EngineContext.Current.Resolve<WeixinAuthenticationSettings>();

                options.AppId = settings.ClientKeyIdentifier;
                options.AppSecret = settings.ClientSecret;
                options.SaveTokens = true;
            });
        }
    }
}
