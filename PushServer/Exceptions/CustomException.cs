using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PushServer.Exceptions {
    class CustomException : Exception, ISerializable
    {   
        private int statusCode;
        private string statusMsg;

        public CustomException() : base("show message") { }
        public CustomException(string message) 
            : base(message) { }
        public CustomException(string message, Exception inner) 
            : base(message, inner) { }
         public CustomException(int code , string message, Exception inner) 
            : base(message, inner) { 
                statusCode = code;
                statusMsg = message;
            }
        protected CustomException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public int getStatusCode
        { get { return statusCode; } }
        public string getStatusMsg
        { get { return statusMsg; } }
    }


}