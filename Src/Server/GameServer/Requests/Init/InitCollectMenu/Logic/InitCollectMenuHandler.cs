using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitCollectMenu.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using System.Buffers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;


namespace Puniemu.Src.Server.GameServer.Requests.InitCollectMenu.Logic
{
    public class InitCollectMenuHandler
    {

        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var deserialized = JsonConvert.DeserializeObject<InitCollectMenuRequest>(requestJsonString!);
            var userdata = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(deserialized!.Level5UserId!, "ywp_user_data");

            var YoukaiCollectRewardMst = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_collect_reward"]!)!["tableData"]);
            var YoukaiIntroMst = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_intro"]!)!["tableData"]);
            var YoukaiCollectMst = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_collect"]!)!["tableData"]);
            var YoukaiCollectEffectMst = new TableParser.Logic.TableParser(JsonConvert.DeserializeObject<Dictionary<string, string>>(DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_youkai_collect_effect"]!)!["tableData"]);
            
            var YoukaiCollectRewardRes = new TableParser.Logic.TableParser("");
            var YoukaiIntroRes = new TableParser.Logic.TableParser("");
            var YoukaiCollectRes = new TableParser.Logic.TableParser("");
            var YoukaiCollectEffectRes = new TableParser.Logic.TableParser("");

            var UserYoukaiCollect = await DBService.Logic.DBService.GetYwpUserAsync<List<YokaiCollectEntry>>(deserialized!.Level5UserId!, "ywp_user_youkai_collect");
            var UserYoukaiIntro = await DBService.Logic.DBService.GetYwpUserAsync<List<YokaiIntroEntry>>(deserialized!.Level5UserId!, "ywp_user_youkai_intro");
            List<YokaiCollectEntry> res_yokai_collect_entry = new();
            List<YokaiIntroEntry> res_yokai_intro_entry = new();
            bool found = false;
            foreach (YokaiCollectEntry idx in UserYoukaiCollect!)
            {
                if (idx.CollectId == deserialized.CollectId)
                {
                    res_yokai_collect_entry.Add(idx);
                    found = true;
                }
            }
            if (!found)
            {
                YokaiCollectEntry tmp = new();
                tmp.CollectId = deserialized.CollectId;
                tmp.CollectCnt = 0;
                tmp.MenuIdList = "";
                UserYoukaiCollect.Add(tmp);
                res_yokai_collect_entry.Add(tmp);
            }
            int[] YoukaiCollectRewardIdx = GetTableIndex.GetIndexs(YoukaiCollectRewardMst, new List<Tuple<int, string>> { Tuple.Create(0, deserialized.CollectId.ToString())});
            foreach (var idx in YoukaiCollectRewardIdx)
            {
                YoukaiCollectRewardRes.AddRow(YoukaiCollectRewardMst.Table[idx]);
            }
            int[] YoukaiCollectEffectIdx = GetTableIndex.GetIndexs(YoukaiCollectEffectMst, new List<Tuple<int, string>> { Tuple.Create(0, deserialized.CollectId.ToString())});
            foreach (var idx in YoukaiCollectRewardIdx)
            {
                YoukaiCollectEffectRes.AddRow(YoukaiCollectEffectMst.Table[idx]);
            }
            int[] YoukaiCollectIdx = GetTableIndex.GetIndexs(YoukaiCollectMst, new List<Tuple<int, string>> { Tuple.Create(0, deserialized.CollectId.ToString())});
            foreach (var idx in YoukaiCollectIdx)
            {
                found = false;
                YokaiIntroEntry entry = new();
                foreach (YokaiIntroEntry idx2 in UserYoukaiIntro!)
                {
                    if (idx2.IntroId == int.Parse(YoukaiCollectMst.Table[idx][3]))
                    {
                        res_yokai_intro_entry.Add(idx2);
                        found = true;
                    }
                }
                if (!found)
                {
                    entry.UserId = userdata!.UserID;
                    entry.IntroId = int.Parse(YoukaiCollectMst.Table[idx][3]);
                }
                var Progress = new TableParser.Logic.TableParser("");
                YoukaiCollectRes.AddRow(YoukaiCollectMst.Table[idx]);
                
                int[] YoukaiIntroIdx = GetTableIndex.GetIndexs(YoukaiIntroMst, new List<Tuple<int, string>> { Tuple.Create(0, YoukaiCollectMst.Table[idx][3])});
                foreach (var idx2 in YoukaiIntroIdx)
                {
                    YoukaiIntroRes.AddRow(YoukaiIntroMst.Table[idx2]);
                    if (!found)
                    {
                        string spe = "0";
                        if (string.IsNullOrEmpty(YoukaiIntroMst.Table[idx2][3])){
                            spe = "1";
                        }
                        Progress.AddRow([YoukaiIntroMst.Table[idx2][1], spe, "0"]);
                    }
                }
                if (!found)
                {
                    entry.Progress = Progress.ToString();
                    res_yokai_intro_entry.Add(entry);
                    UserYoukaiIntro.Add(entry);
                }
            }


            var res = new InitCollectMenuResponse(userdata!);
            res.TruncateItemList = null; //idk
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res))!;
            resdict["ywp_mst_youkai_collect_reward"] = YoukaiCollectRewardRes.ToString();
            resdict["ywp_mst_youkai_intro"] = YoukaiIntroRes.ToString();
            resdict["ywp_mst_youkai_collect"] = YoukaiCollectRes.ToString();
            resdict["ywp_mst_youkai_collect_effect"] = YoukaiCollectEffectRes.ToString();
            resdict["ywp_user_youkai_collect"] = res_yokai_collect_entry;
            resdict["ywp_user_youkai_intro"] = res_yokai_intro_entry;
            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_youkai_collect", UserYoukaiCollect!);
            await DBService.Logic.DBService.SetYwpUserAsync(deserialized!.Level5UserId!, "ywp_user_youkai_intro", UserYoukaiIntro!);
            await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(resdict)));
        }
    }
}
