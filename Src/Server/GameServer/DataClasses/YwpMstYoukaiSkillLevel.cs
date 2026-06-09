namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpMstYoukaiSkillLevel
    {
        public long YoukaiID { get; set; }

        public int SoultLevel { get; set; }

        public string DisplayName { get; set; }

        public int Unk0 { get; set; } //Could be related to how fast the skill gauge gets filled

        public int SoultPt { get; set; } //Might be different from the actual ingame points, for example for befrienders its SoultPt / 187.5 for the display points

        public int MaxUseCount { get; set; } //Maybe

        public int Unk1 { get; set; }

        public int Unk2 { get; set; }

        public int Unk3 { get; set; }

        public int Unk4 { get; set; }

        public int Unk5 { get; set; }

        public string SoultStatsDict { get; set; } //Contains the soult points with variable names if there are multiple

        public double Unk7 { get; set; }


        public int GetBefrienderPt()
        {
            if (string.IsNullOrEmpty(SoultStatsDict)) return SoultPt;
            else
            {
                return int.Parse(DictParse(SoultStatsDict)["friendlyUpProb"]);
            }
        }

        public static Dictionary<string, string> DictParse(string input)
        {
            var dict = new Dictionary<string, string>();

            var pairs = input.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var kv = pair.Split(':', 2, StringSplitOptions.TrimEntries);
                if (kv.Length == 2)
                {
                    dict[kv[0]] = kv[1];
                }
            }

            return dict;
        }
    }
}
