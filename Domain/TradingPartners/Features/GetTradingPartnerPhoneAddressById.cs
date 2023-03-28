namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;

public static class GetTradingPartnerPhoneAddressById
{
    public class GetTradingPartnerPhoneAddressByIdQuery : IRequest<Phone_addresses>
    {
         public Guid PhoneAddressId { get; set; }


        public GetTradingPartnerPhoneAddressByIdQuery(Guid phoneAddressId)
        {
           PhoneAddressId = phoneAddressId;

        }
    }

    public class Handler : IRequestHandler<GetTradingPartnerPhoneAddressByIdQuery, Phone_addresses>
    {
        private readonly TradingPartnerDbContext _db;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<Phone_addresses> Handle(GetTradingPartnerPhoneAddressByIdQuery request, CancellationToken cancellationToken)
        {
           var TradingPartnerListQuery = await _db.TradingPartners.Where(x=>(( EF.Functions.JsonContains(x.TradingPartnerDocument, @"{""phone_addresses"":[{""id"":" + @"""" + request.PhoneAddressId + @"""}]}") ))).ToListAsync();
             if (TradingPartnerListQuery.Count ()== 0 || TradingPartnerListQuery == null)
                throw new NotFoundException("TradingPartner");

            var tradingPartnerDocument = TradingPartnerListQuery.FirstOrDefault().TradingPartnerDocument.Phone_addresses.FirstOrDefault(x=> x.Id == request.PhoneAddressId);

             return _mapper.Map<Phone_addresses>(tradingPartnerDocument);
           
        }
    }
}