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

public static class AssignTradingPartnerRoleToTradingPartner
{
    public class AssignTradingPartnerRoleToTradingPartnerQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
public Roles Roles { get; set;}


        public AssignTradingPartnerRoleToTradingPartnerQuery(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Roles roles)
        {
           TradingPartnerId = tradingPartnerId;
Roles = roles;
        }
    }

     public class Handler : IRequestHandler<AssignTradingPartnerRoleToTradingPartnerQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(AssignTradingPartnerRoleToTradingPartnerQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            
           
            
            if(tradingPartnerQuery.TradingPartnerDocument  == null)
            {
                tradingPartnerQuery.TradingPartnerDocument = new TradingPartnerDocument();
                tradingPartnerQuery.TradingPartnerDocument.Roles = new List<Roles>();
            }
            else if(tradingPartnerQuery.TradingPartnerDocument.Roles == null) 
            {
                tradingPartnerQuery.TradingPartnerDocument.Roles = new List<Roles>();
            }
            tradingPartnerQuery.TradingPartnerDocument.Roles.Add(request.Roles);
         
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
             new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(tradingPartnerForUpdateDto);
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
