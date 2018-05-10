using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Nop.Plugin.ExternalAuth.Weixin.Infrastructure
{
    public class WeixinHandler : OAuthHandler<WeixinOptions>
    {
        public WeixinHandler(IOptionsMonitor<WeixinOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (string.IsNullOrEmpty(properties.RedirectUri))
                properties.RedirectUri = this.CurrentUri;
            this.GenerateCorrelationId(properties);
            string redirectUri = this.BuildChallengeUrl(properties, this.BuildRedirectUri(properties.RedirectUri));
            RedirectContext<OAuthOptions> context = new RedirectContext<OAuthOptions>(this.Context, this.Scheme, (OAuthOptions)this.Options, properties, redirectUri);
            await this.Events.RedirectToAuthorizationEndpoint(context);
        }

        /// <summary>
        /// OAuth第一步,获取code
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            //string oauthstate = this.Options.StateDataFormat.Protect(properties);
            var oauthstate = "Weixin" + DateTime.Now.Millisecond;
            return OAuthApi.GetAuthorizeUrl(this.Options.AppId, redirectUri, oauthstate, OAuthScope.snsapi_userinfo);
        }

        /// <summary>
        /// OAuth第二步,获取token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            var weixinToken = await OAuthApi.GetAccessTokenAsync(this.Options.AppId, this.Options.AppSecret, code);
            if (weixinToken == null) return OAuthTokenResponse.Failed(new Exception("OAuth token endpoint failure"));
            var payload = JObject.FromObject(weixinToken);
            var tokens = OAuthTokenResponse.Success(payload);
            //借用TokenType属性保存openid
            tokens.TokenType = weixinToken.openid;
            return tokens;
        }

        /// <summary>
        /// 获取用户信息生成Ticket
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="properties"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity,
            AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var weixinUserInfo = await OAuthApi.GetUserInfoAsync(tokens.AccessToken, tokens.TokenType);
            JObject user = JObject.FromObject(weixinUserInfo);
            //Reset tokenType to "Weixin"
            tokens.TokenType = WeixinDefaults.AuthenticationScheme;
            OAuthCreatingTicketContext context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity),
                properties, this.Context, this.Scheme, this.Options, this.Backchannel, tokens, user);
            context.RunClaimActions();
            await this.Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, this.Scheme.Name);
        }
    }
}