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
using PushServer.Extension;
using PushServer.Exceptions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Cryptography;
using PushServer.Model.ConfigOptions;
using Microsoft.Extensions.Options;
using PushServer.Helpers;

namespace PushServer.Logistic {
    public class PushMessageLogic : IPushMessageLogic {
        protected readonly ILogger<PushMessageLogic> _logger;
        protected readonly IOptions<GooglePushServerSetting> _settings;
        private readonly IFPS_PushHistoryRepository _pushHistoryRepository;
        private SerialAndEncryption _serialAndEncryption;
        public PushMessageLogic(ILogger<PushMessageLogic> logger = null 
        , IFPS_PushHistoryRepository pushHistoryRepository = null 
        , IOptions<GooglePushServerSetting> settings = null 
        , SerialAndEncryption serialAndEncryption = null )
        {
            _logger = logger;
            _pushHistoryRepository = pushHistoryRepository;
            _settings = settings;
            _serialAndEncryption = serialAndEncryption;
        }

        // 查詢推播訊息
        public async Task<IEnumerable<PushMessage>> GetPushMessageHistory(string messageSN , string dateStart , string dateEnd) {
            var pushHistorys = await _pushHistoryRepository.GetAllFPS_PushHistorys();
            if (!string.IsNullOrEmpty(messageSN))
            {
                if(!"ALL".Equals(messageSN.ToUpper()))
                    pushHistorys = pushHistorys.Where(x => messageSN.Equals(x.MessageSN));
            }
            if (VerifySearchDateRange(ref dateStart, ref dateEnd))
            {
                var ds = DateTime.Parse(dateStart);
                var de = DateTime.Parse(dateEnd);
                pushHistorys = pushHistorys.Where(x => x.PushTime >= ds && x.PushTime <= de );
            }
            return Convert_FPS_PushHistory_To_PushMessage(pushHistorys.ToList());
        }

        // 發送推播訊息
        public async Task<PushMessage> PushAppMessage(PushMessage value) {
            var googleRespone = await SendPushMessageToGoogle(value);
            
            GoogleFCMResponseModel response = null;
            if ( 200.Equals(googleRespone._resultCode) )
            {
                try
                {
                    response = JsonConvert.DeserializeObject<GoogleFCMResponseModel>(googleRespone._resultMsg);
                }
                catch (Exception ex)
                {
                    ThrowCustomException.Exception550("",ex);
                }
            }
            else
            {
                string errMsg = "GoogleServer發生錯誤";
                if (400.Equals(googleRespone._resultCode))
                    ThrowCustomException.Exception451("JsonObject發生錯誤");
                else if (401.Equals(googleRespone._resultCode))
                    ThrowCustomException.Exception453(); // ( Authorized不存在 )
                else
                    errMsg = googleRespone._resultMsg;

                ThrowCustomException.Exception550(errMsg,null);
            }

            //紀錄推播結果
            value = await RecordPushMessageResult(response,value);
            
            return value;
        }
  
