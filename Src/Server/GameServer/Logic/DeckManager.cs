using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;
namespace Puniemu.Src.Server.GameServer.Logic
{
    using BefriendYokaiInfo = (RarityType Rank, int SoultLevel);
    public static class DeckManager
    {

        private static BefriendYokaiInfo GetIsBefriender(long youkaiId, YwpMstYoukai mstYokaiItem, TableParser<YwpUserYoukaiSkill> userSkill)
        {
            var skillObj = MstSkillManager.GetSkillObj(youkaiId);
            if (skillObj == null)
            {
                return (RarityType.RarityNone, 0);
            }
            if (skillObj.SoultType == SoultType.Befriender)
            {
                var skillIdx = userSkill.Items.FindIndex(x => x.YoukaiId == youkaiId);
                if (skillIdx == -1) throw new Exception("Weird issue bad skill shouldnt happen");
                return (mstYokaiItem.YoukaiRarity, userSkill.Items[skillIdx].Level);
            }
            return (RarityType.RarityNone, 0);
        }

        //Rank, soultiamte level
        //Rank is RarityNone if not a befriender
        public static BefriendYokaiInfo[] GetBefrienderSpots(TableParser<YwpUserDeck> deck, YwpMstYoukai mstYokaiItem, TableParser<YwpUserYoukaiSkill> userSkill)
        {
            var currentDeck = deck.Items[0];
            var spots = new BefriendYokaiInfo[5];
            spots[0] = GetIsBefriender(currentDeck.MiddleYoukaiId, mstYokaiItem, userSkill);
            spots[1] = GetIsBefriender(currentDeck.LeftYoukaiId, mstYokaiItem, userSkill);
            spots[2] = GetIsBefriender(currentDeck.RightYoukaiId, mstYokaiItem, userSkill);
            spots[3] = GetIsBefriender(currentDeck.FarLeftYoukaiId, mstYokaiItem, userSkill);
            spots[4] = GetIsBefriender(currentDeck.FarRightYoukaiId, mstYokaiItem, userSkill);

            return spots;
        }
      
    }
}
