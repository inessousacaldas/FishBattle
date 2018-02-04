using LITJson;
using UnityEngine;

namespace CySdk
{
    public class Result
    {
        public int state_code;
        public string message;
        public JsonData data;



        public static Result ToObject(string jsondata)
        {
            
            JsonData jsonData = JsonMapper.ToObject(jsondata);
            return new Result() { state_code = (int)jsonData["state_code"], message = (string)jsonData["message"], data = jsonData["data"]};
        }
    }
}

