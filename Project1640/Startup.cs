using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartupAttribute(typeof(Project1640.Startup))]
namespace Project1640
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var authOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login/NotAuth"),
                ExpireTimeSpan = TimeSpan.FromMinutes(8),
            };
            app.UseCookieAuthentication(authOptions);
        }
    }
}