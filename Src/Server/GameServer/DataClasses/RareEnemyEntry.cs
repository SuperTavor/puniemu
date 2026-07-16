namespace Puniemu.Src.Server.GameServer.DataClasses
{

    public enum RareYokaiScope
    {
        //yokai can appear in any stage in those maps with a different rate (Except bosses)
        MapsWide,
        //yokai can appear in any of the listed stages (Including bosses if so you wish)
        //StagesWide rare encounters can override mapsWide
        StagesWide,
    }
    public class RareEnemyEntry
    {
        public long EnemyID { get; set; }

        public double Rate { get; set; }

        public RareYokaiScope Scope { get; set; }

        //if mapswide, then the maps id, if stages wide, then the stages id 
        public long[] Params { get; set; }


        //if mapswide, the exceptions are stages id to not have the rare encounter on them
        public long[] ExceptionParams { get; set; }
    }
}
