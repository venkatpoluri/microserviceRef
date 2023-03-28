namespace TradingPartnerManagement.Services;

using TradingPartnerManagement.Domain;
using TradingPartnerManagement.Databases;
using SharedKernel.Exceptions;
using Microsoft.EntityFrameworkCore;

public interface IGenericRepository<TEntity> : ITradingPartnerManagementService
    where TEntity : BaseEntity
{
    IQueryable<TEntity> Query();
    IQueryable<TEntity> QueryInclude(string[] includes);
    Task<TEntity> GetByIdOrDefault(Guid id, bool withTracking = true, CancellationToken cancellationToken = default);
    Task<TEntity> GetById(Guid id, bool withTracking = true, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
    Task Add(TEntity entity, CancellationToken cancellationToken = default);    
    Task AddRange(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default);    
    void Update(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entity);
}

public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> 
    where TEntity : BaseEntity
{
    private readonly TradingPartnerDbContext _dbContext;

    protected GenericRepository(TradingPartnerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }
    
    public virtual IQueryable<TEntity> Query()
    {
        return _dbContext.Set<TEntity>();
    }

    public virtual IQueryable<TEntity> QueryInclude(String[] includes)
    {
        IQueryable<TEntity> query = _dbContext.Set<TEntity>();
        foreach(var include in includes )
        {
            if(string.IsNullOrEmpty(include)) continue;
            query = query.Include(include);
        }
        return query;
    }

    public virtual async Task<TEntity> GetByIdOrDefault(Guid id, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        return withTracking 
            ? await _dbContext.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken) 
            : await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<TEntity> GetById(Guid id, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdOrDefault(id, withTracking, cancellationToken);
        
        if(entity == null)
            throw new NotFoundException($"{typeof(TEntity).Name} with an id '{id}' was not found.");

        return entity;
    }

    public virtual async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>()
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task Add(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbContext.Set<TEntity>().RemoveRange(entities);
    }
}
