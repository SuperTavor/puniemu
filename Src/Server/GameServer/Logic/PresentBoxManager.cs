using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitGacha.DataClasses;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic
{
    public class PresentBoxManager
    {
        public static PresentBoxList CreatePresent(string? recv_userId, bool all_recieve, bool bonus, YwpUserData userdata, int rewardId, RewardType rewardType, int itemCnt, string bodyText)
        {
            PresentBoxList present = new PresentBoxList();
            present.BonusLimitMsg = "";
            present.BonusYmoney = 0;
            present.BodyText = bodyText!;
            if (bonus)
            {
                present.BonusLimitMsg = "1 jour";
                present.BonusYmoney = 10;
            }
            present.DistItemCnt = itemCnt;
            present.DistItemId = rewardId;
            present.Seq = (long)(Random.Shared.NextDouble() * 1_000_000_000_000L);

            present.CanReceiveAll = all_recieve ? 1 : 0;

            if (recv_userId == null) {
                present.MsgType = 200;
                present.TargetTitleId = 0;
                present.TargetIconId = 0;
                present.TargetUserId = "";
                present.TargetPlayerName = "";
                present.IconType = 2;
                present.UserId = userdata.UserID;

            }
            else
            {
                present.UserId = recv_userId;
                present.TargetUserId = userdata.UserID;
                present.IconType = 1;
                if (all_recieve)
                    present.MsgType = 10;
                else
                    present.MsgType = 60;
            }


            //idk
            present.DistItemResource = "";
            present.LayerName = "";
            present.DistItemName = "";


            return present;
        }

    }
}
