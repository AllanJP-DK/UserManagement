using UserManagement.Models;

namespace UserManagement.Repositories
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int limit = 100);
        Task<IEnumerable<AuditLog>> GetByTimeIntervalAsync(DateTime startDate, DateTime endDate, int limit = 100);
        Task<IEnumerable<AuditLog>> GetByUserAndTimeIntervalAsync(Guid userId, DateTime startDate, DateTime endDate, int limit = 100);
        Task<IEnumerable<AuditLog>> GetByOperationAsync(string operation, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null, int limit = 100);
        Task<IEnumerable<AuditLog>> GetByTableNameAsync(string tableName, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null, int limit = 100);
    }
}