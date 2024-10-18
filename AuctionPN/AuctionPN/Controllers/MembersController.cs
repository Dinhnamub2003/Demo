using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionPN.Models;
using Newtonsoft.Json;

namespace AuctionPN.Controllers
{
    public class MembersController : Controller
    {
        private readonly AuctionContext _context;

        public MembersController(AuctionContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            ViewBag.Members = _context.Members.ToList();
              return _context.Members != null ? 
                          View(await _context.Members.ToListAsync()) :
                          Problem("Entity set 'AuctionContext.Members'  is null.");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            List<Member> list = _context.Members.ToList();
            foreach( var mem in list)
            {
                if ( mem.Email.ToLower().Equals(email.ToLower()) && mem.Password.Equals(password))
                {
                    string memberJson = JsonConvert.SerializeObject(mem);
                    HttpContext.Session.SetString("User", memberJson);
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View("Login");
        }

        [HttpGet]
        public ActionResult Forgot()
        {
            return View("Forgot");
        }

        [HttpPost]
        public ActionResult Forgot(string email)
        {
            List<Member> list = _context.Members.ToList();
            foreach (var mem in list)
            {
                if (mem.Email.ToLower().Equals(email.ToLower()))
                {
                    ViewBag.Email = email;
                    return View("EnterPassword");
                }
            }
            ViewData["ErrorMessage"] = "Email không tồn tại.";
            return View("Forgot");
        }

        [HttpPost]
        public ActionResult EnterPassword(string newpassword, string email)
        {
            List<Member> list = _context.Members.ToList();
            foreach (var mem in list)
            {
                if (mem.Email.ToLower().Equals(email.ToLower()))
                {
                    mem.Password = newpassword;
                    _context.Update(mem);
                    _context.SaveChanges();
                    string memberJson = JsonConvert.SerializeObject(mem);
                    HttpContext.Session.SetString("User", memberJson);
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewData["ErrorMessage"] = "Mật khẩu không hợp lệ.";
            return View("EnterPassword");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View("Create");
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }



        // GET: Members/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = _context.Members
                .FirstOrDefault(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            ViewBag.Member = member;
            return View();
        }

        // GET: Members/Create

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,Name,Address,Email,Expirationdate,Password")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                string memberJson = JsonConvert.SerializeObject(member);
                HttpContext.Session.SetString("User", memberJson);
                return RedirectToAction("Index", "Home");
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = _context.Members.Find(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewBag.Member = member;
            return View();
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("MemberId,Name,Address,Email,Expirationdate,Password")] Member member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                string memberJson = JsonConvert.SerializeObject(member);
                HttpContext.Session.SetString("User", memberJson);
                return RedirectToAction("Index","Home");
            }
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            ViewBag.Member = member;
            return View();
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Members == null)
            {
                return Problem("Entity set 'AuctionContext.Members'  is null.");
            }
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                List<Bid> lb = getByMember(id);
                List<Item> it = getItemByMember(id);
                foreach (var b in lb)
                {
                    _context.Bids.Remove(b);
                }
                foreach (var i in it){
                    _context.Items.Remove(i);
                }
                _context.Members.Remove(member);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Members");
        }

        private bool MemberExists(int id)
        {
          return (_context.Members?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }

        public List<Bid> getByMember(int id)
        {
            return _context.Bids.Where( m => m.BidderId == id).ToList();
        }

        public List<Item> getItemByMember(int id)
        {
            return _context.Items.Where(i => i.SellerId == id).ToList();
        }
    }
}
