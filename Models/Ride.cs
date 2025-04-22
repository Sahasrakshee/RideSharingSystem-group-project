using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RideSharingSystem.Models
{
    public class Ride
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DriverId { get; set; }

        [ForeignKey("DriverId")]
        public virtual User Driver { get; set; }

        
        public string? StartLocation { get; set; }

        
        public string? EndLocation { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }

        public Ride()
        {
            Bookings = new HashSet<Booking>();
        }
    }
}
