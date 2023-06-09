namespace TradingPartnerManagement.Services;

using System.Security.Claims;

public interface ICurrentUserService : ITradingPartnerManagementService
{
    string? UserId { get; }
    ClaimsPrincipal? User { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
}