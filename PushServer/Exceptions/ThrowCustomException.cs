using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PushServer.Exceptions {
    public class ThrowCustomException {
        public static void Exception451(string info){
            throw new CustomException(451,String.Format("資料格式錯誤。({0}欄位格式有誤!!)",info),null);
        }
        public static void Exception452(string info){
            throw new CustomException(452,String.Format("必要資訊未提供。({0}欄位未提供!!)",info),null);
        }
        public static void Exception453(){
            throw new CustomException(453,"FCMAuth已失效或不存在。",null);
        }

        public static void Exception550(string info,Exception ex){
            throw new CustomException(550,"PushServer內部發生錯誤，執行任務未完成。",ex);
        }
    }
}