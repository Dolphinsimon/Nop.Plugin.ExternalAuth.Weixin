
namespace Nop.Plugin.ExternalAuth.Weixin
{
    /// <summary>
    /// Default values used by the Weixin authentication middleware
    /// </summary>
    public class WeixinAuthenticationDefaults
    {
        /// <summary>
        /// System name of the external authentication method
        /// </summary>
        public const string ProviderSystemName = "ExternalAuth.Weixin";

        /// <summary>
        /// Name of the view component to display plugin in public store
        /// </summary>
        public const string ViewComponentName = "WeixinAuthentication";
    }
}