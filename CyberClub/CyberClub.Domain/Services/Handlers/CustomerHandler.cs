using CyberClub.Domain.Models;
using CyberClub.Domain.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Services.Handlers
{
    public class CustomerHandler
    {
        private readonly ZoneService _zoneService;
        private readonly SeatService _seatService;

        public CustomerHandler(ZoneService zoneService, SeatService seatService)
        {
            _zoneService = zoneService;
            _seatService = seatService;
        }

        public async Task<List<ZoneSeatMap>> BuildSeatMapAsync(DateTime date, TimeSpan time, int duration)
        {
            var requestedTime = date + time;
            var zones = await _zoneService.GetAllZonesAsync();
            var result = new List<ZoneSeatMap>();

            foreach (var zone in zones)
            {
                var seats = await _seatService.GetSeatsWithAvailabilityAsync(zone.ZoneID, requestedTime, duration);
                result.Add(new ZoneSeatMap
                {
                    ZoneID = zone.ZoneID,
                    Name = zone.Name,
                    Capacity = zone.Capacity,
                    Seats = seats.Select(s => new SeatStatus
                    {
                        SeatID = s.SeatID,
                        SeatNumber = s.SeatNumber,
                        IsAvailable = s.IsAvailable
                    }).ToList()
                });
            }

            return result;
        }
    }

}
