using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.Requests.BuyHitodama.DataClasses;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public enum ConditionType
    {
        MinScore = 1,
        UsedYoukai = 2,
        MaxClearTime = 3,
        MaxPuniErase = 4,

        FinishWithSpecificYoukaiSoult = 6,

        FinishWithSoult = 8,
        ClearStageNTimes = 9,
        MinLinkSize = 10,
        MinSize = 11,

        MinCombo = 14,
        MinBonusBalls = 15,
        MinFeverCount = 16,
        MaxMilisecondClearTime = 17,
        MinSuccess = 18,
        CompleteStage = 19,
        ClearRankOnly = 20,
        ClearKindOnly = 21,
        ClearWithoutContinue = 22,
        ClearWithoutHPRefill = 23,
        MinSMove = 24,
        MinPuniErase = 25,

        MinHpRate = 27,
        MaxEnnemyAttackCount = 28,
        MinDamageScoreWithKind = 29,
        ClearMaxRank = 30,
        MinSpecificYoukaiErase = 31,
        MinSpecificYoukaiLink = 32,
        UseSpecificYoukaiSoult = 33,
        MinSpecificYoukaiSize = 34,
        ClearWithOnlyFemalePuni = 35,

    }
}
