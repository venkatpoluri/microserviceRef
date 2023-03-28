namespace TradingPartnerManagement.Domain;

using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class BaseEntity
{
    [Key]
    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Guid Id { get; protected set; } = Guid.NewGuid();
    public virtual DateTime CreatedOn { get; protected set; }
    public virtual string CreatedBy { get; protected set; }
    public virtual DateTime? LastModifiedOn { get; protected set; }
    public virtual string LastModifiedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
    
    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new List<DomainEvent>();

    public void UpdateCreationProperties(DateTime createdOn, string createdBy)
    {
        CreatedOn = createdOn;
        CreatedBy = createdBy;
    }
    
    public void UpdateModifiedProperties(DateTime? lastModifiedOn, string lastModifiedBy)
    {
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }
    
    public void UpdateIsDeleted(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }
    
    public void QueueDomainEvent(DomainEvent @event)
    {
        if(!DomainEvents.Contains(@event))
            DomainEvents.Add(@event);
    }
}