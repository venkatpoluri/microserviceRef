using Sieve.Services;
using TradingPartnerManagement.Domain.SupplyAgreements;


public static class SieveServiceExtension 
{

    public static void AddSieve(this IServiceCollection services)
    {
        services.AddScoped<SieveProcessor>();
        services.AddScoped<ISieveCustomFilterMethods, SieveCustomFilterMethods>();
    }
}
public class SieveCustomFilterMethods : ISieveCustomFilterMethods
{
    public IQueryable<SupplyAgreementLogicalDto> IsExpiringIn30Days(IQueryable<SupplyAgreementLogicalDto> source, string op, string[] values) 
    {
        return source.Where(sa => sa.ThruDate > DateTime.Now && sa.ThruDate <= DateTime.Now.AddMonths(1));
    }

    public IQueryable<SupplyAgreementLogicalDto> IsExpiringIn3060Days(IQueryable<SupplyAgreementLogicalDto> source, string op, string[] values) 
    {
        return source.Where(sa => sa.ThruDate > DateTime.Now.AddMonths(1) && sa.ThruDate <= DateTime.Now.AddMonths(2));
    }
    public IQueryable<SupplyAgreementLogicalDto> IsExpiringIn60Days(IQueryable<SupplyAgreementLogicalDto> source, string op, string[] values) 
    {
        return source.Where(sa => sa.ThruDate > DateTime.Now.AddMonths(2));
    }
    public IQueryable<SupplyAgreementLogicalDto> Expired30Days(IQueryable<SupplyAgreementLogicalDto> source, string op, string[] values) 
    {
        return source.Where(sa => sa.ThruDate < DateTime.Now && sa.ThruDate >= DateTime.Now.AddMonths(-1));
    }

    public IQueryable<SupplyAgreementLogicalDto> Expired3060Days(IQueryable<SupplyAgreementLogicalDto> source, string op, string[] values) 
    {
        return source.Where(sa => sa.ThruDate < DateTime.Now.AddMonths(-1) && sa.ThruDate >= DateTime.Now.AddMonths(-2));
    }
    public IQueryable<SupplyAgreementLogicalDto> Expired60Days(IQueryable<SupplyAgreementLogicalDto> source, string op, string[] values) 
    {
        return source.Where(sa => sa.ThruDate < DateTime.Now.AddMonths(-2));

    }

}