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

namespace PushServer.Controllers
{
    [Route("api/[controller]")]
     public class PushMessageController : Controller
    {
        protected readonly ILogger<PushMessageController> _logger;

        public PushMessageController(ILogger<PushMessageController> logger = null)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            throw new Exception("Self , ERROR");
            return new string[] { "value3", "value4" };
        }

    }
}
