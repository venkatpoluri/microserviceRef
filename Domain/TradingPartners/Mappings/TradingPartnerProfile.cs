namespace TradingPartnerManagement.Domain.TradingPartners.Mappings;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using AutoMapper;
using TradingPartnerManagement.Domain.TradingPartners;

public class TradingPartnerProfile : Profile
{
    public TradingPartnerProfile()
    {
        //createmap<to this, from this>
        CreateMap<TradingPartner, TradingPartnerDto>()
            .ReverseMap();
        CreateMap<TradingPartnerForCreationDto, TradingPartner>();
        CreateMap<TradingPartnerForUpdateDto, TradingPartner>()
            .ReverseMap();

    }
}