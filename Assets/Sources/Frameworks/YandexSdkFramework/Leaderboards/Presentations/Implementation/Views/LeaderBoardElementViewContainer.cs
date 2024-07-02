using System.Collections.Generic;
using UnityEngine;

namespace Sources.Frameworks.YandexSdkFramework.Leaderboards.Presentations.Implementation.Views
{
    public class LeaderBoardElementViewContainer
    {
        [field: SerializeField] public List<LeaderBoardElementView> LeaderBoardElementViews { get; private set; }
    }
}