namespace TradingPartnerManagement.Domain.TradingPartners.DomainEvents;

public class TradingPartnerCreated : DomainEvent
{
    public TradingPartner TradingPartner { get; set; } 
}
            