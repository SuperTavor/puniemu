using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    //Add converter so it works on both WibWob and Puni versions
    internal class TutorialListConverter : JsonConverter<TutorialList>
    {
        public override TutorialList? ReadJson(JsonReader reader, Type objectType, TutorialList? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JToken.Load(reader);
            //WibWob
            if(obj.Type == JTokenType.Array)
            {
                var list = new TutorialList();
                list.Entries = obj.ToObject<List<TutorialEntry>>(serializer) ?? throw new InvalidDataException("Bad WibWob tutorial list array");
                return list;
            }
            //for puni
            else if(obj.Type == JTokenType.String)
            {
                var tblPrsr = new TableParser<TutorialEntry>(obj.ToObject<string>()!);
                var list = new TutorialList();
                list.Entries = tblPrsr.Items;
                return list;
            }
            else
            {
                throw new InvalidDataException("tutorial list is in wrong format");
            }
        }

        public override void WriteJson(JsonWriter writer, TutorialList? value, JsonSerializer serializer)
        {  
            if (DataManager.Logic.DataManager.IsWibWob)
            {
                serializer.Serialize(writer, value.Entries);
            }
            else
            {
                var tblParser = new TableParser<TutorialEntry>();

                foreach (var entry in value.Entries)
                {
                    tblParser.AddItem(entry);
                }

                writer.WriteValue(tblParser.ToString());
            }
        }
    }

    [JsonConverter(typeof(TutorialListConverter))]
    public class TutorialList
    {
        public List<TutorialEntry> Entries { get; set; }

        public TutorialList()
        {
            Entries = [];
        }

        public void EditTutorialFlg(int TutorialType, long TutorialId, int TutorialValue)
        {
            int index = GetTutorialFlgIndex(TutorialId, TutorialType);
            if (index == -1)
            {
                this.Entries.Add(new TutorialEntry { TutorialId = TutorialId, TutorialType = TutorialType, TutorialStatus = TutorialValue });
                return;
            }
            else
            {
                this.Entries[index].TutorialStatus = TutorialValue;
                this.Entries[index].TutorialType = TutorialType;
            }
        }
        public int GetTutorialFlgIndex(long TutorialId, int TutorialType)
        {
            int count = 0;
            foreach (TutorialEntry entry in this.Entries)
            {
                if (entry.TutorialId == TutorialId && entry.TutorialType == TutorialType)
                {
                    return count;
                }
                count += 1;
            }
            return -1;
        }
        public int GetStatus(long tutorialId, int tutorialType)
        {
            int index = GetTutorialFlgIndex(tutorialId, tutorialType);
            if (index == -1)
            {
                return -1;
            }
            return this.Entries[index].TutorialStatus;
        }
    }
}
