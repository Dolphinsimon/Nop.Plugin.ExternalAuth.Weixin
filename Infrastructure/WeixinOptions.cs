using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Security.Claims;

namespace Nop.Plugin.ExternalAuth.Weixin.Infrastructure
{
    public class WeixinOptions : OAuthOptions
    {
        public WeixinOptions()
        {
            this.CallbackPath = new PathString("/signin-weixin");
            this.AuthorizationEndpoint = WeixinDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = WeixinDefaults.TokenEndpoint;
            this.UserInformationEndpoint = WeixinDefaults.UserInformationEndpoint;
            //SaveTokens = true;   
            this.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
            this.ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            this.ClaimActions.MapJsonKey("urn:WeChat:sex", "sex");
            this.ClaimActions.MapJsonKey(ClaimTypes.Country, "country");
            this.ClaimActions.MapJsonKey(ClaimTypes.StateOrProvince, "province");
            this.ClaimActions.MapJsonKey("urn: WeChat:city", "city");
            this.ClaimActions.MapJsonKey("urn:WeChat:headimgurl", "headimgurl");
            this.ClaimActions.MapJsonKey("urn:WeChat:unionid", "unionid");
        }

        /// <summary>
        /// Check that the options are valid.  Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            if (string.IsNullOrEmpty(this.AppId))
                throw new ArgumentException(
                    string.Format((IFormatProvider) CultureInfo.CurrentCulture, "OptionMustBeProvided",
                        (object) "AppId"), "AppId");
            if (string.IsNullOrEmpty(this.AppSecret))
                throw new ArgumentException(
                    string.Format((IFormatProvider) CultureInfo.CurrentCulture, "OptionMustBeProvided",
                        (object) "AppSecret"), "AppSecret");
            base.Validate();
        }

        /// <summary>Gets or sets the Weixin-assigned appId.</summary>
        public string AppId
        {
            get { return this.ClientId; }
            set { this.ClientId = value; }
        }

        /// <summary>Gets or sets the Weixin-assigned app secret.</summary>
        public string AppSecret
        {
            get { return this.ClientSecret; }
            set { this.ClientSecret = value; }
        }

    }
}
