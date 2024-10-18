using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionPN.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Security.Cryptography;

namespace AuctionPN.Controllers
{
    public class ItemsController : Controller
    {
        private readonly AuctionContext _context;

        public ItemsController(AuctionContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
           return View();
        }

        public ActionResult ItemByItemTypeID(string itemTypeId)
        {
            ItemType itemType = _context.ItemTypes.FirstOrDefault(it => it.ItemTypeId == int.Parse(itemTypeId));
            ViewBag.ItemType = itemType;
            ViewBag.ListItemByType = _context.Items.Include(i => i.ItemType).Include(i => i.Seller).Where(i => i.ItemTypeId == int.Parse(itemTypeId)).ToList();
            return View();
        }

        public ActionResult ItemBySeller(string id)
        {
            List<Item> item = _context.Items.Where(s => s.SellerId == int.Parse(id)).ToList();
            ViewBag.ListItemBySeller = item;
            return View();
        }

        public ActionResult ItemByBuyer(string id)
        {
            Member member = _context.Members.FirstOrDefault(m => m.MemberId == int.Parse(id));
            ViewBag.Buyer = member;
            return View();
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            Item item = _context.Items.Include(i => i.ItemType).Include(i => i.Seller).FirstOrDefault(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }
            ViewBag.Item = item;
            return View();
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            List<ItemType> listItemType = _context.ItemTypes.ToList();
            ViewBag.ListItemType = listItemType;
            ViewData["SellerId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,ItemDescription,MinimumBidIncrement,ItemImage,EndDataTime,CurrentPrice,ItemTypeId,SellerId")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("ItemBySeller","Items", new { id = item.SellerId });
            }
            List<ItemType> listItemType = _context.ItemTypes.ToList();
            ViewBag.ListItemType = listItemType;
            ViewData["SellerId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "ItemTypeId", "ItemTypeId", item.ItemTypeId);
            ViewData["SellerId"] = new SelectList(_context.Members, "MemberId", "MemberId", item.SellerId);
            ViewBag.Item = item;
            return View();
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemName,ItemDescription,MinimumBidIncrement,ItemImage,EndDataTime,CurrentPrice,ItemTypeId,SellerId")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "ItemTypeId", "ItemTypeId", item.ItemTypeId);
            ViewData["SellerId"] = new SelectList(_context.Members, "MemberId", "MemberId", item.SellerId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.ItemType)
                .Include(i => i.Seller)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }
            ViewBag.Item = item;
            return View();
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'AuctionContext.Items'  is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                List<Bid> lb = getByItemId(item.ItemId);
                foreach(var bid in lb)
                {
                    _context.Bids.Remove(bid);
                }
                _context.Items.Remove(item);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool ItemExists(int id)
        {
          return (_context.Items?.Any(e => e.ItemId == id)).GetValueOrDefault();
        }

        public List<Bid> getByItemId(int id) {
            List<Bid> lb = _context.Bids.Where(b => b.ItemId == id).ToList();
            return lb;
        }
    }
}
