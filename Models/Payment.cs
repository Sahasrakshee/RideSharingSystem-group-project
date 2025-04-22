using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RideSharingSystem.Models
{
    public class Payment
    {

        
            public int Id { get; set; }
            public int UserBookingId { get; set; }
            public decimal Amount { get; set; }
            public DateTime PaymentDate { get; set; }

            
           
        }


   }

