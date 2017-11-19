using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Poced.Web;

[assembly: OwinStartup(typeof(Startup))]

namespace Poced.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseLogger();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookie",
                LoginPath = new PathString("/login")
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie",
                AuthenticationMode = AuthenticationMode.Passive,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });

            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                ClientId = "656376280253-dj3e3qqhmbcp4oir5gfk8btm17v8b9om.apps.googleusercontent.com",
                ClientSecret = "HvijClsL9XzNeVICelRuWc5W",
                SignInAsAuthenticationType = "ExternalCookie"
            };
            app.UseGoogleAuthentication(google);
        }
    }

    public class LoggingMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public LoggingMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }


        //public async Task Invoke(IDictionary<string, object> env)
        public async Task Invoke(IDictionary<string, object> env)
        {
            OwinContext ctx = new OwinContext(env);
            string httpMethod = ctx.Request.Method;
            string url = ctx.Request.Uri.AbsoluteUri;

            await _next.Invoke(env);

            int responseCode = ctx.Response.StatusCode;

            Console.WriteLine($"HTTP {httpMethod} to {url} returned: {responseCode}");
        }
    }

    public static class AppBuilderExtensions
    {
        public static void UseLogger(this IAppBuilder appBuilder)
        {
            appBuilder.Use<LoggingMiddleware>();
        }
    }
}
