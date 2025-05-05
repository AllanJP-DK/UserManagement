namespace UserManagement.Services
{
    public interface ICurrentUserService
    {
        Guid? GetCurrentUserId();
        string GetCurrentUsername();
    }
}