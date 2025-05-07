using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Components
{
    public class ZoneViewComponent : ViewComponent
    {
        private readonly ZoneService _zoneService;

        public ZoneViewComponent(ZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var zones = await _zoneService.GetAllZonesAsync();
            int? userId = HttpContext.Session.GetInt32("UserID");
            var viewModel = new BookingViewModel
            {
                UserID = userId ?? 0,
                Zones = zones ?? new List<Zone>(),
                SelectedDate = DateTime.Today,
                SelectedTimeRaw = DateTime.Now.TimeOfDay.ToString("hh\\:mm"),
                Duration = 60
            };
            return View("Zone", viewModel);
        }

    }
}
