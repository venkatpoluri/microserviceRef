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

public static class UpdateTradingPartnerPostalAddressById
{
    public class UpdateTradingPartnerPostalAddressByIdQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
        public Guid PostalAddressId { get; set; }
public Postal_addresses Postaladdresses { get; set;}


        public UpdateTradingPartnerPostalAddressByIdQuery(Guid tradingPartnerId, Guid postalAddressId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Postal_addresses postaladdresses)
        {
           TradingPartnerId = tradingPartnerId;
PostalAddressId = postalAddressId;
Postaladdresses = postaladdresses;
        }
    }

     public class Handler : IRequestHandler<UpdateTradingPartnerPostalAddressByIdQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateTradingPartnerPostalAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            if(tradingPartnerQuery.TradingPartnerDocument.Postal_addresses.Where(x => x.Id == request.PostalAddressId).ToList().Count ()== 0)
                        throw new NotFoundException("TradingPartnerPostal_addresses");

                tradingPartnerQuery.TradingPartnerDocument.Postal_addresses =  tradingPartnerQuery.TradingPartnerDocument.Postal_addresses.
                                                                 Where(x => x.Id != request.PostalAddressId)
                                                               .ToList();
           
            tradingPartnerQuery.TradingPartnerDocument.Postal_addresses.Add(request.Postaladdresses);
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
             new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(tradingPartnerForUpdateDto);
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
