using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushServer.Extension
{
    public static class ObjectExtensions
    {
        public static bool isNull(this Object obj)
        {
            if (obj == null) return true;
            else return false;
        }
    }  
}