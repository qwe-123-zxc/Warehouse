using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WarehouseWeb.Startup))]
namespace WarehouseWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
