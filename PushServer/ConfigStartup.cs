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
using PushServer.Database.Repository;
using PushServer.Database.Repository.Interface;
using WDev.AspNetCore.Middleware;
using PushServer.Model.ConfigOptions;

namespace PushServer {
    public class ConfigStartup {
        public static void RegisterConfigureSettingServices( IConfiguration Configuration , ref IServiceCollection services ) {
            //在此設定讀取ConfigSeeting
            
            //MongoDB Setting
            services.Configure<MongoDBSettings>(options =>
            {
                options.ConnectionString 
                    = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database 
                    = Configuration.GetSection("MongoConnection:Database").Value;
            });
            //GooglePushServer Setting 
            services.Configure<GooglePushServerSetting>(options =>
            {
                options.Url 
                    = Configuration.GetSection("GooglePushServer:Url").Value;
            });
        }
    }
}