namespace Puniemu.Src.Server.GameServer.DataClasses
{
    public class YwpMstYoukaiLegendRelease
    {
        public long LegendYokaiID { get; set; }

        //First 3 numbers are the left ones from top to bottom, last 3 numbers are the right ones from top to bottom

        public long Yokai1ID { get;set;  }

        public long Yokai2ID { get; set; }

        public long Yokai3ID { get; set; }

        public long Yokai4ID { get; set; }

        public long Yokai5ID { get; set; }

        public long Yokai6ID { get; set; }

        public string Yokai1Hint { get; set; }

        public string Yokai2Hint { get; set; }

        public string Yokai3Hint { get; set; }

        public string Yokai4Hint { get; set; }

        public string Yokai5Hint { get; set; }

        public string Yokai6Hint { get; set; }

    }
}
