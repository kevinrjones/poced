using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(pocedweb.Startup))]

namespace pocedweb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Console.WriteLine();
        }
    }
}
