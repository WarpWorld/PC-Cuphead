using System;
using System.Collections.Generic;

namespace WarpWorld.CrowdControl
{
    /// <summary>Keeps information of all of the bids for this play session. </summary>
    class CCBidWarLibrary
    {
        private Dictionary<string, uint> bids = new Dictionary<string, uint>();

        private string highestID = "";
        private uint highestBid = 0;

        /// <summary>Place a bid for an option. Returns true if the item sent in ends up being the highest and a different ID. </summary>
        public bool PlaceBid(string id, uint amount)
        {
            if (!bids.ContainsKey(id))
                bids.Add(id, amount);
            else
                bids[id] += amount;

            if (bids[id] < highestBid)
                return false;

            highestBid = bids[id];

            if (id.Equals(highestID, StringComparison.InvariantCultureIgnoreCase))
                return false;

            highestID = id;
            
            return true;
        }
    }
}
