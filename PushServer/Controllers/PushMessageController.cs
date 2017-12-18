using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Newtonsoft.Json;
using PushServer.Filter;
using PushServer.Database;
using PushServer.Database.DBModels;
using PushServer.Database.Repository.Interface;
using PushServer.Model;
using PushServer.Logistic;
using PushServer.Extension;
using PushServer.Exceptions;

namespace PushServer.Controllers
{
    [Route("api/[controller]")]
     public class PushMessageController : Controller
    {
        private IPushMessageLogic _pushMessageLogic;
        public PushMessageController( IPushMessageLogic pushMessageLogic = null )
        {
            _pushMessageLogic = pushMessageLogic;
        }


        // api/PushServer [ GET : 查詢推播 ] , [ POST : 發送推播 ]
        // [TypeFilter(typeof(AuthorizationFilter))] , Authorization檢測TAG

        [HttpGet("{messageSN?}/{dateStart?}/{dateEnd?}")]
        public Task<IEnumerable<PushMessage>> Get(string messageSN ,string dateStart,string dateEnd )
        {
            return _pushMessageLogic.GetPushMessageHistory(messageSN,dateStart,dateEnd);
        }
        
        [HttpPost]
        public Task<PushMessage> Post( [FromBody] PushMessage req )
        {
            return _pushMessageLogic.PushAppMessage(req);
        }
    }
}
