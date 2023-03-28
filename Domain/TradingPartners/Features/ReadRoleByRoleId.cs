namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;

public static class ReadRoleByRoleId
{
    public class ReadRoleByRoleIdQuery : IRequest<Roles>
    {
         public Guid RoleId { get; set; }


        public ReadRoleByRoleIdQuery(Guid roleId)
        {
           RoleId = roleId;

        }
    }

    public class Handler : IRequestHandler<ReadRoleByRoleIdQuery, Roles>
    {
        private readonly TradingPartnerDbContext _db;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<Roles> Handle(ReadRoleByRoleIdQuery request, CancellationToken cancellationToken)
        {
           var TradingPartnerListQuery = await _db.TradingPartners.Where(x=>(( EF.Functions.JsonContains(x.TradingPartnerDocument, @"{""roles"":[{""id"":" + @"""" + request.RoleId + @"""}]}") ))).ToListAsync();
             if (TradingPartnerListQuery.Count ()== 0 || TradingPartnerListQuery == null)
                throw new NotFoundException("TradingPartner");

            var tradingPartnerDocument = TradingPartnerListQuery.FirstOrDefault().TradingPartnerDocument.Roles.FirstOrDefault(x=> x.Id == request.RoleId);

             return _mapper.Map<Roles>(tradingPartnerDocument);
           
        }
    }
}