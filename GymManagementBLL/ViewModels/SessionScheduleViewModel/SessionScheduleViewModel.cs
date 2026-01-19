using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels.SessionScheduleViewModel
{
    public class SessionScheduleViewModel
    {
        public int SessionId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string TrainerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int AvailableSpots { get; set; }

        #region Computed Properties
        public string DateDisplay => $"{StartDate:MMM dd, yyyy}";
        public string TimeRange => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";
        public TimeSpan Duration => EndDate - StartDate;

        public string Status
        {
            get
            {
                var now = DateTime.Now;
                if (now < StartDate)
                    return "Upcoming";
                else if (now >= StartDate && now <= EndDate)
                    return "Ongoing";
                else
                    return "Completed";
            }
        }

        public bool IsUpcoming => DateTime.Now < StartDate;
        public bool IsOngoing => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public bool IsCompleted => DateTime.Now > EndDate;
        public bool CanBook => IsUpcoming && AvailableSpots > 0;
        #endregion
    }

    public class MemberSessionIndexViewModel
    {
        public IEnumerable<SessionScheduleViewModel> UpcomingSessions { get; set; } = [];
        public IEnumerable<SessionScheduleViewModel> OngoingSessions { get; set; } = [];
    }

    public class SessionMemberViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string? MemberPhoto { get; set; }
        public bool IsAttended { get; set; }
        public DateTime BookedAt { get; set; }
    }

    public class SessionMembersListViewModel
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; } = null!;
        public string TrainerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
        public int Capacity { get; set; }
        public int AvailableSpots { get; set; }
        public IEnumerable<SessionMemberViewModel> Members { get; set; } = [];
        public bool CanAddMore => Status == "Upcoming" && AvailableSpots > 0;
    }

    public class MarkAttendanceViewModel
    {
        public int SessionId { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string SessionName { get; set; } = null!;
    }

    public class CancelBookingViewModel
    {
        public int SessionId { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string SessionName { get; set; } = null!;
        public DateTime SessionStartDate { get; set; }
    }

    public class AddMemberToSessionViewModel
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; } = null!;
        public string TrainerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public int AvailableSpots { get; set; }

        [Required(ErrorMessage = "Please select a member")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }
    }

    public class MemberSelectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Photo { get; set; }
    }
}
