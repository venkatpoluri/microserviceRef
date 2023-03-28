namespace TradingPartnerManagement.Domain.TradingPartners.Services;

using TradingPartnerManagement.Domain.TradingPartners;
using TradingPartnerManagement.Databases;
using TradingPartnerManagement.Services;

public interface ITradingPartnerRepository : IGenericRepository<TradingPartner>
{
}

public class TradingPartnerRepository : GenericRepository<TradingPartner>, ITradingPartnerRepository
{
    private readonly TradingPartnerDbContext _dbContext;

    public TradingPartnerRepository(TradingPartnerDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
