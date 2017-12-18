using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using PushServer.Database;
using PushServer.Logistic;
using PushServer.Helpers;
using PushServer.Database.Repository;
using PushServer.Database.Repository.Interface;
using PushServer.Model.ConfigOptions;
using WDev.AspNetCore.Middleware;


namespace PushServer
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
            services.AddMvc();
            //DI Config setting
            ConfigStartup.RegisterConfigureSettingServices(Configuration,ref services);
            //DI Repository
            services.AddTransient<IFPS_PushHistoryRepository, FPS_PushHistoryRepository>();
            //DI Logic
            services.AddTransient<IPushMessageLogic, PushMessageLogic>();
            services.AddTransient<SerialAndEncryption, SerialAndEncryption>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Register Middleware about Catch every request and response .
            app.UseMiddleware<HttpLoggingMiddleware>();
            // Regisiter Middleware about Catch every Controller throw excetion and no throw 500 to res.
            app.UseMiddleware(typeof(ErrorHandlingMiddleware)); 
            app.UseMvc();
            env.ConfigureNLog("./nlog.config");
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();

        }
    }
}
