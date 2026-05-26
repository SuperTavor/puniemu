using Puniemu.Src.Server.GameServer.DataClasses;
using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.DataClasses
{
    public class YwpMstYoukai
    {
        public int YoukaiId { get; set; } //0
        public string? YoukaiName { get; set; } //1
        public int YoukaiType { get; set; } // 2
        public RarityType YoukaiRarity { get; set; } //3
        public int YoukaiKind { get; set; } //4
        public int LevelType { get; set; } //5
        public int FoodType { get; set; } //6, see FoodTypeString::to_string
        public int MaxLevel { get; set; } //7
        public int BaseHp { get; set; } //8
        public int MaxHp { get; set; } //9
        public int BaseAtk { get; set; } //10
        public int MaxAtk { get; set; } //11
        public int EvolutionYoukaiId { get; set; } //12
        public int EvolutionLevel { get; set; } //13
        public int DictionaryId { get; set; } // 14
        public string? YoukaiDescription { get; set; } //15
        public string? TextPuzzle { get; set; } //16
        public string? TextGasha { get; set; } // 17
        public string? TextMission { get; set; } //18
        public string? TextGift { get; set; } //19
        public string? UnusedName { get; set; }//20, not sure
        public int SkillEffectColorR { get; set; } // 21
        public int SkillEffectColorG { get; set; } // 22
        public int SkillEffectColorB { get; set; } // 23
        public int ScaleBattleFriend { get; set; } //24
        public int ScaleBattleEnemy { get; set; } // 25, not sure
        public int YoukaiSize { get; set; } //26 used to know if youkai is bigSize (== 999)
        //these ids are bigSize harcoded 0x1141f8, 0x114202, 0x122c58, 0x13da12, 0x4c4c65, 0x4c4de4, 0x895506, 0x8956e2
        public int Width { get; set; } // 27, not sure
        public int Height { get; set; } // 28, not sure
        public int X { get; set; } // 29, not sure
        public int Y { get; set; } // 30, not sure
        public string? ReadingName { get; set; } //31
        public int FriendOffsetX { get; set; } // 32
        public int FriendOffsetY { get; set; } //33
        public int EffectType { get; set; } // 34, not sure
        public string? OpenDt { get; set; } //35
        public string? YoukaiEffectBack { get; set; } //36
        public string? YoukaiEffectFront { get; set; } //37
        public int ScaleOffsetDeck { get; set; } // 38
    }
}

//BreedType is derived from KindType

//youkai type, 3 == isBoss, 5 == isBigBoss

//youkai rarity, 0 == NoneRarity up to uzp (UZ+) (14 value)

//skillTexturePath derived from YoukaiId

//rankTexturePath derived from YoukaiRarity