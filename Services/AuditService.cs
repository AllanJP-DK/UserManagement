using UserManagement.Models;
using UserManagement.Repositories;

namespace UserManagement.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IGenericRepository<AuditLog> _genericRepository;

        public AuditService(
            IAuditLogRepository auditLogRepository,
            IGenericRepository<AuditLog> genericRepository)
        {
            _auditLogRepository = auditLogRepository;
            _genericRepository = genericRepository;
        }

        public async Task LogAuditAsync(string tableName, string operation, Guid userId)
        {
            var auditLog = new AuditLog
            {
                TableName = tableName,
                Operation = operation,
                ChangedAt = DateTime.UtcNow,
                UserId = userId
            };

            await _genericRepository.AddAsync(auditLog);
            await _genericRepository.SaveChangesAsync();
        }

        public async Task<AuditLog> GetAuditLogByIdAsync(Guid id)
        {
            return await _auditLogRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(
            Guid? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeRange = null,
            string timeZone = "UTC",
            int limit = 100)
        {
            // Handle different filtering combinations
            if (userId.HasValue && (startDate.HasValue || !string.IsNullOrEmpty(timeRange)))
            {
                return await GetByUserAndTimeIntervalAsync(
                    userId.Value, timeRange, startDate, endDate, timeZone, limit);
            }
            else if (startDate.HasValue || !string.IsNullOrEmpty(timeRange))
            {
                return await GetByTimeIntervalAsync(
                    timeRange, startDate, endDate, timeZone, limit);
            }
            else if (userId.HasValue)
            {
                return await GetByUserIdAsync(userId.Value, limit);
            }
            else
            {
                // Default case - get latest logs
                var logs = await _auditLogRepository.GetAllAsync();
                return logs.OrderByDescending(a => a.ChangedAt).Take(limit);
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(
            Guid userId,
            int limit = 100)
        {
            return await _auditLogRepository.GetByUserIdAsync(userId, limit);
        }

        public async Task<IEnumerable<AuditLog>> GetByTimeIntervalAsync(
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100)
        {
            try
            {
                var (effectiveStartDate, effectiveEndDate) = CalculateDateRange(
                    timeRange, startDate, endDate, timeZone);

                return await _auditLogRepository.GetByTimeIntervalAsync(
                    effectiveStartDate, effectiveEndDate, limit);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAndTimeIntervalAsync(
            Guid userId,
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100)
        {
            try
            {
                var (effectiveStartDate, effectiveEndDate) = CalculateDateRange(
                    timeRange, startDate, endDate, timeZone);

                return await _auditLogRepository.GetByUserAndTimeIntervalAsync(
                    userId, effectiveStartDate, effectiveEndDate, limit);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByOperationAsync(
            string operation,
            Guid? userId = null,
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100)
        {
            try
            {
                DateTime? effectiveStartDate = null;
                DateTime? effectiveEndDate = null;

                if (!string.IsNullOrEmpty(timeRange) || startDate.HasValue || endDate.HasValue)
                {
                    var dateRange = CalculateDateRange(timeRange, startDate, endDate, timeZone);
                    effectiveStartDate = dateRange.start;
                    effectiveEndDate = dateRange.end;
                }

                return await _auditLogRepository.GetByOperationAsync(
                    operation, effectiveStartDate, effectiveEndDate, userId, limit);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByTableNameAsync(
            string tableName,
            Guid? userId = null,
            string timeRange = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string timeZone = "UTC",
            int limit = 100)
        {
            try
            {
                DateTime? effectiveStartDate = null;
                DateTime? effectiveEndDate = null;

                if (!string.IsNullOrEmpty(timeRange) || startDate.HasValue || endDate.HasValue)
                {
                    var dateRange = CalculateDateRange(timeRange, startDate, endDate, timeZone);
                    effectiveStartDate = dateRange.start;
                    effectiveEndDate = dateRange.end;
                }

                return await _auditLogRepository.GetByTableNameAsync(
                    tableName, effectiveStartDate, effectiveEndDate, userId, limit);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private (DateTime start, DateTime end) CalculateDateRange(
            string timeRange, DateTime? startDate, DateTime? endDate, string timeZone)
        {
            DateTime effectiveStartDate;
            DateTime effectiveEndDate;

            // Handle predefined time ranges
            if (!string.IsNullOrEmpty(timeRange))
            {
                switch (timeRange.ToLowerInvariant())
                {
                    case "today":
                        effectiveStartDate = DateTime.UtcNow.Date;
                        effectiveEndDate = DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);
                        break;
                    case "yesterday":
                        effectiveStartDate = DateTime.UtcNow.Date.AddDays(-1);
                        effectiveEndDate = DateTime.UtcNow.Date.AddSeconds(-1);
                        break;
                    case "last7days":
                        effectiveStartDate = DateTime.UtcNow.Date.AddDays(-7);
                        effectiveEndDate = DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);
                        break;
                    case "thismonth":
                        effectiveStartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        effectiveEndDate = effectiveStartDate.AddMonths(1).AddSeconds(-1);
                        break;
                    case "lastmonth":
                        var firstDayOfThisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        effectiveStartDate = firstDayOfThisMonth.AddMonths(-1);
                        effectiveEndDate = firstDayOfThisMonth.AddSeconds(-1);
                        break;
                    default:
                        throw new ArgumentException(
                            "Invalid timeRange value. Valid values are: today, yesterday, last7days, thismonth, lastmonth");
                }
            }
            else if (startDate.HasValue && endDate.HasValue)
            {
                // Apply time zone adjustments if needed
                try
                {
                    var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

                    // If dates were provided without time (midnight), treat as full day ranges
                    if (startDate.Value.TimeOfDay.TotalSeconds == 0)
                    {
                        // Use date only - start of day in specified time zone
                        var localStartDate = new DateTime(
                            startDate.Value.Year, startDate.Value.Month, startDate.Value.Day,
                            0, 0, 0, DateTimeKind.Unspecified);

                        // Convert to UTC for database storage
                        effectiveStartDate = TimeZoneInfo.ConvertTimeToUtc(localStartDate, timezone);
                    }
                    else
                    {
                        // Assume the date is already in UTC if time was specified
                        effectiveStartDate = startDate.Value;
                    }

                    if (endDate.Value.TimeOfDay.TotalSeconds == 0)
                    {
                        // Use date only - set to end of day in the specified time zone
                        var localEndDate = new DateTime(
                            endDate.Value.Year, endDate.Value.Month, endDate.Value.Day,
                            23, 59, 59, DateTimeKind.Unspecified);

                        // Convert to UTC for database storage
                        effectiveEndDate = TimeZoneInfo.ConvertTimeToUtc(localEndDate, timezone);
                    }
                    else
                    {
                        // Assume the date is already in UTC if time was specified
                        effectiveEndDate = endDate.Value;
                    }
                }
                catch (TimeZoneNotFoundException)
                {
                    throw new ArgumentException($"Invalid time zone: {timeZone}");
                }

                if (effectiveStartDate > effectiveEndDate)
                {
                    throw new ArgumentException("Start date must be before or equal to end date.");
                }
            }
            else
            {
                throw new ArgumentException("Either timeRange or both startDate and endDate must be specified.");
            }

            return (effectiveStartDate, effectiveEndDate);
        }
    }
}