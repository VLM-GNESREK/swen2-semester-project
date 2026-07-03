using Moq;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using TourPlanner.BL.Services;
using TourPlanner.DAL.Repositories;
using TourPlanner.DAL.Entities;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Exceptions;

namespace TourPlanner.Tests.BL
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IConfiguration> _mockConfiguration;
        private UserService _userService;

        private const string ValidSecretKey = "super-secret-key-that-is-at-least-256-bits-long-for-hmac";

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"]).Returns(ValidSecretKey);
            _mockConfiguration.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");

            _userService = new UserService(_mockUserRepository.Object, _mockConfiguration.Object);
        }

        private string ComputeExpectedHash(string password, string secret)
        {
            var keyEncoded = Encoding.UTF8.GetBytes(secret);
            using var hmac = new HMACSHA256(keyEncoded);
            var result = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(result);
        }

        [TestCase(false, "Valid1Password!123", false, false, (string?)null)] 
        [TestCase(false, "Exact16Pass!1234", false, false, (string?)null)]
        [TestCase(true, "Valid1Password!123", false, false, "Conflict: Username already exists. (BL30)")] 
        [TestCase(false, "Short1!a", false, false, "Bad Request: Password must be at least 16 characters long. (BL31)")] 
        [TestCase(false, "Short1Pass!1234", false, false, "Bad Request: Password must be at least 16 characters long. (BL31)")]
        [TestCase(false, "nouppercasepassword!123", false, false, "Bad Request: Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. (BL32)")] 
        [TestCase(false, "NOLOWERCASEPASSWORD!123", false, false, "Bad Request: Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. (BL32)")] 
        [TestCase(false, "NoDigitPassword!!!", false, false, "Bad Request: Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. (BL32)")] 
        [TestCase(false, "NoSpecialCharPassword123", false, false, "Bad Request: Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. (BL32)")] 
        [TestCase(false, "Valid1Password!123", false, true, "JWT Key is missing from configuration.")] 
        [TestCase(false, "Valid1Password!123", true, false, "Failed to register user. (BL33)")] 
        public async Task RegisterUserAsync_Scenarios(bool userExists, string password, bool throwRepoException, bool removeJwtKey, string? expectedExceptionMsg)
        {
            var dto = new UserRegistrationDTO { Username = "TestUser", Password = password };
            var existingUser = userExists ? new User { username = "TestUser" } : null;

            _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(dto.Username)).ReturnsAsync(existingUser);

            if (throwRepoException)
            {
                _mockUserRepository.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                                   .ThrowsAsync(new DataAccessException("DB Error"));
            }

            if (removeJwtKey)
            {
                _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"]).Returns((string?)null);
            }

            if (expectedExceptionMsg != null)
            {
                if (expectedExceptionMsg.Contains("JWT Key"))
                {
                    var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUserAsync(dto));
                    Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
                }
                else
                {
                    var ex = Assert.ThrowsAsync<BusinessException>(() => _userService.RegisterUserAsync(dto));
                    Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
                }
                
                if (!throwRepoException)
                {
                    _mockUserRepository.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
                }
            }
            else
            {
                await _userService.RegisterUserAsync(dto);

                string expectedHash = ComputeExpectedHash(dto.Password, ValidSecretKey);

                _mockUserRepository.Verify(r => r.AddUserAsync(It.Is<User>(u => 
                    u.username == dto.Username && 
                    u.pw_hash == expectedHash
                )), Times.Once);
            }
        }

        [TestCase(true, true, false, false, (string?)null)] 
        [TestCase(false, true, false, false, "Unauthorised: Invalid username or password. (BL34)")] 
        [TestCase(true, false, false, false, "Unauthorised: Invalid username or password. (BL34)")] 
        [TestCase(true, true, false, true, "JWT Key is missing from configuration.")] 
        [TestCase(true, true, true, false, "Failed to login user. (BL35)")] 
        public async Task LoginUserAsync_Scenarios(bool userExists, bool correctPassword, bool throwRepoException, bool removeJwtKey, string? expectedExceptionMsg)
        {
            var dto = new UserLoginDTO { Username = "TestUser", Password = "ProvidedPassword123!" };
            var dbPassword = correctPassword ? "ProvidedPassword123!" : "DifferentPassword123!";
            
            var existingUser = userExists ? new User 
            { 
                user_id = 1, 
                username = "TestUser", 
                pw_hash = ComputeExpectedHash(dbPassword, ValidSecretKey) 
            } : null;

            if (throwRepoException)
            {
                _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(dto.Username))
                                   .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(dto.Username))
                                   .ReturnsAsync(existingUser);
            }

            if (removeJwtKey)
            {
                _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"]).Returns((string?)null);
            }

            if (expectedExceptionMsg != null)
            {
                if (expectedExceptionMsg.Contains("JWT Key"))
                {
                    var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _userService.LoginUserAsync(dto));
                    Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
                }
                else
                {
                    var ex = Assert.ThrowsAsync<BusinessException>(() => _userService.LoginUserAsync(dto));
                    Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
                }
            }
            else
            {
                var result = await _userService.LoginUserAsync(dto);

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Username, Is.EqualTo(dto.Username));
                Assert.That(string.IsNullOrWhiteSpace(result.Token), Is.False);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(result.Token);

                var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var uniqueNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
                var hasJtiClaim = jwtToken.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Jti);

                Assert.That(subClaim, Is.EqualTo("1"));
                Assert.That(uniqueNameClaim, Is.EqualTo("TestUser"));
                Assert.That(hasJtiClaim, Is.True);
                Assert.That(jwtToken.Issuer, Is.EqualTo("TestIssuer"));
                Assert.That(jwtToken.Audiences.First(), Is.EqualTo("TestAudience"));
                Assert.That(jwtToken.ValidTo, Is.EqualTo(DateTime.UtcNow.AddHours(2)).Within(TimeSpan.FromSeconds(5)));
            }
        }
    }
}