using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.DeckEdit.DataClasses;
using System;
using System.Buffers;
using System.Text;
namespace Puniemu.Src.Server.GameServer.Requests.DeckEdit.Logic
{
    public static class DeckEditHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<DeckEditRequest>(requestJsonString!);

            // tutorial handling
            var tutorialList = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_tutorial_list");
            var tutorialListTable = new TableParser.Logic.TableParser(tutorialList!);

            if (TutorialFlagManager.GetStatus(tutorialListTable, 1000) == 6)
            {
                tutorialListTable = TutorialFlagManager.EditTutorialFlg(tutorialListTable, 1, 1000, 7);
            }
            if (TutorialFlagManager.GetStatus(tutorialListTable, 1) == 0)
            {
                tutorialListTable = TutorialFlagManager.EditTutorialFlg(tutorialListTable, 2, 1, 1);
            }

            // Get Watch Id
            var userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(deserialized!.Gdkey!, "ywp_user_data");

            //Construct response
            var res = new DeckEditResponse();
            var resdict = await res.ToDictionary();

            // Get and edit deck data
            var UserDeck = new TableParser.Logic.TableParser((await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(deserialized!.Gdkey!, "ywp_user_youkai_deck"))!);

            var Counter = 0;
            foreach (var item in deserialized!.YoukaiIdList!) {
                Counter += 1;
                var Id = item["youkaiId"].ToString();
                UserDeck.Table[0][Counter] = Id;
            }

            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_youkai_deck", UserDeck.ToString());

            resdict["ywp_user_youkai_deck"] = UserDeck.ToString();
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(deserialized!.Gdkey!, "ywp_user_tutorial_list", tutorialListTable.ToString());
            resdict["ywp_user_tutorial_list"] = tutorialListTable.ToString();
            resdict["ywp_user_data"] = userData!;
            foreach (var table in Consts.DECK_EDIT_TABLES)
            {
                string? tableText = null!;
                if (table.StartsWith("ywp_user"))
                {
                    var gotten = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<object>(deserialized!.Gdkey!, table);
                    tableText = JsonConvert.SerializeObject(gotten);
                }
                else tableText = DataManager.Logic.DataManager.GameDataManager!.GamedataCache[table];
                //if we can't deserialize json it means it's not a json and we store it as is
                object tableObj = new();
                try
                {
                    tableObj = JsonConvert.DeserializeObject<object>(tableText)!;
                    if (tableObj is JObject)
                    {
                        var jObject_temp = (JObject)tableObj;
                        if (jObject_temp.ContainsKey("data"))
                        {
                            tableObj = jObject_temp["data"]!;
                        }
                        else if (jObject_temp.ContainsKey("tableData"))
                        {
                            tableObj = jObject_temp["tableData"]!;
                        }

                    }

                }
                catch
                {
                    tableObj = tableText;
                }
                if (tableObj == null)
                {
                    tableObj = new List<object>();
                }
                resdict[table] = tableObj;

            }
            var encryptedRes = NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict));
            ctx.Response.Headers.ContentType = "application/json";
            await ctx.Response.WriteAsync(encryptedRes);
        }
    }
}
