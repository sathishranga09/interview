using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AWEPayUserAPI.Startup))]
namespace AWEPayUserAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
