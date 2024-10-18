using System;
using System.Collections.Generic;

namespace AuctionPN.Models
{
    public partial class Member
    {
        public Member()
        {
            Bids = new HashSet<Bid>();
            Items = new HashSet<Item>();
        }

        public int MemberId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Email { get; set; }
        public DateTime? Expirationdate { get; set; }
        public string Password { get; set; } = null!;

        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
