using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Interfaces
{
    public interface ITournamentRepository
    {
        Task<List<Tournament>> GetAllAsync();
        Task<Tournament?> GetByIdAsync(int id);
        Task<bool> AddAsync(Tournament tournament);
        Task<bool> UpdateAsync(Tournament tournament);
        Task<bool> DeleteAsync(int id);
    }
}
