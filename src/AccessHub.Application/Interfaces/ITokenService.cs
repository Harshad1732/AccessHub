using AccessHub.Domain.Entities;

namespace AccessHub.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}
