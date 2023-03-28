namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;

public static class GetTradingPartnerElectronicAddressByEmailId
{
    public class GetTradingPartnerElectronicAddressByEmailIdQuery : IRequest<Electronic_email_addresses>
    {
         public Guid EmailId { get; set; }


        public GetTradingPartnerElectronicAddressByEmailIdQuery(Guid emailId)
        {
           EmailId = emailId;

        }
    }

    public class Handler : IRequestHandler<GetTradingPartnerElectronicAddressByEmailIdQuery, Electronic_email_addresses>
    {
        private readonly TradingPartnerDbContext _db;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<Electronic_email_addresses> Handle(GetTradingPartnerElectronicAddressByEmailIdQuery request, CancellationToken cancellationToken)
        {
           var TradingPartnerListQuery = await _db.TradingPartners.Where(x=>(( EF.Functions.JsonContains(x.TradingPartnerDocument, @"{""electronic_addresses"":{""electronic_email_addresses"":[{""id"":" + @"""" + request.EmailId + @"""}]}}") ))).ToListAsync();
             if (TradingPartnerListQuery.Count ()== 0 || TradingPartnerListQuery == null)
                throw new NotFoundException("TradingPartner");

            var tradingPartnerDocument = TradingPartnerListQuery.FirstOrDefault().TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses.FirstOrDefault(x=> x.Id == request.EmailId);

             return _mapper.Map<Electronic_email_addresses>(tradingPartnerDocument);
           
        }
    }
}