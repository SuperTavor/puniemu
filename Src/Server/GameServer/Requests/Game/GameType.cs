using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.Requests.GameEnd
{
    public enum GameEndType
    {
        GameEnd = 0,
        GameRetire = 1,
        GameEndScoreAttack = 2,
    }
    public enum GameStartType
    {
        GameStart = 0,
        GameStartScoreAttack = 1,
    }
}
