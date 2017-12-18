namespace PushServer.Model
{
    public class PushMessage
    {
        public string MessageSN {get;set;}
        public string FCMAuth {get;set;}
        public string TargetDeviceID {get;set;}
        public object Data{get;set;}
        public object Notification{get;set;}
        public string DataTime{get;set;}
        public string Status{get;set;}
        public string ErrorMsg{get;set;}

    }
}