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
            else if (type == ConditionType.FinishWithSpecificYoukaiSoult && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.FinishWithSoult && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.ClearStageNTimes && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinLinkSize && deserialized.LinkSizeMax >= param1)
            {
                return true;
            }
            else if (type == ConditionType.MinSize && deserialized.EraseSizeMax >= param1) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinCombo && deserialized.ComboMax >= param1) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinBonusBalls && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinFeverCount && deserialized.FeverTimeNum >= param1) //idk
            {
                return true;
            }
            else if (type == ConditionType.MaxMilisecondClearTime && deserialized.ClearTimeSec * 1000 >= param1) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinSuccess && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.CompleteStage) //idk
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
            else if (type == ConditionType.MinSMove && false) //idk
            {
                return true;
            }
            else if (type == ConditionType.MinPuniErase && deserialized.EraseNumTotal >= param1) //idk
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
                return true;
            }
            else if (type == ConditionType.UseSpecificYoukaiSoult && false) //idk
            {
                return true;
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
