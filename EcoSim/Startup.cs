using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EcoSim.Startup))]
namespace EcoSim
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
