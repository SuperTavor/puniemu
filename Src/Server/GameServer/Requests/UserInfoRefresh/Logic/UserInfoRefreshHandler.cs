using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.UserInfoRefresh.DataClasses;
using Puniemu.Src.Server.GameServer.DataClasses;
using System.Text;
using Puniemu.Src.ConfigManager.Logic;
using Puniemu.Src.Server.GameServer.UserInfoRefresh.DataClasses;
using System;
using System.Collections.Generic;
using System.Reflection;
using Google.Protobuf.WellKnownTypes;
using Puniemu.Src.Server.GameServer.Requests.CreateUser.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GetGdkeyAccounts.DataClasses;
using Puniemu.Src.Server.GameServer.UpdateProfile.DataClasses;
using System.Text.Json.Nodes;
using System.Buffers;

namespace Puniemu.Src.Server.GameServer.Requests.UserInfoRefresh.Logic
{
    public class UserInfoRefreshHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<UserInfoRefreshRequest>(requestJsonString!);
            ctx.Response.ContentType = "application/json";
            // userdata is send by default
            YwpUserData userData;
            UserInfoRefreshResponse userInfoRefreshResponse;
            try
            {
                userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized.Level5UserID, "ywp_user_data");
                userInfoRefreshResponse = new UserInfoRefreshResponse(userData);
                // Convert struct into dict (because the response can have different number of ywp_user table)
                var userInfoRefreshDict = userInfoRefreshResponse.ToDictionary();
                // add requested table in the response
                foreach (var item in deserialized.RequestedTab)
                {
                    try
                    {
                        var tempTable = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<dynamic>(deserialized.Level5UserID, item);
                        userInfoRefreshDict.Add(item, tempTable);
                    }
                    catch
                    {
                        Console.WriteLine(item);
                    }
                }
                var marshalledResponse = JsonConvert.SerializeObject(userInfoRefreshDict);
                var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
                await ctx.Response.WriteAsync(encryptedResponse);
            }
            catch
            {
                var errorResponse = new MsgAndGoBackToTitle("This account doesn't exist", "Authentication Error");
                var marshalledErrorResponse = JsonConvert.SerializeObject(errorResponse);
                var encryptedErrorResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledErrorResponse);
                await ctx.Response.WriteAsync(encryptedErrorResponse);

            
            }
        }
    }
}
