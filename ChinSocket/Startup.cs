using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChinSocket
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<Manager>();
            services.AddSingleton<Handler>();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseWebSockets();

            Handler handler = serviceProvider.GetService<Handler>();
            app.Map("/ws", (_app) => _app.UseMiddleware<Middleware>(handler));

            app.UseFileServer();
        }
    }
}
