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

public static class CreateTradingPartnerPhoneAddress
{
    public class CreateTradingPartnerPhoneAddressQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
public Phone_addresses Phoneaddresses { get; set;}


        public CreateTradingPartnerPhoneAddressQuery(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Phone_addresses phoneaddresses)
        {
           TradingPartnerId = tradingPartnerId;
Phoneaddresses = phoneaddresses;
        }
    }

     public class Handler : IRequestHandler<CreateTradingPartnerPhoneAddressQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CreateTradingPartnerPhoneAddressQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            
           
            
            if(tradingPartnerQuery.TradingPartnerDocument  == null)
            {
                tradingPartnerQuery.TradingPartnerDocument = new TradingPartnerDocument();
                tradingPartnerQuery.TradingPartnerDocument.Phone_addresses = new List<Phone_addresses>();
            }
            else if(tradingPartnerQuery.TradingPartnerDocument.Phone_addresses == null) 
            {
                tradingPartnerQuery.TradingPartnerDocument.Phone_addresses = new List<Phone_addresses>();
            }
            tradingPartnerQuery.TradingPartnerDocument.Phone_addresses.Add(request.Phoneaddresses);
         
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
             new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(tradingPartnerForUpdateDto);
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
