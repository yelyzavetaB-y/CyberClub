using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberClub.Domain.Interfaces;
using CyberClub.Helper;

namespace CyberClub.Domain.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }
        public async Task<bool> AddUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Email and full name must be provided.");

            return await _userRepository.AddUserAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Email and full name must be provided.");

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> UpdateUserWithRoleAsync(User user, int newRoleId)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (newRoleId <= 0)
                throw new ArgumentException("Invalid role ID.");

            return await _userRepository.UpdateUserWithRoleAsync(user, newRoleId);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<bool> RegisterUserAsync(User user, string confirmPassword)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.HashPassword) || user.HashPassword != confirmPassword)
                throw new ArgumentException("Passwords do not match or are invalid");

            if (await _userRepository.FindByEmailAsync(user.Email) != null)
                throw new InvalidOperationException("A user with this email already exists");

            string salt = SecurityHelper.GenerateSalt(70);
            string hashedPassword = SecurityHelper.HashPassword(user.HashPassword, salt, 10101, 70);
            

            user.HashPassword = hashedPassword;
            user.Salt = salt;

            return await _userRepository.AddUserAsync(user);
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            User user = await _userRepository.FindByEmailAsync(email);
            if (user != null)
            {
                string hashedPassword = SecurityHelper.HashPassword(password, user.Salt, 10101, 70);
               
                if (hashedPassword == user.HashPassword)
                {
                    return user;
                }
                else { throw new ArgumentException("hash dont match"); }
            }
            return null;
        }

        public async Task<bool> SignInAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.HashPassword))
                throw new ArgumentException("Email and password must be provided.");

            return await _userRepository.SignInAsync(user);
        }

        public async Task<bool> CreateUserWithProfileAsync(User user, UserProfile profile)
        {
            if (user == null || profile == null)
                throw new ArgumentNullException("User or UserProfile cannot be null.");

            try
            {
                user.Profile = profile;
                return await _userRepository.AddUserAsync(user);
            }
            catch
            {
                return false;
            }
        }
    }
}
