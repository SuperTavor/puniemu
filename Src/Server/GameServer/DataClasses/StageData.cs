﻿using Newtonsoft.Json;

namespace Puniemu.Src.Server.GameServer.DataClasses
{
    // stage data info : WIP (we actually store enemy (and if he was default befriends) and tutorial changes
    public class TutorialEntry
    {
        // tutorial type
        [JsonProperty("tutorialType")]
        public int TutorialType { get; set; }
        [JsonProperty("tutorialId")]
        public long TutorialId { get; set; }
        [JsonProperty("tutorialStatus")]
        public int TutorialStatus { get; set; }
        [JsonProperty("firstClear")]
        public int FirstClear { get; set; }
    }
    public class MenufuncEntry
    {
        // tutorial type
        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("value")]
        public long value { get; set; }
    }
    public class TutorialEntryRespRes
    {
        // tutorial requests
        [JsonProperty("requests")]
        public List<TutorialEntry>? TutorialReq { get; set; }
        // tutorial response
        [JsonProperty("response")]
        public List<TutorialEntry>? TutorialResp { get; set; }
    }
    public class EnemyStageEntry
    {
        [JsonProperty("id")]
        public long EnemyId { get; set; }
        [JsonProperty("defBefriends")]
        public int DefaultBefriends { get; set; }
    }
    public class FirstRewardEntry
    {
        [JsonProperty("itemId")]
        public long ItemId { get; set; }
        [JsonProperty("itemType")]
        public int ItemType { get; set; }
        [JsonProperty("itemCnt")]
        public int ItemCount { get; set; }
    }
    public class StageData
    {
        // list of enemy id
        [JsonProperty("enemy")]
        public List<EnemyStageEntry> ?Enemy { get; set; }
        [JsonProperty("menufunc_edit")]
        public List<MenufuncEntry> ?Menufunc { get; set; }
        // first reward
        [JsonProperty("first_reward")]
        public List<FirstRewardEntry> ?FirstReward { get; set; }
        // tutorial edition
        [JsonProperty("tutorial_edit")]
        public TutorialEntryRespRes ?TutorialEdit { get; set; }
    }
}
