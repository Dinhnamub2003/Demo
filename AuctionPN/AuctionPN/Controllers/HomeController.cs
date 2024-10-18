using AuctionPN.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AuctionPN.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuctionContext _context;

        public HomeController(AuctionContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.ListItem = _context.Items.OrderBy(i => i.EndDataTime).Take(8).ToList();
            return View();
        }

        public ActionResult Search(string search)
        {
            List<Item> item = _context.Items.Where(i => i.ItemName.Contains(search.ToLower()) || i.ItemDescription.Contains(search.ToLower()) || i.CurrentPrice.ToString().Contains(search.ToLower()) || i.EndDataTime.ToString().Contains(search.ToLower())).ToList();
            ViewBag.ListItem = item;
            ViewBag.Search = search;
            return View();
        }

        public ActionResult Order(string order)
        {
            List<Item> item = new List<Item>();
            if ( order == "1")
            {
                item = _context.Items.OrderBy(i => i.CurrentPrice).ToList();
            } else if ( order == "2")
            {
                item = _context.Items.OrderBy(i => i.EndDataTime).ToList();
            } else if ( order == "3")
            {
                item = _context.Items.OrderBy(i => i.ItemName).ToList();
            }
            ViewBag.ListItem = item;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}