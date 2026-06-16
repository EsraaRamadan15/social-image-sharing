namespace Application.Abstractions
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? SessionId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
    }
}
