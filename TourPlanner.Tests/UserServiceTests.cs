using Moq;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
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

        private const string Secret = "super-secret-key-that-is-at-least-256-bits-long-for-hmac";

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"]).Returns(Secret);
            _mockConfiguration.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");

            _userService = new UserService(_mockUserRepository.Object, _mockConfiguration.Object);
        }

        private static string Hash(string password)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        // -------------------------
        // WE REGIST IT SHOULD WORK
        // -------------------------

        [Test]
        public async Task RegisterUser_ShouldSucceed()
        {
            var dto = new UserRegistrationDTO
            {
                Username = "test",
                Password = "VeryStrongPassword123!"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);

            await _userService.RegisterUserAsync(dto);

            _mockUserRepository.Verify(r =>
                r.AddUserAsync(It.Is<User>(u =>
                    u.username == dto.Username &&
                    u.pw_hash == Hash(dto.Password)
                )), Times.Once);
        }
        // USER ALERADY EXISTS
        [Test]
        public void RegisterUser_ShouldFail_WhenUserExists()
        {
            var dto = new UserRegistrationDTO
            {
                Username = "test",
                Password = "VeryStrongPassword123!"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync(new User());

            Assert.ThrowsAsync<BusinessException>(() =>
                _userService.RegisterUserAsync(dto));
        }
        //PASSWORD TO WEAK / SHORT / WHY DID WE USE 16 CHARS??
        [Test]
        public void RegisterUser_ShouldFail_WhenPasswordTooWeak()
        {
            var dto = new UserRegistrationDTO
            {
                Username = "test",
                Password = "weakpass"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);

            Assert.ThrowsAsync<BusinessException>(() =>
                _userService.RegisterUserAsync(dto));
        }
        //THIS SHOULD NEVER HAPPEN - I HOPE
        [Test]
        public void RegisterUser_ShouldFail_WhenJwtMissing()
        {
            var dto = new UserRegistrationDTO
            {
                Username = "test",
                Password = "VeryStrongPassword123!"
            };

            _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"])
                .Returns((string?)null);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.RegisterUserAsync(dto));
        }
        // REPO DOWN
        [Test]
        public void RegisterUser_ShouldFail_OnRepoError()
        {
            var dto = new UserRegistrationDTO
            {
                Username = "test",
                Password = "VeryStrongPassword123!"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);

            _mockUserRepository
                .Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .ThrowsAsync(new DataAccessException("db"));

            Assert.ThrowsAsync<BusinessException>(() =>
                _userService.RegisterUserAsync(dto));
        }

        // -------------------------
        // LOGIN WORKS GIMME TOKEN
        // -------------------------

        [Test]
        public async Task LoginUser_ShouldReturnToken()
        {
            var dto = new UserLoginDTO
            {
                Username = "test",
                Password = "Password123!"
            };

            var user = new User
            {
                user_id = 1,
                username = dto.Username,
                pw_hash = Hash(dto.Password)
            };

            _mockUserRepository
                .Setup(r => r.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync(user);

            var result = await _userService.LoginUserAsync(dto);

            Assert.That(result.Username, Is.EqualTo(dto.Username));
            Assert.That(result.Token, Is.Not.Null.And.Not.Empty);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

            Assert.That(jwt.Claims.First(c => c.Type == "sub").Value, Is.EqualTo("1"));
            Assert.That(jwt.Claims.First(c => c.Type == "unique_name").Value, Is.EqualTo(dto.Username));
        }
        // WRONG PW / USERNAME
        [Test]
        public void LoginUser_ShouldFail_WhenInvalidCredentials()
        {
            var dto = new UserLoginDTO
            {
                Username = "test",
                Password = "wrong"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync(new User
                {
                    user_id = 1,
                    username = dto.Username,
                    pw_hash = Hash("correct")
                });

            Assert.ThrowsAsync<BusinessException>(() =>
                _userService.LoginUserAsync(dto));
        }
        //THAT SHOULD NOT HAPPEN NONOONONO
        //WELL IT MAY HAPPEN IF ENVIRONMENT VARIABLE IS NOT SET
        [Test]
        public void LoginUser_ShouldFail_WhenJwtMissing()
        {
            var dto = new UserLoginDTO
            {
                Username = "test",
                Password = "Password123!"
            };

            _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"])
                .Returns((string?)null);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.LoginUserAsync(dto));
        }
        // REPO DOWN
        [Test]
        public void LoginUser_ShouldFail_OnRepoError()
        {
            var dto = new UserLoginDTO
            {
                Username = "test",
                Password = "Password123!"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByUsernameAsync(dto.Username))
                .ThrowsAsync(new DataAccessException("db"));

            Assert.ThrowsAsync<BusinessException>(() =>
                _userService.LoginUserAsync(dto));
        }
    }
}