using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Components
{
    public class SeatMapViewComponent : ViewComponent
    {
        private readonly SeatService _seatService;

        public SeatMapViewComponent(SeatService seatService)
        {
            _seatService = seatService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int defaultZoneId = 1; 
            var seats = await _seatService.GetSeatsByZoneIdAsync(defaultZoneId);

            return View("SeatMap", seats);
        }
    }
}
