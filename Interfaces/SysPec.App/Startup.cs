using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SysPec.App.Startup))]
namespace SysPec.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
