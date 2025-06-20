using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiniAccountManagementSystem.Startup))]
namespace MiniAccountManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
