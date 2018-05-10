using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.Weixin.Components
{
    [ViewComponent(Name = WeixinAuthenticationDefaults.ViewComponentName)]
    public class WeixinAuthenticationViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/ExternalAuth.Weixin/Views/WeixinAuthentication/PublicInfo.cshtml");
        }
    }
}