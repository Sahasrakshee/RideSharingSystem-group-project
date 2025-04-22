using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideSharingSystem.Models;
using RideSharingSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace RideSharingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // This will be our landing page now
            return View();
        }

        public IActionResult RideList()
        {
            try
            {
                var rides = _context.Rides
                    .Include(r => r.Driver)
                    .OrderByDescending(r => r.Id)
                    .ToList();

                return View(rides ?? new List<Ride>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rides");
                return View(new List<Ride>());
            }
        }


        // GET: Home/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string StartLocation, string EndLocation, int DriverId, int AvailableSeats, decimal Price)
        {
            try
            {
                // Create new ride manually from form fields
                var ride = new Ride
                {
                    StartLocation = StartLocation,
                    EndLocation = EndLocation,
                    DriverId = DriverId,
                    AvailableSeats = AvailableSeats,
                    Price = Price
                };

                // Debug info
                System.Diagnostics.Debug.WriteLine($"Creating ride: {StartLocation} to {EndLocation}, Driver: {DriverId}, Seats: {AvailableSeats}, Price: {Price}");

                // Add and save
                _context.Rides.Add(ride);
                await _context.SaveChangesAsync();

               
                return RedirectToAction(nameof(RideList));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Failed to create ride: " + ex.Message);
                return View();
            }
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Home/Edit/5
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

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string StartLocation, string EndLocation,
            int DriverId, int AvailableSeats, decimal Price)
        {
            try
            {
                var ride = await _context.Rides.FindAsync(id);
                if (ride == null)
                {
                    return NotFound();
                }

                // Update the ride properties
                ride.StartLocation = StartLocation;
                ride.EndLocation = EndLocation;
                ride.DriverId = DriverId;
                ride.AvailableSeats = AvailableSeats;
                ride.Price = Price;

                _context.Update(ride);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(RideList));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating ride: " + ex.Message);
                return View();
            }
        }

        // GET: Home/Book/id
        public async Task<IActionResult> Book(int? id)
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

        // POST: Home/BookConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookConfirmed(int id)
        {
            var ride = await _context.Rides.FindAsync(id);
            if (ride == null)
            {
                return NotFound();
            }

            if (ride.AvailableSeats < 1)
            {
                TempData["Error"] = "No seats available for this ride.";
                return RedirectToAction(nameof(RideList));
            }

            // Create new booking
            var booking = new Booking
            {
                RideId = ride.Id,
                RiderId = 1, // You should replace this with actual logged-in user's ID
                Status = "Confirmed"
            };

            // Decrease available seats
            ride.AvailableSeats--;

            try
            {
                _context.Bookings.Add(booking);
                _context.Update(ride);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ride booked successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error booking ride: {ex.Message}");
                TempData["Error"] = "Unable to book ride. Please try again.";
            }

            return RedirectToAction(nameof(RideList));
        }




        // GET: Home/Delete/5
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

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ride = await _context.Rides
                    .Include(r => r.Bookings)  // Include related bookings
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (ride != null)
                {
                    // First remove all related bookings
                    _context.Bookings.RemoveRange(ride.Bookings);
                    // Then remove the ride
                    _context.Rides.Remove(ride);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Ride deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting ride: {ex.Message}");
                TempData["Error"] = "Unable to delete ride. Please try again.";
            }

            return RedirectToAction(nameof(RideList));
        }

        // GET: Home/Feedback
        public IActionResult Feedback()
        {
            return View(); // Return the Feedback view
        }

        // POST: Home/Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirect to homepage after submission
            }
            return View(feedback); // Return the view with the feedback model if validation fails
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}