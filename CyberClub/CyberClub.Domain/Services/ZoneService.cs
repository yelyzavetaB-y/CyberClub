using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Services
{
    public class ZoneService
    {
        private readonly IZoneRepository _zoneRepository;

        public ZoneService(IZoneRepository zoneRepository)
        {
            this._zoneRepository = zoneRepository;
        }
        public async Task<List<Zone>> GetAllZonesAsync()
        {
            return await _zoneRepository.GetAllZonesAsync(); 
        }


    }
}
