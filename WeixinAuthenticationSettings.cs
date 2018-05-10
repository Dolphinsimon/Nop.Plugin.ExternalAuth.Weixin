using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.Weixin
{
    public class WeixinAuthenticationSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }

    }
}
