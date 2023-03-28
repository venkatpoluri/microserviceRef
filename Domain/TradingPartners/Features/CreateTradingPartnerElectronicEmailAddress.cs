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

public static class CreateTradingPartnerElectronicEmailAddress
{
    public class CreateTradingPartnerElectronicEmailAddressQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
public Electronic_email_addresses Electronicemailaddresses { get; set;}


        public CreateTradingPartnerElectronicEmailAddressQuery(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Electronic_email_addresses electronicemailaddresses)
        {
           TradingPartnerId = tradingPartnerId;
Electronicemailaddresses = electronicemailaddresses;
        }
    }

     public class Handler : IRequestHandler<CreateTradingPartnerElectronicEmailAddressQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CreateTradingPartnerElectronicEmailAddressQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            
           
            
            if(tradingPartnerQuery.TradingPartnerDocument  == null)
            {
                tradingPartnerQuery.TradingPartnerDocument = new TradingPartnerDocument();
                tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses = new Electronic_addresses();
tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses = new List<Electronic_email_addresses>();
            }
            if(tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses==null)  
            {
               
               tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses = new Electronic_addresses();
tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses = new List<Electronic_email_addresses>();
            } 
            else if(tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses == null) 
            {
                tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses = new List<Electronic_email_addresses>();
            }
            tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses.Add(request.Electronicemailaddresses);
         
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
             new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(tradingPartnerForUpdateDto);
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
