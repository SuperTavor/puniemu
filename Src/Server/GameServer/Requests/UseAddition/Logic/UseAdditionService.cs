namespace Puniemu.Src.Server.GameServer.Requests.UseAddition.Logic
{
    public class UseAdditionService
    {
        private string _gdkey;

        private string _todayStr = DateTime.UtcNow.ToString("yyyyMMdd");
        public UseAdditionService(string gdkey)
        {
            _gdkey = gdkey;
        }

        public async Task<bool> CanDoShrineToday()
        {
            var lastShrineDate = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(_gdkey, "lastAdditionDate");
            return lastShrineDate != _todayStr;
        }

        public async Task MarkShrine()
        {
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(_gdkey, "lastAdditionDate", _todayStr);
        }
    }
}
