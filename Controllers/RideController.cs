using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideSharingSystem.Models;
using RideSharingSystem.Data;    // Add this

namespace RideSharingSystem.Controllers
{
    public class RideController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RideController> _logger;

        public RideController(ApplicationDbContext context, ILogger<RideController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ride
        public async Task<IActionResult> Index()
        {
            try
            {
                var rides = await _context.Rides
                    .Include(r => r.Driver)
                    .ToListAsync();
                return View(rides);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rides");
                return View(new List<Ride>());
            }
        }

        // GET: Ride/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var ride = await _context.Rides
                    .Include(r => r.Driver)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (ride == null)
                {
                    return NotFound();
                }

                return View(ride);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ride details");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Ride/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ride/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Origin,Destination,DepartureTime,AvailableSeats,Price")] Ride ride)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(ride);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating ride");
                    ModelState.AddModelError("", "Unable to create ride. Please try again.");
                }
            }
            return View(ride);
        }

        // GET: Ride/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides.FindAsync(id);
            if (ride == null)
            {
                return NotFound();
            }
            return View(ride);
        }

        // POST: Ride/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Origin,Destination,DepartureTime,AvailableSeats,Price")] Ride ride)
        {
            if (id != ride.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ride);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!RideExists(ride.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency error updating ride");
                        ModelState.AddModelError("", "Unable to save changes. The ride was modified by another user.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating ride");
                    ModelState.AddModelError("", "Unable to save changes. Please try again.");
                }
            }
            return View(ride);
        }

        // GET: Ride/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }

        // POST: Ride/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ride = await _context.Rides.FindAsync(id);
                if (ride != null)
                {
                    _context.Rides.Remove(ride);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ride");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.Id == id);
        }
    }
}