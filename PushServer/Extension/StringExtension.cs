using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushServer.Extension
{
    public static class StringExtensions
    {
        public static bool isNullOrEmpty(this String str)
        {
            if( str == null )
                return true;
            if( str.Equals("") )
                return true;
            return false;
        }
    }   
}