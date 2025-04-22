using Microsoft.AspNetCore.Mvc;
using RideSharingSystem.Data;
using RideSharingSystem.Models;
using Microsoft.EntityFrameworkCore;


public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: User/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // GET: User/Index
    public async Task<IActionResult> Index()
    {
        return View(await _context.Users.ToListAsync());
    }
}
