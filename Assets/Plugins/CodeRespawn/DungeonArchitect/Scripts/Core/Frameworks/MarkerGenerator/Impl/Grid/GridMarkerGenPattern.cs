using System;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Grid
{
    public class GridMarkerGenPattern : MarkerGenPattern
    {
        [Tooltip(@"List of marker names that should be on the same level while testing for the pattern
* E.g. 1: If you add 'Ground' to the list, and if that marker exist at the location of any of the pattern rule blocks, they'll have to be on the same height, or the pattern won't match. This might be useful if you're inserting a larger ground tile (2x2) and don't want it to match with a adjacent ground tile of a different height
* E.g. 2: You're trying to turn a (Door-Wall) edge into a larger 2x Door. We don't want to match the pattern if the Wall and Door adjacent edges are on different heights, so you'd specify both 'Wall' and 'Door' in this list")]
        public string[] sameHeightMarkers = Array.Empty<string>();

        public bool expandMarkerDomain = false;
        public int expandMarkerDomainAmount = 0;
    }
}