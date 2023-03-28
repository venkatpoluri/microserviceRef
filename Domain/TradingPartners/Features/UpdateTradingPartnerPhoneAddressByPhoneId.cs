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

public static class UpdateTradingPartnerPhoneAddressByPhoneId
{
    public class UpdateTradingPartnerPhoneAddressByPhoneIdQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
        public Guid PhoneAddressId { get; set; }
public Phone_addresses Phoneaddresses { get; set;}


        public UpdateTradingPartnerPhoneAddressByPhoneIdQuery(Guid tradingPartnerId, Guid phoneAddressId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Phone_addresses phoneaddresses)
        {
           TradingPartnerId = tradingPartnerId;
PhoneAddressId = phoneAddressId;
Phoneaddresses = phoneaddresses;
        }
    }

     public class Handler : IRequestHandler<UpdateTradingPartnerPhoneAddressByPhoneIdQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateTradingPartnerPhoneAddressByPhoneIdQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            if(tradingPartnerQuery.TradingPartnerDocument.Phone_addresses.Where(x => x.Id == request.PhoneAddressId).ToList().Count ()== 0)
                        throw new NotFoundException("TradingPartnerPhone_addresses");

                tradingPartnerQuery.TradingPartnerDocument.Phone_addresses =  tradingPartnerQuery.TradingPartnerDocument.Phone_addresses.
                                                                 Where(x => x.Id != request.PhoneAddressId)
                                                               .ToList();
           
            tradingPartnerQuery.TradingPartnerDocument.Phone_addresses.Add(request.Phoneaddresses);
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
             new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(tradingPartnerForUpdateDto);
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
