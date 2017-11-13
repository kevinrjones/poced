using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using PocedWeb;

[assembly: OwinStartup(typeof(Startup))]

namespace PocedWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseLogger();
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
