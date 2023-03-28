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

public static class UpdateTradingPartnerElectronicAddressByEmailId
{
    public class UpdateTradingPartnerElectronicAddressByEmailIdQuery : IRequest<bool>
    {
                public Guid TradingPartnerId { get; set; }
        public Guid EmailId { get; set; }
public Electronic_email_addresses Electronicemailaddresses { get; set;}


        public UpdateTradingPartnerElectronicAddressByEmailIdQuery(Guid tradingPartnerId, Guid emailId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Electronic_email_addresses electronicemailaddresses)
        {
           TradingPartnerId = tradingPartnerId;
EmailId = emailId;
Electronicemailaddresses = electronicemailaddresses;
        }
    }

     public class Handler : IRequestHandler<UpdateTradingPartnerElectronicAddressByEmailIdQuery, bool>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateTradingPartnerElectronicAddressByEmailIdQuery request, CancellationToken cancellationToken)
        {
            var tradingPartnerQuery = await _dbContext.TradingPartners.FirstOrDefaultAsync(t => t.Id == request.TradingPartnerId, cancellationToken);
            if (tradingPartnerQuery == null)
                throw new NotFoundException("TradingPartner");
            if(tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses.Where(x => x.Id == request.EmailId).ToList().Count ()== 0)
                        throw new NotFoundException("TradingPartnerElectronic_addresses.Electronic_email_addresses");

                tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses =  tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses.
                                                                 Where(x => x.Id != request.EmailId)
                                                               .ToList();
           
            tradingPartnerQuery.TradingPartnerDocument.Electronic_addresses.Electronic_email_addresses.Add(request.Electronicemailaddresses);
             
            TradingPartnerForUpdateDto tradingPartnerForUpdateDto = new TradingPartnerForUpdateDto(tradingPartnerQuery);
       
             new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(tradingPartnerForUpdateDto);
          
            tradingPartnerQuery.Update(tradingPartnerForUpdateDto);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
