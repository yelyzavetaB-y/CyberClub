using CyberClub.Domain.Models;
using CyberClub.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> AddBookingAsync(Booking booking);
        Task<bool> BookAvailableSeatAsync(int userId, DateTime startTime, int zoneId, int duration);
        Task<List<int>> GetBookedSeatIdsAsync(int zoneId, DateTime start, DateTime end);
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId, bool includeCancelled = false);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<List<Booking>> GetBookingsForZoneAndTimeAsync(int zoneId, DateTime startTime, int durationMinutes);
        Task<int> DeleteExpiredBookingsAsync();
        Task<List<Booking>> GetAllBookingsAsync();
        Task<bool> UpdateBookingStatusAsync(int bookingId, Status newStatus);
    }
}
