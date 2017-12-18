using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushServer.Database.DBModels
{
    public class FPS_PushHistory
    {
        public ObjectId _id {get;set;}
        public string MessageSN  {get;set;}
        public string FCMAuth  {get;set;}
        public string DeviceID  {get;set;}
        public string Message_Data  {get;set;}
        public string Message_Noti  {get;set;}
        public string Status  {get;set;}
        public string Error  {get;set;}

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PushTime { get; set; }
        public string PushID  {get;set;}
    } 
}