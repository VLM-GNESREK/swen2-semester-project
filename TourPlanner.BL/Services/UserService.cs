using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TourPlanner.DAL.Repositories;
using TourPlanner.DAL.Entities;
using TourPlanner.DAL.Exceptions;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task RegisterUserAsync(UserRegistrationDTO registrationDto)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByUsernameAsync(registrationDto.Username);
                if (existingUser != null)
                {
                    throw new BusinessException("Conflict: Username already exists. (BL30)");
                }

                if (registrationDto.Password.Length < 16)
                {
                    throw new BusinessException("Bad Request: Password must be at least 16 characters long. (BL31)");
                }

                var complexityPattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$");
                if (!complexityPattern.IsMatch(registrationDto.Password))
                {
                    throw new BusinessException(
                        "Bad Request: Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. (BL32)");
                }
                var jwtKey = _configuration["JwtSettings:SecretKey"] ??
                             throw new InvalidOperationException("JWT Key is missing from configuration.");
                var keyEncoded =Encoding.UTF8.GetBytes(jwtKey);
                //string passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password,jwtKey );
                var hmac = new HMACSHA256(keyEncoded);
                
                var result = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registrationDto.Password));
                var passwordHash = Convert.ToBase64String(result);
                
                Console.WriteLine("HASHED PASSWORD " + passwordHash);

                var newUser = new User
                {
                    username = registrationDto.Username,
                    pw_hash = passwordHash
                };

                await _userRepository.AddUserAsync(newUser);
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to register user. (BL33)", ex);
            }
        }

        public async Task<AuthResponseDTO> LoginUserAsync(UserLoginDTO loginDto)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
                var jwtKey = _configuration["JwtSettings:SecretKey"] ??
                             throw new InvalidOperationException("JWT Key is missing from configuration.");
                byte[] keyBytes = Encoding.UTF8.GetBytes(jwtKey);
                var hmac = new HMACSHA256(keyBytes);

                var result = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));
                if (user == null || Convert.ToBase64String(result) != user!.pw_hash)
                {
                    Console.WriteLine("PASSWORD HASH: "+user!.pw_hash);
                    Console.WriteLine("RESULT OF HASH: " + Convert.ToBase64String(result));
                    throw new BusinessException("Unauthorised: Invalid username or password. (BL34)");
                }
                /*
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.pw_hash))
                {
                    throw new BusinessException("Unauthorised: Invalid username or password. (BL34)");
                }
                */

                string token = GenerateJwtToken(user);

                return new AuthResponseDTO
                {
                    Token = token,
                    Username = user.username,
                };
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to login user. (BL35)", ex);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["JwtSettings:SecretKey"] ??
                         throw new InvalidOperationException("JWT Key is missing from configuration.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.user_id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = credentials,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}