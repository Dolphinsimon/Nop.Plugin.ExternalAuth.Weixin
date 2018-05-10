using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Plugin.ExternalAuth.Weixin.Infrastructure
{

        public static class WeixinAuthenticationOptionsExtensions
        {
            public static AuthenticationBuilder AddWeixin(this AuthenticationBuilder builder)
            {
                return builder.AddWeixin("Weixin", (Action<WeixinOptions>)(_ => { }));
            }

            public static AuthenticationBuilder AddWeixin(this AuthenticationBuilder builder, Action<WeixinOptions> configureOptions)
            {
                return builder.AddWeixin("Weixin", configureOptions);
            }

            public static AuthenticationBuilder AddWeixin(this AuthenticationBuilder builder, string authenticationScheme, Action<WeixinOptions> configureOptions)
            {
                return builder.AddWeixin(authenticationScheme, WeixinDefaults.DisplayName, configureOptions);
            }

            public static AuthenticationBuilder AddWeixin(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeixinOptions> configureOptions)
            {
                return builder.AddOAuth<WeixinOptions, WeixinHandler>(authenticationScheme, displayName, configureOptions);
            }
        }
    }
