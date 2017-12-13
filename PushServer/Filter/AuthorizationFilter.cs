using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PushServer.Extension;
using PushServer.Exceptions;

namespace PushServer.Filter {
    public class AuthorizationFilter  :  IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            String auth =  context.HttpContext.Request.Headers["Authorization"];
            if( auth.isNullOrEmpty() ) {
                throw new CustomException(403,"Authorization is error.",null);
            }
        }
    }
}