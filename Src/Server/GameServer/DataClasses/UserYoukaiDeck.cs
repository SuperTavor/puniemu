using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class UserYoukaiDeck : TableParser.Logic.TableParser
    {
        private readonly int[] EXCLUDE_INDICES = [
            0,//unk
            6,//WatchID?
            7//unk
        ];
        public List<int> YoukaiInDeck { get; set; }

        public UserYoukaiDeck(string src) : base(src)
        {
            //Remove the watch ID and the 2 unknown values
            YoukaiInDeck = this.Table.Select(x =>
                    int.Parse(x.FirstOrDefault()!))
                    .ToList();
        }

        public override void PrepareForToString()
        {
            for(int i = 0; i  < YoukaiInDeck.Count; i++)
            {
                if (EXCLUDE_INDICES.Contains(i)) return;
                Table[i] = 
                [ 
                    YoukaiInDeck[i].ToString() 
                ];
            }
        }
    }
}
