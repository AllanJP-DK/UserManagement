using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.Repositories
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int limit = 100)
        {
            // Using the userid index for efficiency
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .OrderByDescending(a => a.ChangedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByTimeIntervalAsync(DateTime startDate, DateTime endDate, int limit = 100)
        {
            // Using the changed_at index for efficiency
            return await _context.AuditLogs
                .Where(a => a.ChangedAt >= startDate && a.ChangedAt <= endDate)
                .Include(a => a.User)
                .OrderByDescending(a => a.ChangedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAndTimeIntervalAsync(Guid userId, DateTime startDate, DateTime endDate, int limit = 100)
        {
            // Using both indexes for efficiency
            return await _context.AuditLogs
                .Where(a => a.UserId == userId && a.ChangedAt >= startDate && a.ChangedAt <= endDate)
                .Include(a => a.User)
                .OrderByDescending(a => a.ChangedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByOperationAsync(string operation, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null, int limit = 100)
        {
            IQueryable<AuditLog> query = _context.AuditLogs
                .Where(a => a.Operation.ToLower() == operation.ToLower());

            // Apply filters that can use indexes
            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(a => a.ChangedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.ChangedAt <= endDate.Value);
            }

            // Limit results and include user data
            return await query
                .Include(a => a.User)
                .OrderByDescending(a => a.ChangedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByTableNameAsync(string tableName, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null, int limit = 100)
        {
            IQueryable<AuditLog> query = _context.AuditLogs
                .Where(a => a.TableName.ToLower() == tableName.ToLower());

            // Apply filters that can use indexes
            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(a => a.ChangedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.ChangedAt <= endDate.Value);
            }

            // Limit results and include user data
            return await query
                .Include(a => a.User)
                .OrderByDescending(a => a.ChangedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}