using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _dbContext;

        public UserRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _dbContext.AddAsync(user,ct);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email,ct);
        }
    }
}
