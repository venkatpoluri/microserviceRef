namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using TradingPartnerManagement.Domain.TradingPartners.Validators;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;
using Newtonsoft.Json;
using FluentValidation;

public static class UnAssignTradingPartnerRoleFromTradingPartner
{
    public class UnAssignTradingPartnerRoleFromTradingPartnerQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
        public Guid RoleId { get; set; }


        public UnAssignTradingPartnerRoleFromTradingPartnerQuery(Guid tradingPartnerId, Guid roleId)
        {
           TradingPartnerId = tradingPartnerId;
RoleId = roleId;

        }
    }

     public class Handler : IRequestHandler<UnAssignTradingPartnerRoleFromTradingPartnerQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UnAssignTradingPartnerRoleFromTradingPartnerQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            if(tradingPartnerQuery.TradingPartnerDocument.Roles.Where(x => x.Id == request.RoleId).ToList().Count ()== 0)
                        throw new NotFoundException("TradingPartnerRoles");

                tradingPartnerQuery.TradingPartnerDocument.Roles =  tradingPartnerQuery.TradingPartnerDocument.Roles.
                                                                 Where(x => x.Id != request.RoleId)
                                                               .ToList();
           
            
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
            
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
