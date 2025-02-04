﻿using System;
using System.Collections.Generic;

namespace AuctionPN.Models
{
    public partial class Item
    {
        public Item()
        {
            Bids = new HashSet<Bid>();
        }

        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string ItemDescription { get; set; } = null!;
        public decimal MinimumBidIncrement { get; set; }
        public string ItemImage { get; set; } = null!;
        public DateTime EndDataTime { get; set; }
        public decimal CurrentPrice { get; set; }
        public int? ItemTypeId { get; set; }
        public int? SellerId { get; set; }

        public virtual ItemType? ItemType { get; set; }
        public virtual Member? Seller { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
    }
}
