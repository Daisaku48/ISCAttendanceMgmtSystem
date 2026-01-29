using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SkillDiscriminantSystem.Web.Startup))]
namespace SkillDiscriminantSystem.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
