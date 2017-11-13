using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup(typeof(PocedWeb.Startup))]

namespace PocedWeb
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

            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                ClientId = "585966796531-o2h4on55tnjm6g8hsarotr5dddm5bsf3.apps.googleusercontent.com",
                ClientSecret = "VwgqtwyZ9hmbAqBuHELywPDE",
                SignInAsAuthenticationType = "Cookie"
            };
            app.UseGoogleAuthentication(google);

            var facebook = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                AppId = "1426662680922180",
                AppSecret = "8870f6bd80788834f2a9092503b95c89",
                SignInAsAuthenticationType = "Cookie"
            };
            app.UseFacebookAuthentication(facebook);

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
