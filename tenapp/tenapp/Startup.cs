using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(tenapp.Startup))]
namespace tenapp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
