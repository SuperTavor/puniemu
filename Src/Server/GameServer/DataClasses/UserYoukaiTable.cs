namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class UserYoukaiTable: TableParser.Logic.TableParser
    {
        public List<UserYoukaiItem> Youkai = new();
        
        public UserYoukaiTable(string src) : base(src)
        {
            foreach(var list in Table)
            {
                var item = new UserYoukaiItem()
                {
                    YoukaiId = int.Parse(list[0]),
                    SkillLevel = int.Parse(list[1]),
                    SSkillLevel = int.Parse(list[2]),
                    Hp = int.Parse(list[3]),
                    AttackPower = int.Parse(list[4]),
                    BefriendTimestamp = long.Parse(list[9]),
                };
                Youkai!.Add(item);
            }
        }

        public override void PrepareForToString()
        {
            for(int i = 0; i < Youkai.Count; i++)
            {
                var tableItemRef = Table[i];
                var yokai = Youkai[i];
                //Set all the values back
                tableItemRef[0] = yokai.YoukaiId.ToString();
                tableItemRef[1] = yokai.SkillLevel.ToString();
                tableItemRef[2] = yokai.SSkillLevel.ToString();
                tableItemRef[3] = yokai.Hp.ToString();
                tableItemRef[4] = yokai.AttackPower.ToString();
                tableItemRef[9] = yokai.BefriendTimestamp.ToString();
            }
        }
    }
}
