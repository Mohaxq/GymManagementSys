using GymManagementBLL.ViewModels.SessionScheduleViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    public interface IMemberSessionService
    {
        /// <summary>
        /// Gets all upcoming and ongoing sessions for booking/attendance management
        /// </summary>
        MemberSessionIndexViewModel GetSessionsForManagement();

        /// <summary>
        /// Gets all members booked for an upcoming session (for booking cancellation)
        /// </summary>
        SessionMembersListViewModel? GetMembersForUpcomingSession(int sessionId);

        /// <summary>
        /// Gets all members booked for an ongoing session (for attendance marking)
        /// </summary>
        SessionMembersListViewModel? GetMembersForOngoingSession(int sessionId);

        /// <summary>
        /// Cancels a member's booking for an upcoming session
        /// </summary>
        (bool Success, string Message) CancelBooking(int sessionId, int memberId);

        /// <summary>
        /// Marks a member as attended for an ongoing session
        /// </summary>
        (bool Success, string Message) MarkAttendance(int sessionId, int memberId);

        /// <summary>
        /// Gets details for cancel booking confirmation
        /// </summary>
        CancelBookingViewModel? GetCancelBookingDetails(int sessionId, int memberId);

        /// <summary>
        /// Gets details for mark attendance confirmation
        /// </summary>
        MarkAttendanceViewModel? GetMarkAttendanceDetails(int sessionId, int memberId);

        /// <summary>
        /// Gets the view model for adding a member to a session
        /// </summary>
        AddMemberToSessionViewModel? GetAddMemberToSessionViewModel(int sessionId);

        /// <summary>
        /// Gets available members who can be booked for a session (not already booked)
        /// </summary>
        IEnumerable<MemberSelectViewModel> GetAvailableMembersForSession(int sessionId);

        /// <summary>
        /// Books a member to a session
        /// </summary>
        (bool Success, string Message) BookMemberToSession(int sessionId, int memberId);
    }
}
