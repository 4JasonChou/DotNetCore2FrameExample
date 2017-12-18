namespace PushServer.Model
{
    public class ApiResult
    {
        public ApiResult()
        {
            _resultCode = 0;
            _resultMsg = "";
        }
        public ApiResult(int code, string msg)
        {
            _resultCode = code;
            _resultMsg = msg;
        }
        public int _resultCode { get; set; }
        public string _resultMsg { get; set; }
    }
}