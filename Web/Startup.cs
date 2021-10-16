using System.Collections.Generic;
using System.IO;
using Chess.Models;
using HotSauceDbOrm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Data;
using Web.Interfaces;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();

            File.WriteAllText("HotSauceDb.hdb", null);
            var executor = Executor.GetInstance();
            executor.CreateTable<PlayResultEntity>();
            executor.CreateTable<GameSession>();
            services.AddSingleton(executor);
            services.AddTransient<IPlayResultDal, PlayResultDal>();
            services.AddTransient<IGameSessionDal, GameSessionDal>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseMvc();

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new
                    List<string> { "index.html" }
            });

            app.UseSignalR(route =>
            {
                route.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
