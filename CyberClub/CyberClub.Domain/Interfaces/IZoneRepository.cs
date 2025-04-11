using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Interfaces
{
    public interface IZoneRepository
    {
        Task<bool> AddZoneAsync(Zone zone);
        Task<List<Zone>> GetAllZonesAsync();
    }
}
