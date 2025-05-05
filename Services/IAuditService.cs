using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IAuditService
    {
        Task LogAuditAsync(string tableName, string operation, Guid userId);
        
        Task<AuditLog> GetAuditLogByIdAsync(Guid id);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(
            Guid? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeRange = null,
            string timeZone = "UTC",
            int limit = 100);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(
            Guid userId,
            int limit = 100);
        Task<IEnumerable<AuditLog>> GetByTimeIntervalAsync(
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100);
        Task<IEnumerable<AuditLog>> GetByUserAndTimeIntervalAsync(
            Guid userId,
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100);
        Task<IEnumerable<AuditLog>> GetByOperationAsync(
            string operation,
            Guid? userId = null,
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100);
        Task<IEnumerable<AuditLog>> GetByTableNameAsync(
            string tableName,
            Guid? userId = null,
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100);
    }
}