using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MineradorRH.Startup))]
namespace MineradorRH
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
