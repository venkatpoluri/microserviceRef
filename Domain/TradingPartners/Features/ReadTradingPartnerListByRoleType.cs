namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;

public static class ReadTradingPartnerListByRoleType
{
    public class ReadTradingPartnerListByRoleTypeQuery : IRequest<List<TradingPartnerDto>>
    {
         public string Type { get; set; }


        public ReadTradingPartnerListByRoleTypeQuery(string type)
        {
           Type = type;

        }
    }

    public class Handler : IRequestHandler<ReadTradingPartnerListByRoleTypeQuery, List<TradingPartnerDto>>
    {
        private readonly TradingPartnerDbContext _db;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<List<TradingPartnerDto>> Handle(ReadTradingPartnerListByRoleTypeQuery request, CancellationToken cancellationToken)
        {
           var TradingPartnerListQuery = await _db.TradingPartners.Where(x=>(( EF.Functions.JsonContains(x.TradingPartnerDocument, @"{""roles"":[{""type"":" + @"""" + request.Type + @"""}]}") ))).ToListAsync();
             if (TradingPartnerListQuery.Count ()== 0 || TradingPartnerListQuery == null)
                throw new NotFoundException("TradingPartner");

            return _mapper.Map<List<TradingPartnerDto>>(TradingPartnerListQuery);
           
        }
    }
}