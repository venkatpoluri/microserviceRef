namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;

public static class GetTradingPartnerElectronicAddressByWebsiteId
{
    public class GetTradingPartnerElectronicAddressByWebsiteIdQuery : IRequest<Electronic_website_addresses>
    {
         public Guid WebsiteId { get; set; }


        public GetTradingPartnerElectronicAddressByWebsiteIdQuery(Guid websiteId)
        {
           WebsiteId = websiteId;

        }
    }

    public class Handler : IRequestHandler<GetTradingPartnerElectronicAddressByWebsiteIdQuery, Electronic_website_addresses>
    {
        private readonly TradingPartnerDbContext _db;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<Electronic_website_addresses> Handle(GetTradingPartnerElectronicAddressByWebsiteIdQuery request, CancellationToken cancellationToken)
        {
           var TradingPartnerListQuery = await _db.TradingPartners.Where(x=>(( EF.Functions.JsonContains(x.TradingPartnerDocument, @"{""electronic_addresses"":{""electronic_website_addresses"":[{""id"":" + @"""" + request.WebsiteId + @"""}]}}") ))).ToListAsync();
             if (TradingPartnerListQuery.Count ()== 0 || TradingPartnerListQuery == null)
                throw new NotFoundException("TradingPartner");

            var tradingPartnerDocument = TradingPartnerListQuery.FirstOrDefault().TradingPartnerDocument.Electronic_addresses.Electronic_website_addresses.FirstOrDefault(x=> x.Id == request.WebsiteId);

             return _mapper.Map<Electronic_website_addresses>(tradingPartnerDocument);
           
        }
    }
}