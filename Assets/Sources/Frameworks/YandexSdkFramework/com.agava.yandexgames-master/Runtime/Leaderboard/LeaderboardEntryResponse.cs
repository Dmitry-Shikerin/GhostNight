using System;
using Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.PlayerAccount;
using UnityEngine.Scripting;

namespace Sources.Frameworks.YandexSdkFramework.com.agava.yandexgames_master.Runtime.Leaderboard
{
    [Serializable]
    public class LeaderboardEntryResponse
    {
        [field: Preserve]
        public int score;
        [field: Preserve]
        public string extraData;
        [field: Preserve]
        public int rank;
        [field: Preserve]
        public PlayerAccountProfileDataResponse player;
        [field: Preserve]
        public string formattedScore;
    }
}
