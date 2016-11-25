using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogSystem.WebApp.Startup))]
namespace BlogSystem.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
