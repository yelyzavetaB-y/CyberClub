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
        Task<bool> AddTournamentAsync(Tournament tournament);
        Task<bool> UpdateAsync(Tournament tournament);
        Task<bool> DeleteAsync(int id);
        Task<bool> RegisterUserAsync(int tournamentId, int userId);
        Task<List<Tournament>> GetTournamentsByUserIdAsync(int userId);
        Task<bool> CancelUserBookingAsync(int tournamentId, int userId);
        Task<int> GetRegisteredCountAsync(int tournamentId);
    }
}
