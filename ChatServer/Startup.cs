using System;
using System.IO;
using ChatServer.Hubs;
using ChatServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ChatServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            var path = Directory.GetCurrentDirectory();
            services.AddSingleton<IMessageHistoryService>(new FileMessageHistory($"{path}\\History.txt"));
            services.AddSingleton<IConnectedUsersService, ConnectedUsersService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSignalR(routes =>
           {
               routes.MapHub<ChatHub>("/chat");
           });

        }
    }
}
