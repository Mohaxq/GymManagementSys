using AutoMapper;
using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.SessionScheduleViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberSessionService : IMemberSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberSessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public MemberSessionIndexViewModel GetSessionsForManagement()
        {
            var sessions = _unitOfWork.sessionReposistory.GetAllSessionsWithTrainerAndCategory();
            var now = DateTime.Now;

            var upcomingSessions = sessions
                .Where(s => s.startDate > now)
                .Select(s => MapToViewModel(s))
                .ToList();

            var ongoingSessions = sessions
                .Where(s => s.startDate <= now && s.EndDate >= now)
                .Select(s => MapToViewModel(s))
                .ToList();

            return new MemberSessionIndexViewModel
            {
                UpcomingSessions = upcomingSessions,
                OngoingSessions = ongoingSessions
            };
        }

        public SessionMembersListViewModel? GetMembersForUpcomingSession(int sessionId)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null || session.startDate <= DateTime.Now)
            {
                return null;
            }

            return GetMembersList(session, "Upcoming");
        }

        public SessionMembersListViewModel? GetMembersForOngoingSession(int sessionId)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null)
            {
                return null;
            }

            var now = DateTime.Now;
            if (session.startDate > now || session.EndDate < now)
            {
                return null;
            }

            return GetMembersList(session, "Ongoing");
        }

        public (bool Success, string Message) CancelBooking(int sessionId, int memberId)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null)
            {
                return (false, "Session not found");
            }

            if (session.startDate <= DateTime.Now)
            {
                return (false, "Cannot cancel booking for sessions that have already started");
            }

            var memberSession = _unitOfWork.GetRepository<MemberSession>()
                .GetAll()
                .FirstOrDefault(ms => ms.SessionId == sessionId && ms.MemberId == memberId);

            if (memberSession is null)
            {
                return (false, "Booking not found");
            }

            _unitOfWork.GetRepository<MemberSession>().Delete(memberSession);
            var result = _unitOfWork.SaveChanges();

            return result > 0 
                ? (true, "Booking cancelled successfully") 
                : (false, "Failed to cancel booking");
        }

        public (bool Success, string Message) MarkAttendance(int sessionId, int memberId)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null)
            {
                return (false, "Session not found");
            }

            var now = DateTime.Now;
            if (session.startDate > now)
            {
                return (false, "Cannot mark attendance for sessions that haven't started yet");
            }

            if (session.EndDate < now)
            {
                return (false, "Cannot mark attendance for completed sessions");
            }

            var memberSession = _unitOfWork.GetRepository<MemberSession>()
                .GetAll()
                .FirstOrDefault(ms => ms.SessionId == sessionId && ms.MemberId == memberId);

            if (memberSession is null)
            {
                return (false, "Booking not found");
            }

            if (memberSession.isAttended)
            {
                return (false, "Member is already marked as attended");
            }

            memberSession.isAttended = true;
            memberSession.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<MemberSession>().Update(memberSession);
            var result = _unitOfWork.SaveChanges();

            return result > 0 
                ? (true, "Attendance marked successfully") 
                : (false, "Failed to mark attendance");
        }

        public CancelBookingViewModel? GetCancelBookingDetails(int sessionId, int memberId)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null || session.startDate <= DateTime.Now)
            {
                return null;
            }

            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null)
            {
                return null;
            }

            var memberSession = _unitOfWork.GetRepository<MemberSession>()
                .GetAll()
                .FirstOrDefault(ms => ms.SessionId == sessionId && ms.MemberId == memberId);

            if (memberSession is null)
            {
                return null;
            }

            return new CancelBookingViewModel
            {
                SessionId = sessionId,
                MemberId = memberId,
                MemberName = member.Name,
                SessionName = session.SessionCategory?.CategoryName ?? "Unknown Session",
                SessionStartDate = session.startDate
            };
        }

        public MarkAttendanceViewModel? GetMarkAttendanceDetails(int sessionId, int memberId)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null)
            {
                return null;
            }

            var now = DateTime.Now;
            if (session.startDate > now || session.EndDate < now)
            {
                return null;
            }

            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null)
            {
                return null;
            }

            var memberSession = _unitOfWork.GetRepository<MemberSession>()
                .GetAll()
                .FirstOrDefault(ms => ms.SessionId == sessionId && ms.MemberId == memberId);

            if (memberSession is null || memberSession.isAttended)
            {
                return null;
            }

            return new MarkAttendanceViewModel
            {
                SessionId = sessionId,
                MemberId = memberId,
                MemberName = member.Name,
                SessionName = session.SessionCategory?.CategoryName ?? "Unknown Session"
            };
        }

        public AddMemberToSessionViewModel? GetAddMemberToSessionViewModel(int sessionId)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null || session.startDate <= DateTime.Now)
            {
                return null;
            }

            var bookedSpots = _unitOfWork.sessionReposistory.GetCountBookedSpots(sessionId);
            var availableSpots = session.capacity - bookedSpots;

            if (availableSpots <= 0)
            {
                return null;
            }

            return new AddMemberToSessionViewModel
            {
                SessionId = sessionId,
                SessionName = session.SessionCategory?.CategoryName ?? "Unknown Session",
                TrainerName = session.SessionTrainer?.Name ?? "Unknown",
                StartDate = session.startDate,
                AvailableSpots = availableSpots
            };
        }

        public IEnumerable<MemberSelectViewModel> GetAvailableMembersForSession(int sessionId)
        {
            // Get all members
            var allMembers = _unitOfWork.GetRepository<Member>().GetAll();

            // Get members already booked for this session
            var bookedMemberIds = _unitOfWork.GetRepository<MemberSession>()
                .GetAll()
                .Where(ms => ms.SessionId == sessionId)
                .Select(ms => ms.MemberId)
                .ToHashSet();

            // Filter out already booked members
            var availableMembers = allMembers
                .Where(m => !bookedMemberIds.Contains(m.Id))
                .Select(m => new MemberSelectViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Photo = m.Photo
                })
                .ToList();

            return availableMembers;
        }

        public (bool Success, string Message) BookMemberToSession(int sessionId, int memberId)
        {
            // Validate session exists and is upcoming
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(sessionId);
            if (session is null)
            {
                return (false, "Session not found");
            }

            if (session.startDate <= DateTime.Now)
            {
                return (false, "Cannot book members to sessions that have already started");
            }

            // Check available spots
            var bookedSpots = _unitOfWork.sessionReposistory.GetCountBookedSpots(sessionId);
            if (bookedSpots >= session.capacity)
            {
                return (false, "Session is fully booked");
            }

            // Validate member exists
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null)
            {
                return (false, "Member not found");
            }

            // Check if member is already booked
            var existingBooking = _unitOfWork.GetRepository<MemberSession>()
                .GetAll()
                .FirstOrDefault(ms => ms.SessionId == sessionId && ms.MemberId == memberId);

            if (existingBooking is not null)
            {
                return (false, "Member is already booked for this session");
            }

            // Create the booking
            var memberSession = new MemberSession
            {
                SessionId = sessionId,
                MemberId = memberId,
                isAttended = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _unitOfWork.GetRepository<MemberSession>().Add(memberSession);
            var result = _unitOfWork.SaveChanges();

            return result > 0
                ? (true, $"{member.Name} has been successfully booked for this session")
                : (false, "Failed to book member to session");
        }

        private SessionScheduleViewModel MapToViewModel(Session session)
        {
            var bookedSpots = _unitOfWork.sessionReposistory.GetCountBookedSpots(session.Id);
            return new SessionScheduleViewModel
            {
                SessionId = session.Id,
                CategoryName = session.SessionCategory?.CategoryName ?? "Unknown",
                Description = session.Description,
                TrainerName = session.SessionTrainer?.Name ?? "Unknown",
                StartDate = session.startDate,
                EndDate = session.EndDate,
                Capacity = session.capacity,
                AvailableSpots = session.capacity - bookedSpots
            };
        }

        private SessionMembersListViewModel GetMembersList(Session session, string status)
        {
            var bookedSpots = _unitOfWork.sessionReposistory.GetCountBookedSpots(session.Id);
            var memberSessions = _unitOfWork.GetRepository<MemberSession>()
                .GetAll()
                .Where(ms => ms.SessionId == session.Id)
                .ToList();

            var members = new List<SessionMemberViewModel>();
            foreach (var ms in memberSessions)
            {
                var member = _unitOfWork.GetRepository<Member>().GetById(ms.MemberId);
                if (member != null)
                {
                    members.Add(new SessionMemberViewModel
                    {
                        MemberId = member.Id,
                        MemberName = member.Name,
                        MemberPhoto = member.Photo,
                        IsAttended = ms.isAttended,
                        BookedAt = ms.CreatedAt
                    });
                }
            }

            return new SessionMembersListViewModel
            {
                SessionId = session.Id,
                SessionName = session.SessionCategory?.CategoryName ?? "Unknown Session",
                TrainerName = session.SessionTrainer?.Name ?? "Unknown",
                StartDate = session.startDate,
                EndDate = session.EndDate,
                Status = status,
                Capacity = session.capacity,
                AvailableSpots = session.capacity - bookedSpots,
                Members = members
            };
        }
    }
}
