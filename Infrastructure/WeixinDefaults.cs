namespace Nop.Plugin.ExternalAuth.Weixin.Infrastructure
{
    public static class WeixinDefaults
    {
        public static readonly string DisplayName = "Weixin";
        public static readonly string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/oauth2/authorize";
        public static readonly string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";
        public static readonly string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";
        public const string AuthenticationScheme = "Weixin";
    }
}