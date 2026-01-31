using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Tests.Helpers
{
    public static class TestDataFactory
    {
        public static LoginRequest CreateLoginRequest(
            string email = "test@user.com",
            string password = "password123")
        {
            return new LoginRequest
            {
                Email = email,
                Password = password
            };
        }

        public static RegisterRequest CreateRegisterRequest(
        string email = "new@user.com",
        string password = "password123")
        {
            return new RegisterRequest
            {
                Email = email,
                Password = password
            };
        }

        public static User CreateUser(
        string email = "test@user.com",
        string passwordHash = "hashed-password")
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = passwordHash,
                Role = "User",
                IsActive = true
            };
        }
    }
}
