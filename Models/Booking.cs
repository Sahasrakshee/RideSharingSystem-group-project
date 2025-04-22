using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RideSharingSystem.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public int RideId { get; set; }

        public int RiderId { get; set; }

        public string Status { get; set; }

        [ForeignKey("RideId")]
        public virtual Ride Ride { get; set; }

        [ForeignKey("RiderId")]
        public virtual User Rider { get; set; }
    }
}