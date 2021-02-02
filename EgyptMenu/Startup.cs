using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EgyptMenu.Startup))]
namespace EgyptMenu
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
