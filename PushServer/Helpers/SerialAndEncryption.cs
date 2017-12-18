using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PushServer.Helpers {
    public class SerialAndEncryption
    {
        public string ToMD5String(string value)
        {
            byte[] Original = Encoding.Default.GetBytes(value);
            MD5 key = MD5.Create(); //使用MD5 
            byte[] Change = key.ComputeHash(Original);//進行加密 
            string res = Convert.ToBase64String(Change);
            return res;
        }
        public int CreateUniqueSN()
        {
            Random counter = new Random(Guid.NewGuid().GetHashCode());
            int sn = counter.Next(10000, 99999);
            return sn;
        }
    }
}