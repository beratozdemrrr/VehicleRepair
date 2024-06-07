using System.ComponentModel.DataAnnotations;

namespace VehicleRepairMVC.Models
{
    public class AppointmentStatusChanges
    {
        [Key]
        public int StatusChangeID { get; set; }
        public int AppointmentID { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public int ChangedBy { get; set; }

    }
}
