using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.ExternalAuth.Weixin.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.Weixin.ClientKeyIdentifier")]
        public string ClientKeyIdentifier { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Weixin.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}