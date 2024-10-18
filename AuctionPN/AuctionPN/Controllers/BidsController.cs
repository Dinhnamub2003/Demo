using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionPN.Models;

namespace AuctionPN.Controllers
{
    public class BidsController : Controller
    {
        private readonly AuctionContext _context;

        public BidsController(AuctionContext context)
        {
            _context = context;
        }

        // GET: Bids
        public async Task<IActionResult> Index()
        {
            var auctionContext = _context.Bids.Include(b => b.Bidder).Include(b => b.Item);
            ViewBag.ListBid = _context.Bids.Include(b => b.Bidder).Include(b => b.Item).ToList();
            return View();
        }

        public async Task<IActionResult> ByDay(string day)
        {
            var auctionContext = _context.Bids.Include(b => b.Bidder).Include(b => b.Item);
            ViewBag.ListBidDay = _context.Bids.Include(b => b.Bidder).Include(b => b.Item).Where( b => b.BidDataTime.Date == DateTime.Parse(day)).ToList();
            ViewBag.Day = day;
            return View();
        }

        public async Task<IActionResult> ByMonth(string month)
        {
            var auctionContext = _context.Bids.Include(b => b.Bidder).Include(b => b.Item);
            ViewBag.ListBidMonth = _context.Bids.Include(b => b.Bidder).Include(b => b.Item).Where(b => b.BidDataTime.Month == int.Parse(month)).ToList();
            decimal total = 0;
            List<Bid> lb = _context.Bids.Include(b => b.Bidder).Include(b => b.Item).Where(b => b.BidDataTime.Month == int.Parse(month)).ToList();
            foreach (var bid in lb)
            {
                total += bid.Item.CurrentPrice;
            }
            ViewBag.Month = month;
            string totalmoney = total.ToString();
            ViewBag.Total = totalmoney;
            return View();
        }

        // GET: Bids/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bids == null)
            {
                return NotFound();
            }

            var bid = await _context.Bids
                .Include(b => b.Bidder)
                .Include(b => b.Item)
                .FirstOrDefaultAsync(m => m.BidId == id);
            if (bid == null)
            {
                return NotFound();
            }
            ViewBag.Bid = bid;
            return View();
        }

        public ActionResult Update(string id, string biddatetime, string bidprice, string bidderid,  string itemid)
        {
            Bid bid = _context.Bids.FirstOrDefault(b => b.BidId == int.Parse(id));
            bid.BidDataTime = DateTime.Parse(biddatetime);
            bid.BidPrice = decimal.Parse(bidprice);
            bid.BidderId = int.Parse(bidderid);
            bid.ItemId = int.Parse(itemid);

            Item item = _context.Items.FirstOrDefault(i => i.ItemId == bid.ItemId);
            item.CurrentPrice = decimal.Parse(bidprice);
            _context.Update(item);
            _context.Update(bid);
            _context.SaveChanges();
            ViewBag.Bid = bid;
            ViewBag.Item = item;
            ViewBag.Messenger = "Tham gia đấu giá thành công.";
            return RedirectToAction("Details","Items", new { id = bid.ItemId }); 
        }

        public ActionResult BidEarly(string time)
        {
            DateTime now = DateTime.Now;
            List<Bid> listBid = _context.Bids.Where(b => b.BidDataTime > now).Include(i => i.Item).ToList();
            ViewBag.ListBid = listBid;
            return View();
        }

        public ActionResult BidLate(string time)
        {
            DateTime now = DateTime.Now;
            List<Bid> listBid = _context.Bids.Where(b => b.BidDataTime < now).Include(i => i.Item).ToList();
            ViewBag.ListBid = listBid;
            return View();
        }

        public ActionResult BidNow(string time)
        {
            DateTime now = DateTime.Now;
            List<Bid> listBid = _context.Bids.Where(b => b.BidDataTime == now).Include(i => i.Item).ToList();
            ViewBag.ListBid = listBid;
            return View();
        }

        // GET: Bids/Create
        public IActionResult Create()
        {
            ViewData["BidderId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId");
            return View();
        }

        // POST: Bids/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BidId,BidDataTime,BidPrice,ItemId,BidderId")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BidderId"] = new SelectList(_context.Members, "MemberId", "MemberId", bid.BidderId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", bid.ItemId);
            return View(bid);
        }

        // GET: Bids/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bids == null)
            {
                return NotFound();
            }

            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
            {
                return NotFound();
            }
            ViewData["BidderId"] = new SelectList(_context.Members, "MemberId", "MemberId", bid.BidderId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", bid.ItemId);
            return View(bid);
        }

        // POST: Bids/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BidId,BidDataTime,BidPrice,ItemId,BidderId")] Bid bid)
        {
            if (id != bid.BidId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BidExists(bid.BidId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BidderId"] = new SelectList(_context.Members, "MemberId", "MemberId", bid.BidderId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", bid.ItemId);
            return View(bid);
        }

        // GET: Bids/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bids == null)
            {
                return NotFound();
            }

            var bid = await _context.Bids
                .Include(b => b.Bidder)
                .Include(b => b.Item)
                .FirstOrDefaultAsync(m => m.BidId == id);
            if (bid == null)
            {
                return NotFound();
            }

            return View(bid);
        }

        // POST: Bids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bids == null)
            {
                return Problem("Entity set 'AuctionContext.Bids'  is null.");
            }
            var bid = await _context.Bids.FindAsync(id);
            if (bid != null)
            {
                _context.Bids.Remove(bid);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BidExists(int id)
        {
          return (_context.Bids?.Any(e => e.BidId == id)).GetValueOrDefault();
        }
    }
}