        private async Task<ApiResult> SendPushMessageToGoogle(PushMessage value) {
            ApiResult _result = new ApiResult();
            GoogleFCMRequestModel requestModel = new GoogleFCMRequestModel()
            {
                to = value.TargetDeviceID,
                data = value.Data,
                notification = value.Notification
            };
            
            string FCMData = Newtonsoft.Json.JsonConvert.SerializeObject(requestModel);
            string FCMAuth = "key=" + value.FCMAuth;
            string FCMUrl = _settings.Value.Url;

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", FCMAuth);
                    var content = new StringContent(FCMData, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(FCMUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    _result._resultCode = (int)response.StatusCode;
                    _result._resultMsg = responseString;
                }
                catch (Exception ex)
                {
                    _result._resultMsg = ex.ToString();
                }
            }
            return _result;
        }
        private async Task<PushMessage> RecordPushMessageResult(GoogleFCMResponseModel result, PushMessage pushValue) {

            FPS_PushHistory pushMessage = new FPS_PushHistory();

            pushMessage.MessageSN = DateTime.Now.ToString("yyyyMMddhhmmss") + _serialAndEncryption.CreateUniqueSN();
            pushMessage.DeviceID = pushValue.TargetDeviceID;
            pushMessage.Message_Data = pushValue.Data.isNull()? "" : pushValue.Data.ToString();
            pushMessage.Message_Noti = pushValue.Notification.isNull()? "" : pushValue.Notification.ToString();
            //目前僅使用單Device推播
            pushMessage.Status = 1.Equals(result.success) ? "S" : "F";
            pushMessage.Error  = 1.Equals(result.success) ? "" : ( result.results.FirstOrDefault().error) ;
            pushMessage.FCMAuth = pushValue.FCMAuth;
            pushMessage.PushID = result.multicast_id.ToString();
            pushMessage.PushTime =  DateTime.Now;
            var addRes = await _pushHistoryRepository.AddFPS_PushHistory(pushMessage);
  
            PushMessage apiBack = new PushMessage();
            apiBack.MessageSN = pushMessage.MessageSN;
            apiBack.Status = pushMessage.Status;
            apiBack.ErrorMsg = pushMessage.Error;
            apiBack.DataTime = pushMessage.PushTime.ToString("yyyy/MM/dd HH:mm:ss");

            return apiBack;

        }

        private PushMessage Convert_FPS_PushHistory_To_PushMessage(FPS_PushHistory org) {
            var res = new PushMessage();
            res.MessageSN = org.MessageSN;
            res.FCMAuth = org.FCMAuth;
            res.TargetDeviceID = org.DeviceID;
            res.Data = org.Message_Data;
            res.Notification = org.Message_Noti;
            res.DataTime = org.PushTime.ToString("yyyy/MM/dd HH:mm:ss");
            res.Status = org.Status;
            res.ErrorMsg = org.Error;
            return res;
        }
        private List<PushMessage> Convert_FPS_PushHistory_To_PushMessage(List<FPS_PushHistory> org) {
            var resList = new List<PushMessage>();

            foreach(FPS_PushHistory one in org){
                resList.Add( Convert_FPS_PushHistory_To_PushMessage(one) );
            }
            return resList;
        }
        private bool VerifySearchDateRange(ref string dateStart, ref string dateEnd) {
            // true : 執行查詢時間區間 , false : 不執行 , ThrowException : 查詢條件錯誤

            if ( string.IsNullOrEmpty(dateStart) && string.IsNullOrEmpty(dateEnd) ) return false;

            if (!string.IsNullOrEmpty(dateStart) && !string.IsNullOrEmpty(dateEnd)) { }
            else ThrowCustomException.Exception452("查詢時間起訖");
            if (dateStart.Length == 10 && dateEnd.Length == 10) { } //不做事
            else ThrowCustomException.Exception451("查詢時間");

            dateStart = MakeDateComplete(dateStart);
            dateEnd = MakeDateComplete(dateEnd);

            try
            {
                if(DateTime.Parse(dateStart)>=DateTime.Parse(dateEnd))
                    ThrowCustomException.Exception451("查詢時間起訖");
            }
            catch
            {
                ThrowCustomException.Exception451("查詢時間");
            }

            return true;
        }
        private string MakeDateComplete(string date) {
            date = date.Insert(4, "/");
            date = date.Insert(7, "/");
            if ("24".Equals(date.Substring(10, 2)))
                date = date.Substring(0, 10) + " 23:59:59";
            else
            {
                date = date.Substring(0, 10) + " " + date.Substring(10, 2) + ":00:00";
            }
            return date;
        }

        // 一些只有該邏輯會用到的 Model
        #region GoogleFirebaseCloudMessageModel
        private class GoogleFCMRequestModel
        {
            public string to { get; set; }
            public object data { get; set; }
            public object notification { get; set; }
        }
        private class GoogleFCMResponseModel
        {
            public long multicast_id { get; set; }
            public int success { get; set; }
            public int failure { get; set; }
            public int canonical_ids { get; set; }
            public GoogleFCMResponseModelInnerResult[] results { get; set; }
        }
        private class GoogleFCMResponseModelInnerResult
        {
            public string message_id { get; set; }
            public string error { get; set; }
        }
        #endregion

    }
}