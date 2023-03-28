namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;

public static class GetTradingPartnerPostalAddressById
{
    public class GetTradingPartnerPostalAddressByIdQuery : IRequest<Postal_addresses>
    {
         public Guid PostalAddressId { get; set; }


        public GetTradingPartnerPostalAddressByIdQuery(Guid postalAddressId)
        {
           PostalAddressId = postalAddressId;

        }
    }

    public class Handler : IRequestHandler<GetTradingPartnerPostalAddressByIdQuery, Postal_addresses>
    {
        private readonly TradingPartnerDbContext _db;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<Postal_addresses> Handle(GetTradingPartnerPostalAddressByIdQuery request, CancellationToken cancellationToken)
        {
           var TradingPartnerListQuery = await _db.TradingPartners.Where(x=>(( EF.Functions.JsonContains(x.TradingPartnerDocument, @"{""postal_addresses"":[{""id"":" + @"""" + request.PostalAddressId + @"""}]}") ))).ToListAsync();
             if (TradingPartnerListQuery.Count ()== 0 || TradingPartnerListQuery == null)
                throw new NotFoundException("TradingPartner");

            var tradingPartnerDocument = TradingPartnerListQuery.FirstOrDefault().TradingPartnerDocument.Postal_addresses.FirstOrDefault(x=> x.Id == request.PostalAddressId);

             return _mapper.Map<Postal_addresses>(tradingPartnerDocument);
           
        }
    }
}