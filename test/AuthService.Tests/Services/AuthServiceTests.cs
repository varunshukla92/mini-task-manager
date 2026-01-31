using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthSvc =  AuthService.Application.Services.AuthService;
using AuthService.Tests.Helpers;
using AuthService.Domain.Entities;
using FluentAssertions;
using Moq;


namespace AuthService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasherService> _passwordHasherMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;

        private readonly AuthSvc _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasherService>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();

            _authService = new AuthSvc(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object
        );
        }

        [Fact]
        
        public async Task LoginAsync_Should_Return_AuthResponse_When_Credentials_Are_Valid()
        {
            // Arrange
            var request = TestDataFactory.CreateLoginRequest();
            var user = TestDataFactory.CreateUser();

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email.ToLowerInvariant(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash!))
                .Returns(true);

            var token = "fake-jwt-token";
            var expiresAt = DateTime.UtcNow.AddHours(1);

            _jwtTokenServiceMock
               .Setup(j => j.GenerateToken(user))
               .Returns((token, expiresAt));

            // Act
            var result = await _authService.LoginAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
            result.Role.Should().Be(user.Role);
            result.AccessToken.Should().Be(token);
            result.ExpiresAt.Should().Be(expiresAt);
            result.Message.Should().Be("Login successful");

        }

        [Fact]
        public async Task LoginAsync_Should_Throw_Unauthorized_When_User_Does_Not_Exist()
        {
            // Arrange
            var request = TestDataFactory.CreateLoginRequest();

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email.ToLowerInvariant(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = () => _authService.LoginAsync(request, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials");
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_Unauthorized_When_Password_Is_Invalid()
        {
            // Arrange
            var request = TestDataFactory.CreateLoginRequest();

            var user = TestDataFactory.CreateUser();

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email.ToLowerInvariant(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash!))
                .Returns(false);

            // Act
            Func<Task> act = () => _authService.LoginAsync(request, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials");

            _jwtTokenServiceMock.Verify(
                j => j.GenerateToken(It.IsAny<User>()),
                Times.Never
            );
        }

        [Fact]
        public async Task RegisterAsync_Should_Create_User_When_Email_Does_Not_Exist()
        {
            // Arrange
            var request = TestDataFactory.CreateRegisterRequest();

            _userRepositoryMock
                .Setup(r => r.ExistsByEmailAsync(request.Email.ToLowerInvariant(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherMock
                .Setup(h => h.HashPassword(request.Password))
                .Returns("hashed-password");

            User? savedUser = null;

            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, _) => savedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email.ToLowerInvariant());
            result.Role.Should().Be("User");
            result.Message.Should().Be("User Registered Successfully");

            savedUser.Should().NotBeNull();
            savedUser!.Email.Should().Be(request.Email.ToLowerInvariant());
            savedUser.PasswordHash.Should().Be("hashed-password");
            savedUser.IsActive.Should().BeTrue();
        }

    }
}
