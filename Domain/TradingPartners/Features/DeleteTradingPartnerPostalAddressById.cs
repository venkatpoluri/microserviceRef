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

public static class DeleteTradingPartnerPostalAddressById
{
    public class DeleteTradingPartnerPostalAddressByIdQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
        public Guid PostalAddressId { get; set; }


        public DeleteTradingPartnerPostalAddressByIdQuery(Guid tradingPartnerId, Guid postalAddressId)
        {
           TradingPartnerId = tradingPartnerId;
PostalAddressId = postalAddressId;

        }
    }

     public class Handler : IRequestHandler<DeleteTradingPartnerPostalAddressByIdQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteTradingPartnerPostalAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            if(tradingPartnerQuery.TradingPartnerDocument.Postal_addresses.Where(x => x.Id == request.PostalAddressId).ToList().Count ()== 0)
                        throw new NotFoundException("TradingPartnerPostal_addresses");

                tradingPartnerQuery.TradingPartnerDocument.Postal_addresses =  tradingPartnerQuery.TradingPartnerDocument.Postal_addresses.
                                                                 Where(x => x.Id != request.PostalAddressId)
                                                               .ToList();
           
            
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
            
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
