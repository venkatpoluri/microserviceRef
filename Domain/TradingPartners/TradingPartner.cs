namespace TradingPartnerManagement.Domain.TradingPartners;

using SharedKernel.Exceptions;
using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Mappings;
using TradingPartnerManagement.Domain.TradingPartners.Validators;
using TradingPartnerManagement.Domain.TradingPartners.DomainEvents;
using AutoMapper;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Sieve.Attributes;
using TradingPartnerManagement.Domain.Concepts;
using TradingPartnerManagement.Extensions.Application;
public class TradingPartnerLogicalDto : TradingPartner
{
        [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Name { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string ShortName { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? DunsNum { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsSubsidiary { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual int Num { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? FederalEmployerIdent { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Guid Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
    public virtual RolesType? Type { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual StatusesStatus? Status { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual ICollection<Minority_vendors>? MinorityVendors { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Postal_addressesType? AddressType { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? Address_1 { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? Address_2 { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Postal_addressesState_type? StateType { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? State { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? City { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? StateCode { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? CountryName { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? CountryCode { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? PostalCode { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsPostalPrimary { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsDoNotContact { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Phone_addressesType? PhoneType { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? PhoneCountryCode { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? AreaCode { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? PhoneNumber { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? PhoneExtension { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsPhonePrimary { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsPhoneDoNotContact { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Electronic_email_addressesType? EmailType { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? Email { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsEmailPrimary { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsEmailDoNotContact { get; set; }
    
}


public class TradingPartner : BaseEntity ,ISoftDelete
{
    [Required]
    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Name { get; protected set; }

    [Required]
    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string ShortName { get; protected set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? DunsNum { get; protected set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual bool? IsSubsidiary { get; protected set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Guid? ExternalOid { get; protected set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual Guid? ParentExternalOid { get; protected set; }

    [Sieve(CanFilter = true, CanSort = true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual int Num { get; protected set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string? FederalEmployerIdent { get; protected set; }

    [Required]
    [JsonIgnore]
    [IgnoreDataMember]
    [ForeignKey("Concept")]
    public virtual Guid ConceptId { get; protected set; }
    public virtual Concept Concept { get; protected set; }

    [Required]
    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string ConceptKey { get; protected set; }

    [JsonIgnore]
    [IgnoreDataMember]
    [Column(TypeName = "jsonb")]
[Innofactor.EfCoreJsonValueConverter.JsonField]
    public TradingPartnerDocument? TradingPartnerDocument { get;  set; }


    public static TradingPartner Create(TradingPartnerForCreationDto tradingPartnerForCreationDto)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.AddProfile<TradingPartnerProfile>();
        }));
        var newTradingPartner = mapper.Map<TradingPartner>(tradingPartnerForCreationDto);
        newTradingPartner.QueueDomainEvent(new TradingPartnerCreated(){ TradingPartner = newTradingPartner });
        
        return newTradingPartner;
    }

    public void Update(TradingPartnerForUpdateDto tradingPartnerForUpdateDto)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.AddProfile<TradingPartnerProfile>();
            cfg.IgnoreSourceWhenNull();
        }));
        mapper.Map(tradingPartnerForUpdateDto, this);
        QueueDomainEvent(new TradingPartnerUpdated(){ Id = Id });
    }
    
    protected TradingPartner() { } // For EF + Mocking
}