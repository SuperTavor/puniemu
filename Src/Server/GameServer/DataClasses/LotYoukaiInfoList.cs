using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class LotYoukaiInfo
    {
        [JsonProperty("lotPattern")]
        public string LotPattern { get; set; }

        [JsonProperty("lotResult")]
        public string LotResult { get; set; }
    }

    internal class LotYoukaiInfoListConverter : JsonConverter<LotYoukaiInfoList>
    {
        public override LotYoukaiInfoList? ReadJson(JsonReader reader, Type objectType, LotYoukaiInfoList? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                //def behavior for wibwob
                var normal = token.ToObject<LotYoukaiInfoList>();
                return normal;
            }
            else if (token.Type == JTokenType.String)
            {
                var tbl = new TableParser.Logic.TableParser<LotYoukaiInfo>(token.ToObject<string>());
                return new LotYoukaiInfoList() { Entries = tbl.Items };
            }
            else throw new InvalidDataException("Bad lotYoukaiInfoList");
        }

        public override void WriteJson(JsonWriter writer, LotYoukaiInfoList? value, JsonSerializer serializer)
        {
            if (DataManager.Logic.DataManager.IsWibWob)
            {
                serializer.Serialize(writer, value.Entries);
            }
            else
            {
                var tbl = new TableParser<LotYoukaiInfo>();
                tbl.Items = value.Entries;
                writer.WriteValue(tbl.ToString());
            }
        }
    }
    [JsonConverter(typeof(LotYoukaiInfoListConverter))]
    public class LotYoukaiInfoList
    {
        public List<LotYoukaiInfo> Entries = new();
    }
}
