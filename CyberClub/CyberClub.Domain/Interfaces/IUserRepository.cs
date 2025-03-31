using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddUserAsync(User user);
        Task<bool> AddUserProfileAsync(UserProfile profile);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateUserWithRoleAsync(User user, int newRoleId);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> RegisterAsync(User user);
        Task<bool> SignInAsync(User user);
        Task<User> FindByEmailAsync(string email);

    }
}
