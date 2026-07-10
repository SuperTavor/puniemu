using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Logic;
using Puniemu.Src.Server.GameServer.Requests.Map.MapUnLock.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.MapUnLock.DataClasses;
using Puniemu.Src.TableParser.DataClasses;
using Puniemu.Src.TableParser.Logic;

namespace Puniemu.Src.Server.GameServer.Requests.Map.MapUnLock.Logic
{
    public sealed class MapUnlockException : Exception
    {
        public MapUnlockException()
        {
        }

        public MapUnlockException(string message)
            : base(message)
        {
        }

        public MapUnlockException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    public class MapUnLockService
    {

        private YwpUserData _userData;
        private List<YwpMstMap> _mstMap;
        private TableParser<YwpUserYoukai> _userYoukai;
        private TableParser<YwpUserStage> _userStage;
        private TableParser<YwpUserMap> _userMap;
        private UnlockType _unlockType;
        private long _mapId;
        private int _userMapIndex;
        private int _mstMapIndex;
        private int _userStageIndex;
        private string _gdkey;
        private MapUnLockService() { }
        public async static Task<MapUnLockService> BuildAsync(MapUnLockRequest req)
        {
            var built = new MapUnLockService();
            built._gdkey = req.Level5UserID;
            built._mapId = req.MapId;
            built._unlockType = req.UnlockType;
            built._userData = await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<YwpUserData>(req.Level5UserID, "ywp_user_data")!;
            built._userStage = new TableParser<YwpUserStage>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(req.Level5UserID, "ywp_user_stage")!);
            built._userMap = new TableParser<YwpUserMap>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(req.Level5UserID, "ywp_user_map"));
            built._userYoukai = new TableParser<YwpUserYoukai>(await UserDataManager.Logic.UserDataManager.GetYwpUserAsync<string>(req.Level5UserID, "ywp_user_youkai"));
            built._mstMap = JsonConvert.DeserializeObject<List<YwpMstMap>>(
                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    DataManager.Logic.DataManager.GameDataManager.GamedataCache["ywp_mst_map"]
                )!["data"].ToString()!
            )!;

            built.GetIndicesAndCheckValidity();


            return built;
        }

        private void GetIndicesAndCheckValidity()
        {
            _userMapIndex = MapManager.GetMapIndex(_userMap, _mapId);
            _mstMapIndex = MstMapManager.GetMapIndex(_mstMap, _mapId);
            _userStageIndex = StageManager.GetStageIndex(_userStage, (((int)_mapId * 1000) + 1));

            if (_userMapIndex == -1 || _mstMapIndex == -1)
            {
                throw new MapUnlockException("Invalid map");
            }
            if (_userStageIndex == -1)
            {
                throw new MapUnlockException("Invalid stage");
            }
        }
        public async Task<MapUnLockResponse> SaveDataAndGetResponse()
        {
            var compiledUserStage = _userStage.ToString();
            var compiledUserMap = _userMap.ToString();  

            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(_gdkey, "ywp_user_data", _userData);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(_gdkey, "ywp_user_stage", compiledUserStage);
            await UserDataManager.Logic.UserDataManager.SetYwpUserAsync(_gdkey, "ywp_user_map", compiledUserMap);

            var response = new MapUnLockResponse(_userData, compiledUserStage, compiledUserMap);

            return response;
        }
        public async Task Unlock()
        {
            switch (_unlockType)
            {
                case UnlockType.WithYMoney:
                    YMoney();
                    break;
                case UnlockType.WithYokai:
                    YoukaiLevel();
                    break;
                //TODO: implement friend points
                default:
                    throw new MapUnlockException($"Unsupported unlock type: {_unlockType}");
            }

          
            await GenerateFriendData.RefreshYwpUserFriend(_gdkey, -1, -1, _userData!.PlayerName, -1, "");
            
            _userStage.Items[_userStageIndex].StageStatus = 0;
            _userMap.Items[_userMapIndex].IsUnlocked = 1;
        }
        private void YMoney()
        {
            if (_mstMap[_mstMapIndex].NeedYmoney > _userData.YMoney)
            {
                throw new MapUnlockException("Not enough Y-Money");
            }
            _userData.YMoney -= (int)_mstMap[_mstMapIndex].NeedYmoney;
        }

        private void YoukaiLevel()
        {
            var targetYokaiId = _mstMap[_mstMapIndex].NeedYoukaiId;
            var targetYokaiLevel = _mstMap[_mstMapIndex].NeedYoukaiLevel;
            //Get yokai
            var yokaiIdx = YoukaiManager.GetYoukaiIndex(_userYoukai, targetYokaiId);
            if(yokaiIdx == -1)
            {
                throw new MapUnlockException("You don't have the Yo-kai");
            }
            if (!(_userYoukai.Items[yokaiIdx].Level >= targetYokaiLevel))
            {
                throw new MapUnlockException("Yo-kai not at required level");
            }
        }
    
    }
}
