using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels.BookingViewModel
{
    public class BookingViewModel
    {
        public int Id { get; set; }

        // Member Information
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;

        // Session Information
        public int SessionId { get; set; }
        public string SessionDescription { get; set; } = null!;
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public string? TrainerName { get; set; }
        public string? CategoryName { get; set; }

        // Booking Status
        public bool IsAttended { get; set; }
        public DateTime BookedAt { get; set; }

        // Computed Properties
        public string SessionStatus
        {
            get
            {
                var now = DateTime.Now;
                if (SessionStartDate > now)
                    return "Upcoming";
                else if (SessionEndDate < now)
                    return "Completed";
                else
                    return "Ongoing";
            }
        }

        public bool CanCancel => SessionStartDate > DateTime.Now;
        public bool CanMarkAttendance => SessionStartDate <= DateTime.Now && SessionEndDate > DateTime.Now && !IsAttended;
    }
}
