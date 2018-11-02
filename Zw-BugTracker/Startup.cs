using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Zw_BugTracker.Startup))]
namespace Zw_BugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
