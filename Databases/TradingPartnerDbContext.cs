namespace TradingPartnerManagement.Databases;

using TradingPartnerManagement.Domain;
using TradingPartnerManagement.Services;
using TradingPartnerManagement.Domain.Concepts;
using TradingPartnerManagement.Domain.TradingPartners;
using TradingPartnerManagement.Domain.Facilities;
using TradingPartnerManagement.Domain.Contacts;
using TradingPartnerManagement.Domain.SupplyAgreements;
using Innofactor.EfCoreJsonValueConverter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using TradingPartnerManagement.Resources;

public class TradingPartnerDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _env;

    public TradingPartnerDbContext(
        DbContextOptions<TradingPartnerDbContext> options, ICurrentUserService currentUserService, IMediator mediator, IWebHostEnvironment env) : base(options)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
        _env = env;
    }

    #region DbSet Region - Do Not Delete

    public DbSet<Concept> Concepts { get; set; }
    public DbSet<TradingPartner> TradingPartners { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<SupplyAgreement> SupplyAgreements { get; set; }
    #endregion DbSet Region - Do Not Delete

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.FilterSoftDeletedRecords();
        /* any query filters added after this will override soft delete 
                https://docs.microsoft.com/en-us/ef/core/querying/filters
                https://github.com/dotnet/efcore/issues/10275
        */
        if (_env.IsEnvironment(LocalConfig.FunctionalTestingEnvName))
        {
            modelBuilder.Ignore<TradingPartnerManagement.Domain.TradingPartners.Dtos.TradingPartnerDocument>();
modelBuilder.Ignore<TradingPartnerManagement.Domain.Facilities.Dtos.FacilityDocument>();
modelBuilder.Ignore<TradingPartnerManagement.Domain.Contacts.Dtos.ContactDocument>();
modelBuilder.Ignore<TradingPartnerManagement.Domain.SupplyAgreements.Dtos.SupplyAgreementDocument>();

modelBuilder.AddJsonFields();
        }
        
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        var result = base.SaveChanges();
        _dispatchDomainEvents().GetAwaiter().GetResult();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        UpdateAuditFields();
        var result = await base.SaveChangesAsync(cancellationToken);
        await _dispatchDomainEvents();
        return result;
    }
    
    private async Task _dispatchDomainEvents()
    {
        var domainEventEntities = ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            foreach (var entityDomainEvent in entity.DomainEvents)
                await _mediator.Publish(entityDomainEvent);
        }
    }
        
    private void UpdateAuditFields()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.UpdateCreationProperties(now, _currentUserService?.UserId);
                    entry.Entity.UpdateModifiedProperties(now, _currentUserService?.UserId);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdateModifiedProperties(now, _currentUserService?.UserId);
                    break;
                
                case EntityState.Deleted:
                if(entry.Entity is  ISoftDelete)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.UpdateModifiedProperties(now, _currentUserService?.UserId);
                    entry.Entity.UpdateIsDeleted(true);break;}
                else
                {entry.State = EntityState.Deleted;break;}
                    
            }
        }
    }
}

public static class Extensions
{
    public static void FilterSoftDeletedRecords(this ModelBuilder modelBuilder)
    {
        Expression<Func<BaseEntity, bool>> filterExpr = e => !e.IsDeleted;
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes()
            .Where(m => m.ClrType.IsAssignableTo(typeof(BaseEntity))))
        {
            // modify expression to handle correct child type
            var parameter = Expression.Parameter(mutableEntityType.ClrType);
            var body = ReplacingExpressionVisitor
                .Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
            var lambdaExpression = Expression.Lambda(body, parameter);

            // set filter
            mutableEntityType.SetQueryFilter(lambdaExpression);
        }
    }
}