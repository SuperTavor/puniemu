using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.GameEnd.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Transactions;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class ConditionManager
    {
        private static bool IsFinishWithSoult(List<EnemyYoukaiResultList> enemies, long youkaiId = -1)
        {
            //Find enemy that died last
            int biggest = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                var item = enemies[i];
                if (item.DeadEndOrder > biggest) biggest = item.DeadEndOrder;
            }
            //Check if ended with soult and if it was specific to teh youkai
            var lastItem = enemies.FirstOrDefault(x => x.DeadEndOrder == biggest);
            if(youkaiId == -1 && lastItem.DeadEndType != 0)
            {
                return true;
            }
            else if(lastItem.DeadEndType == youkaiId)
            {
                return true;
            }
            else return false;
        }
        public static bool ComputeStageCondition(ConditionType type, GameEndRequest deserialized, long param1, long param2, long param3)
        {
            if (type == ConditionType.MinScore && deserialized.Score >= param1)
            {
                return true;
            }
            else if (type == ConditionType.UsedYoukai)
            {
                foreach (UserYoukaiResultListReq item in deserialized.UserYoukaiResultList!)
                {
                    if (item.YoukaiId == param1)
                    {
                        return true;
                    }
                }
            }
            else if (type == ConditionType.MaxClearTime && deserialized.ClearTimeSec <= param1)
            {
                return true;
            }
            else if (type == ConditionType.MaxPuniErase && deserialized.EraseNumTotal <= param1)
            {
                return true;
            }
            else if (type == ConditionType.FinishWithSpecificYoukaiSoult) 
            {
                return IsFinishWithSoult(deserialized.EnemyYoukaiResultList, youkaiId: param1);
            }
            else if (type == ConditionType.FinishWithSoult) 
            {
                // Youkai ID specified as -1 to say that the soult check is generic and not specific
                return IsFinishWithSoult(deserialized.EnemyYoukaiResultList, youkaiId: -1);
            }
            else if (type == ConditionType.ClearStageNTimes && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinLinkSize && deserialized.LinkSizeMax >= param1)
            {
                return true;
            }
            else if (type == ConditionType.MinSize && deserialized.EraseSizeMax >= param1) 
            {
                return true;
            }
            else if (type == ConditionType.MinCombo && deserialized.ComboMax >= param1) 
            {
                return true;
            }
            else if (type == ConditionType.MinBonusBalls && deserialized.BonusBlockNum >= param1) 
            {
                return true;
            }
            else if (type == ConditionType.MinFeverCount && deserialized.FeverTimeNum >= param1) 
            {
                return true;
            }
            else if (type == ConditionType.MaxMilisecondClearTime && deserialized.ClearTimeSec * 1000 >= param1) 
            {
                return true;
            }
            else if (type == ConditionType.MinSuccess && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.CompleteStage) 
            {
                return true;
            }
            else if (type == ConditionType.ClearRankOnly && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.ClearKindOnly && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.ClearWithoutContinue && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.ClearWithoutHPRefill && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinSMove && false) 
            {
                int totalSMove = 0;
                foreach(var kai in deserialized.UserYoukaiResultList)
                {
                    totalSMove += kai.SkillUseNum;
                }

                return totalSMove >= param1;
            }
            else if (type == ConditionType.MinPuniErase && deserialized.EraseNumTotal >= param1) 
            {
                return true;
            }
            else if (type == ConditionType.MinHpRate && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MaxEnnemyAttackCount && deserialized.ResultRecivedAttackNum <= param1) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinDamageScoreWithKind && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.ClearMaxRank && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinSpecificYoukaiErase && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinSpecificYoukaiLink && false) //idk
            {
             
            }
            else if (type == ConditionType.UseSpecificYoukaiSoult) 
            {
                foreach(var kai in deserialized.UserYoukaiResultList)
                {
                    if (kai.YoukaiId == param1 && kai.SkillUseNum > 0) return true;
                }
                return false;
            }
            else if (type == ConditionType.MinSpecificYoukaiSize && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.ClearWithOnlyFemalePuni && false) //idk
            {
                return true;
            }
            return false;
        }
    }
}
