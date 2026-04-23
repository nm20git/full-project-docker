using Microsoft.AspNetCore.Http.HttpResults;
using project.BLL.Interfaces;
using project.DAL.Intefaces;
using project.Models;
using project.Models.DTO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace project.BLL
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDAL _userDAL;
        private readonly ILogger<UserBLL> _logger;

        public UserBLL(IUserDAL userDAL, ILogger<UserBLL> logger)
        {
            _userDAL = userDAL;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> Register(User user, string password)
        {
            _logger.LogInformation("BLL: Register attempt for Email: {Email}", user.Email);

            if (await _userDAL.GetByEmail(user.Email) != null)
            {
                _logger.LogWarning("BLL: Registration failed - Email already exists: {Email}", user.Email);
                return (false, "Email already exists");
            }

            CreatePasswordHash(password, out byte[] hash, out byte[] salt);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            await _userDAL.Add(user);

            _logger.LogInformation("BLL: User registered successfully. UserId: {UserId}, Email: {Email}",
                user.Id, user.Email);

            return (true, Message: "User registered successfully");
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public async Task<User?> Login(string email, string password)
        {
            _logger.LogInformation("BLL: Login attempt for Email: {Email}", email);

            var user = await _userDAL.GetByEmail(email);

            if (user == null)
            {
                _logger.LogWarning("BLL: Login failed - user not found. Email: {Email}", email);
                return null;
            }

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning("BLL: Login failed - invalid password. Email: {Email}", email);
                return null;
            }

            _logger.LogInformation("BLL: Login successful. UserId: {UserId}", user.Id);

            return user;
        }

        private bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(hash);
        }

        public async Task<List<Gift>> GetUserGifts(int userId)
        {
            _logger.LogInformation("BLL: Getting gifts for UserId: {UserId}", userId);

            var gifts = await _userDAL.GetUserGifts(userId);

            _logger.LogInformation("BLL: Retrieved {Count} gifts for UserId: {UserId}", gifts.Count, userId);

            return gifts;
        }
    }
}
