using Newtonsoft.Json;

namespace Puniemu.Src.Server.L5ID.DataClasses
{
    //This struct is responsible for representing all of the errored out responses from the L5ID API, as they all follow the same format.
    public struct SBadResponse
    {
        //False if an error occured, in our case always false
        [JsonProperty("result")]
        public bool Result { get; set; }
        //Error code
        [JsonProperty("code")]
        public int Code { get; set; }
        //Detailed error message
        [JsonProperty("message")]
        public string Message { get; set; }

        public SBadResponse(L5IDErr code, string? message = null)
        {
            if (message == null)
            {
                Message = code.ToString();
            }
            else
            {
                Message = message;
            }
            Result = false;
            Code = (int)code;
        }
    }
}
