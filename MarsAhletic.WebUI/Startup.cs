using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MarsAhletic.WebUI.Startup))]
namespace MarsAhletic.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
