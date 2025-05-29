using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Services
{
    public class TournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentService(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }

        public async Task<List<Tournament>> GetAllTournamentsAsync()
        {
            return await _tournamentRepository.GetAllAsync();
        }

        public async Task<Tournament?> GetByIdAsync(int id)
        {
            return await _tournamentRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateTournamentAsync(Tournament tournament)
        {
            return await _tournamentRepository.AddTournamentAsync(tournament);
        }

        public async Task<bool> UpdateTournamentAsync(Tournament tournament)
        {
            return await _tournamentRepository.UpdateAsync(tournament);
        }

        public async Task<bool> DeleteTournamentAsync(int id)
        {
            return await _tournamentRepository.DeleteAsync(id);
        }
        public async Task<bool> RegisterUserAsync(int tournamentId, int userId)
        {
            return await _tournamentRepository.RegisterUserAsync(tournamentId, userId);
        }
        public async Task<List<Tournament>> GetTournamentsByUserIdAsync(int userId)
        {
            return await _tournamentRepository.GetTournamentsByUserIdAsync(userId);
        }
        public async Task<bool> CancelUserBookingAsync(int tournamentId, int userId)
        {
            return await _tournamentRepository.CancelUserBookingAsync(tournamentId, userId);
        }
        public async Task<int> GetRegisteredCountAsync(int tournamentId)
        {
            return await _tournamentRepository.GetRegisteredCountAsync(tournamentId);
        }

    }
}
