using Dotnet10AISamples.Api.Data;
using Dotnet10AISamples.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet10AISamples.Api.Repositories;

/// <summary>
/// 角色資料存取實作
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetUserRolesAsync(string userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> RoleExistsAsync(string roleId)
    {
        return await _context.Roles.AnyAsync(r => r.Id == roleId);
    }

    public async Task<bool> UserHasRoleAsync(string userId, string roleId)
    {
        return await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<int> AssignRoleToUserAsync(string userId, string roleId, string assignedBy)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        };

        _context.UserRoles.Add(userRole);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> RemoveRoleFromUserAsync(string userId, string roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole == null)
        {
            return 0;
        }

        _context.UserRoles.Remove(userRole);
        return await _context.SaveChangesAsync();
    }

    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _context.Roles
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Role> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }
}