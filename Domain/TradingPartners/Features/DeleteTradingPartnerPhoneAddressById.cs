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

public static class DeleteTradingPartnerPhoneAddressById
{
    public class DeleteTradingPartnerPhoneAddressByIdQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
        public Guid PhoneAddressId { get; set; }


        public DeleteTradingPartnerPhoneAddressByIdQuery(Guid tradingPartnerId, Guid phoneAddressId)
        {
           TradingPartnerId = tradingPartnerId;
PhoneAddressId = phoneAddressId;

        }
    }

     public class Handler : IRequestHandler<DeleteTradingPartnerPhoneAddressByIdQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteTradingPartnerPhoneAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            if(tradingPartnerQuery.TradingPartnerDocument.Phone_addresses.Where(x => x.Id == request.PhoneAddressId).ToList().Count ()== 0)
                        throw new NotFoundException("TradingPartnerPhone_addresses");

                tradingPartnerQuery.TradingPartnerDocument.Phone_addresses =  tradingPartnerQuery.TradingPartnerDocument.Phone_addresses.
                                                                 Where(x => x.Id != request.PhoneAddressId)
                                                               .ToList();
           
            
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
            
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
