namespace UserManagement.DTOs
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }

        public string TableName { get; set; }

        public string Operation { get; set; }

        public DateTime ChangedAt { get; set; }

        public Guid UserId { get; set; }

        public string Username { get; set; }
    }
}