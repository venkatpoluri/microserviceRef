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

public static class CreateTradingPartnerPostalAddress
{
    public class CreateTradingPartnerPostalAddressQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
public Postal_addresses Postaladdresses { get; set;}


        public CreateTradingPartnerPostalAddressQuery(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Postal_addresses postaladdresses)
        {
           TradingPartnerId = tradingPartnerId;
Postaladdresses = postaladdresses;
        }
    }

     public class Handler : IRequestHandler<CreateTradingPartnerPostalAddressQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CreateTradingPartnerPostalAddressQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            
           
            
            if(tradingPartnerQuery.TradingPartnerDocument  == null)
            {
                tradingPartnerQuery.TradingPartnerDocument = new TradingPartnerDocument();
                tradingPartnerQuery.TradingPartnerDocument.Postal_addresses = new List<Postal_addresses>();
            }
            else if(tradingPartnerQuery.TradingPartnerDocument.Postal_addresses == null) 
            {
                tradingPartnerQuery.TradingPartnerDocument.Postal_addresses = new List<Postal_addresses>();
            }
            tradingPartnerQuery.TradingPartnerDocument.Postal_addresses.Add(request.Postaladdresses);
         
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
             new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(tradingPartnerForUpdateDto);
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
