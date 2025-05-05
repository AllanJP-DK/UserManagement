using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditLogsController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        // GET: api/AuditLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] Guid? userId,
            [FromQuery] string timeRange = null,
            [FromQuery] string timeZone = "UTC",
            [FromQuery] int limit = 100)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsAsync(
                    userId, startDate, endDate, timeRange, timeZone, limit);

                var auditLogDtos = auditLogs.Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    TableName = a.TableName,
                    Operation = a.Operation,
                    ChangedAt = a.ChangedAt,
                    UserId = a.UserId,
                    Username = a.User?.Username ?? "Unknown",
                }).ToList();

                return Ok(auditLogDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/AuditLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLogDto>> GetAuditLog(Guid id)
        {
            var auditLog = await _auditService.GetAuditLogByIdAsync(id);

            if (auditLog == null)
            {
                return NotFound();
            }

            var auditLogDto = new AuditLogDto
            {
                Id = auditLog.Id,
                TableName = auditLog.TableName,
                Operation = auditLog.Operation,
                ChangedAt = auditLog.ChangedAt,
                UserId = auditLog.UserId,
                Username = auditLog.User?.Username ?? "Unknown"
            };

            return Ok(auditLogDto);
        }

        // GET: api/AuditLogs/ByUser/{userId}
        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUser(
            Guid userId,
            [FromQuery] int limit = 100)
        {
            var auditLogs = await _auditService.GetByUserIdAsync(userId, limit);

            var auditLogDtos = auditLogs.Select(a => new AuditLogDto
            {
                Id = a.Id,
                TableName = a.TableName,
                Operation = a.Operation,
                ChangedAt = a.ChangedAt,
                UserId = a.UserId,
                Username = a.User?.Username ?? "Unknown"
            }).ToList();

            return Ok(auditLogDtos);
        }

        // GET: api/AuditLogs/ByTimeInterval
        [HttpGet("ByTimeInterval")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByTimeInterval(
            [FromQuery] string timeRange = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string timeZone = "UTC",
            [FromQuery] int limit = 100)
        {
            try
            {
                var auditLogs = await _auditService.GetByTimeIntervalAsync(
                    timeRange, startDate, endDate, timeZone, limit);

                var auditLogDtos = auditLogs.Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    TableName = a.TableName,
                    Operation = a.Operation,
                    ChangedAt = a.ChangedAt,
                    UserId = a.UserId,
                    Username = a.User?.Username ?? "Unknown"
                }).ToList();

                return Ok(auditLogDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/AuditLogs/ByUserAndTimeInterval/{userId}
        [HttpGet("ByUserAndTimeInterval/{userId}")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUserAndTimeInterval(
            Guid userId,
            [FromQuery] string timeRange = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string timeZone = "UTC",
            [FromQuery] int limit = 100)
        {
            try
            {
                var auditLogs = await _auditService.GetByUserAndTimeIntervalAsync(
                    userId, timeRange, startDate, endDate, timeZone, limit);

                var auditLogDtos = auditLogs.Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    TableName = a.TableName,
                    Operation = a.Operation,
                    ChangedAt = a.ChangedAt,
                    UserId = a.UserId,
                    Username = a.User?.Username ?? "Unknown"
                }).ToList();

                return Ok(auditLogDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/AuditLogs/ByOperation/{operation}
        [HttpGet("ByOperation/{operation}")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByOperation(
            string operation,
            [FromQuery] string timeRange = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string timeZone = "UTC",
            [FromQuery] Guid? userId = null,
            [FromQuery] int limit = 100)
        {
            try
            {
                var auditLogs = await _auditService.GetByOperationAsync(
                    operation, userId, timeRange, startDate, endDate, timeZone, limit);

                var auditLogDtos = auditLogs.Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    TableName = a.TableName,
                    Operation = a.Operation,
                    ChangedAt = a.ChangedAt,
                    UserId = a.UserId,
                    Username = a.User?.Username ?? "Unknown"
                }).ToList();

                return Ok(auditLogDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/AuditLogs/ByTable/{tableName}
        [HttpGet("ByTable/{tableName}")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByTable(
            string tableName,
            [FromQuery] string timeRange = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string timeZone = "UTC",
            [FromQuery] Guid? userId = null,
            [FromQuery] int limit = 100)
        {
            try
            {
                var auditLogs = await _auditService.GetByTableNameAsync(
                    tableName, userId, timeRange, startDate, endDate, timeZone, limit);

                var auditLogDtos = auditLogs.Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    TableName = a.TableName,
                    Operation = a.Operation,
                    ChangedAt = a.ChangedAt,
                    UserId = a.UserId,
                    Username = a.User?.Username ?? "Unknown"
                }).ToList();

                return Ok(auditLogDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}