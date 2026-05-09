using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.StartScoreAttack.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Requests.StartScoreAttack.Logic
{
    public static class StartScoreAttackHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);

            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var req = JsonConvert.DeserializeObject<StartScoreAttackRequest>(requestJsonString!);

            if (req == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }

            var userData = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(req.Level5UserId!, "ywp_user_data");
            if (userData == null)
            {
                await GeneralUtils.SendBadRequest(ctx);
                return;
            }

            if (userData.Hitodama > 0 || userData.FreeHitodama > 0)
            {
                if (userData.Hitodama > 0)
                {
                    userData.Hitodama -= 1;
                }
                else
                {
                    if (userData.FreeHitodama == 5)
                    {
                        userData.HitodamaRecoverSec = 900;
                    }
                    userData.FreeHitodama -= 1;
                }

                var res = new StartScoreAttackResponse(userData);
                
                var requestId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                res.RequestId = requestId;

                res.ScoreAttackId = req.ScoreAttackId ?? 1068;
                res.FreePlayFlg = req.FreePlayFlg ?? 0;

                try
                {
                    var deckData = await DBService.Logic.DBService.GetYwpUserAsync<string>(req.Level5UserId!, "ywp_user_youkai_deck");
                    var youkaiData = await DBService.Logic.DBService.GetYwpUserAsync<string>(req.Level5UserId!, "ywp_user_youkai");

                    if (deckData != null && youkaiData != null)
                    {
                        BuildUserYoukaiList(res, deckData, youkaiData);
                    }

                    await DBService.Logic.DBService.SetYwpUserAsync(req.Level5UserId!, "ywp_user_requestid", requestId);
                    await DBService.Logic.DBService.SetYwpUserAsync(req.Level5UserId!, "ywp_user_data", userData);

                    var resDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res))!;

                    List<string> tables = new()
                    {
                        "ywp_mst_bonus_block_lot",
                        "ywp_user_event",
                        "ywp_user_item", 
                        "ywp_mst_youkai_bonus_effect_exclude",
                        "ywp_user_dictionary",
                        "ywp_mst_score_attack_breed",
                        "ywp_mst_big_boss",
                        "ywp_mst_score_attack_youkai_assist",
                        "enemyYoukaiOrderList",
                        "continueInfoList",
                        "ywp_mst_event",
                        "ywp_mst_youkai_pos_effect_exclude",
                        "ywp_mst_big_boss_effect",
                        "ywp_mst_score_attack",
                        "ywp_mst_game_const"
                    };

                    resDict["enemyYoukaiOrderList"] = BuildEnemyYoukaiOrderList();
                    resDict["continueInfoList"] = BuildContinueInfoList();

                    await GeneralUtils.AddTablesToResponse(tables, resDict!, true, req.Level5UserId!);

                    var marshalledResponse = JsonConvert.SerializeObject(resDict);
                    var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
                    ctx.Response.Headers.ContentType = "application/json";
                    await ctx.Response.WriteAsync(encryptedResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[StartScoreAttack] Error for {req.Level5UserId}: {ex}");
                    await GeneralUtils.SendBadRequest(ctx);
                    return;
                }
            }
            else
            {
                var errorResponse = new MsgBoxResponse("You don't have enough spirit.", "Not Enough Spirit");
                await ctx.Response.WriteAsync(NHNCrypt.Logic.NHNCrypt.EncryptResponse(JsonConvert.SerializeObject(errorResponse)));
            }
        }

        private static void BuildUserYoukaiList(StartScoreAttackResponse response, string deckData, string youkaiData)
        {
            try
            {
                var youkaiParser = new TableParser.Logic.TableParser(youkaiData);
                string[] userDeck = deckData.Split('|');
                var userYoukaiList = new List<Dictionary<string, object>>();

                for (int i = 1; i <= 5 && i < userDeck.Length; i++)
                {
                    var youkaiId = userDeck[i];
                    var yokaiInfoIndex = youkaiParser.FindIndex(new[] { youkaiId });
                    
                    if (yokaiInfoIndex != -1)
                    {
                        var youkaiInfo = new Dictionary<string, object>
                        {
                            ["youkaiId"] = int.Parse(youkaiId),
                            ["skillLv"] = 1,
                            ["sSkillLv"] = 1, 
                            ["hp"] = int.Parse(youkaiParser.Table[yokaiInfoIndex][3]),
                            ["atkPower"] = int.Parse(youkaiParser.Table[yokaiInfoIndex][4])
                        };
                        userYoukaiList.Add(youkaiInfo);
                    }
                }

                response.UserYoukaiList = userYoukaiList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BuildUserYoukaiList] Error: {ex}");
                response.UserYoukaiList = new List<Dictionary<string, object>>();
            }
        }

        private static List<Dictionary<string, object>> BuildEnemyYoukaiOrderList()
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["addTime"] = 0,
                    ["addScore"] = 0,
                    ["enemyYoukaiList"] = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            ["actionTurn"] = 4,
                            ["hp"] = 216,
                            ["atkPower"] = 94,
                            ["enemyId"] = 536800101
                        },
                        new Dictionary<string, object>
                        {
                            ["actionTurn"] = 3,
                            ["hp"] = 333,
                            ["atkPower"] = 80,
                            ["enemyId"] = 536800102
                        },
                        new Dictionary<string, object>
                        {
                            ["actionTurn"] = 5,
                            ["hp"] = 201,
                            ["atkPower"] = 60,
                            ["enemyId"] = 536800103
                        }
                    }
                },
                new Dictionary<string, object>
                {
                    ["addTime"] = 0,
                    ["addScore"] = 0,
                    ["enemyYoukaiList"] = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            ["actionTurn"] = 3,
                            ["hp"] = 389,
                            ["atkPower"] = 168,
                            ["enemyId"] = 536800201
                        },
                        new Dictionary<string, object>
                        {
                            ["actionTurn"] = 5,
                            ["hp"] = 358,
                            ["atkPower"] = 111,
                            ["enemyId"] = 536800202
                        },
                        new Dictionary<string, object>
                        {
                            ["actionTurn"] = 4,
                            ["hp"] = 308,
                            ["atkPower"] = 136,
                            ["enemyId"] = 536800203
                        }
                    }
                },
                new Dictionary<string, object>
                {
                    ["addTime"] = 0,
                    ["addScore"] = 0,
                    ["enemyYoukaiList"] = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            ["actionTurn"] = 9,
                            ["hp"] = 9999999,
                            ["atkPower"] = 234,
                            ["enemyId"] = 536800301
                        }
                    }
                }
            };
        }

        private static List<Dictionary<string, object>> BuildContinueInfoList()
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["continueNum"] = 1,
                    ["needMoney"] = 300,
                    ["addTime"] = 5,
                    ["recoveryValue"] = 100000
                },
                new Dictionary<string, object>
                {
                    ["continueNum"] = 2,
                    ["needMoney"] = 500,
                    ["addTime"] = 5,
                    ["recoveryValue"] = 100000
                }
            };
        }
    }
}