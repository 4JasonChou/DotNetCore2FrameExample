using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using PushServer.Filter;
using PushServer.Database;
using PushServer.Database.DBModels;
using PushServer.Database.Repository.Interface;
using PushServer.Model;
using Newtonsoft.Json;
using PushServer.Logistic;

namespace PushServer.Logistic {
    public interface IPushMessageLogic{

        Task<IEnumerable<PushMessage>> GetPushMessageHistory(string messageSN , string dateStart , string dateEnd);
        Task<PushMessage> PushAppMessage(PushMessage value);
    }
}