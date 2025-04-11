using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Services
{
    public class SeatService
    {
        private readonly ISeatRepository seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            this.seatRepository = seatRepository;
        }
        public async Task<List<Seat>> GetSeatsByZoneIdAsync(int zoneId)
        {
            return await seatRepository.GetSeatsByZoneIdAsync(zoneId);
        }
    }
}
