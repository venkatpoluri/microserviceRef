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

public static class DeleteTradingPartnerRoleStatusById
{
    public class DeleteTradingPartnerRoleStatusByIdQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
        public Guid RoleId { get; set; }
        public Guid StatusId { get; set; }


        public DeleteTradingPartnerRoleStatusByIdQuery(Guid tradingPartnerId, Guid roleId, Guid statusId)
        {
           TradingPartnerId = tradingPartnerId;
RoleId = roleId;
StatusId = statusId;

        }
    }

     public class Handler : IRequestHandler<DeleteTradingPartnerRoleStatusByIdQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteTradingPartnerRoleStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            
                          var roles = tradingPartnerQuery.TradingPartnerDocument.Roles.Where(x => x.Id == request.RoleId).SingleOrDefault();
                    tradingPartnerQuery.TradingPartnerDocument.Roles.Remove(roles);
                        roles.Statuses =    roles.Statuses.Where(x => x.Id != request.StatusId).ToList();
                        tradingPartnerQuery.TradingPartnerDocument.Roles.Add(roles);
                    
           
            
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
            
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
