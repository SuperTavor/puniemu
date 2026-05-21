using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puniemu.Src.DataManager.Logic;
using Puniemu.Src.Server.GameServer.Requests.ExecuteGacha.DataClasses;
using System.ComponentModel;

public class ExecuteGachaResponseConverter : JsonConverter<ExecuteGachaResponse>
{

    public override void WriteJson(JsonWriter writer, ExecuteGachaResponse? value, JsonSerializer serializer)
    {
        JObject responseObj = JObject.FromObject(value!);

        if (DataManager.IsWibWob &&
            value.GachaPrizeList != null &&
            value.GachaPrizeList.Length == 1)
        {
            //Wibwob can only have one prize and its gachaPrize properties are in the root of the response
            JObject firstPrize = JObject.FromObject(value.GachaPrizeList[0], serializer);

            foreach (var prop in firstPrize.Properties())
            {
                responseObj[prop.Name] = prop.Value;
            }
            //Also stuff inside `youkai` is in the root ALONG with being in the youkai object or else it crashes
            var yokaiWonInfo = JObject.FromObject(value.GachaPrizeList[0].Youkai!);
            foreach (var prop in yokaiWonInfo.Properties())
            {
                responseObj[prop.Name] = prop.Value;
            }

            responseObj.Remove("gachaPrizeList");
        }

        responseObj.WriteTo(writer);
    }

    public override ExecuteGachaResponse? ReadJson(
        JsonReader reader,
        Type objectType,
        ExecuteGachaResponse? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        return JObject.Load(reader).ToObject<ExecuteGachaResponse>();
    }
}