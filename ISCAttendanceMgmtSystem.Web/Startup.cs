using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ISCAttendanceMgmtSystem.Web.Startup))]
namespace ISCAttendanceMgmtSystem.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
