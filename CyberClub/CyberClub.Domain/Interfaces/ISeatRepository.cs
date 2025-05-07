using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Interfaces
{
    public interface ISeatRepository
    {
        Task<bool> AddSeatAsync(Seat seat);
        Task<List<Seat>> FindAvailableSeatAsync(int zoneId, DateTime startTime, int duration);
        Task<bool> UpdateSeatAvailabilityAsync(int seatId, bool isAvailable);
        Task<Seat> GetSeatByIdAsync(int seatId);
        Task<List<Seat>> GetSeatsByZoneIdAsync(int zoneId);
        Task<int> ReleaseSeatsWithEndedBookingsAsync();
    }

}
