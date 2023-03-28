namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners;
using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Domain.TradingPartners.Validators;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Services;
using TradingPartnerManagement.Databases;
using AutoMapper;
using MediatR;
using FluentValidation;

public static class UpdateTradingPartner
{
    public class UpdateTradingPartnerCommand : IRequest<bool>
    {
        public readonly Guid Id;
        public readonly TradingPartnerForUpdateDto TradingPartnerToUpdate;

        public UpdateTradingPartnerCommand(Guid tradingPartner, TradingPartnerForUpdateDto newTradingPartnerData)
        {
            Id = tradingPartner;
            TradingPartnerToUpdate = newTradingPartnerData;
        }
    }

    public class Handler : IRequestHandler<UpdateTradingPartnerCommand, bool>
    {
        private readonly ITradingPartnerRepository _tradingPartnerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TradingPartnerDbContext _dbContext;

        public Handler(ITradingPartnerRepository tradingPartnerRepository, IUnitOfWork unitOfWork, IMapper mapper,TradingPartnerDbContext dbContext)
        {
            _mapper = mapper;
            _tradingPartnerRepository = tradingPartnerRepository;
            _unitOfWork = unitOfWork;
            _dbContext =dbContext;
        }

        public async Task<bool> Handle(UpdateTradingPartnerCommand request, CancellationToken cancellationToken)
        {
            new TradingPartnerForUpdateDtoValidator(_dbContext).ValidateAndThrow(request.TradingPartnerToUpdate);
       
            var tradingPartnerToUpdate = await _tradingPartnerRepository.GetById(request.Id, cancellationToken: cancellationToken);

            tradingPartnerToUpdate.Update(request.TradingPartnerToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);

            return true;
        }
    }
}