namespace TradingPartnerManagement.Services;

using TradingPartnerManagement.Databases;

public interface IUnitOfWork : ITradingPartnerManagementService
{
    Task CommitChanges(CancellationToken cancellationToken = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly TradingPartnerDbContext _dbContext;

    public UnitOfWork(TradingPartnerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitChanges(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
